using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System;

public class CombatStats : MonoBehaviour {
    public const int ATTACKABLE_LAYER = 1 << 8;
    public const int SCENE_OBJECTS_LAYER = 1 << 9;

    private EffectManager effectManager;

    private bool isAlive = true;

    protected float currentHealth;
    protected float currentMana;

    // Add later so that maxHealth and maxMana can't be 0 or under.
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxMana = 100;
    [SerializeField] private float healthRegen = 1f;
    [SerializeField] private float healthRegenThreshold = 0.15f;
    [SerializeField] private float manaRegenSetter = 5f;
    private ModifiableStat<ManaRegenModifier, ManaRegenAdditive> manaRegen;

    private float manaRegenModifier = 1f;
    private float currentShield = 0f;
    
    private ModifiableStat<EnergyShieldModifier, EnergyShieldAdditive> maxShield;

    private bool isRegeneratingHealth = false;
    private bool isRegeneratingMana = false;

    private float lastHitTimeStamp = 0;
    private bool isHitRecently = false;

    [SerializeField] private float meleeDamage = 40f;
    [SerializeField] private float meleeAttackSpeed = 1f;

    // Events for death and damage. Change these to delegates or my own event class instead.
    private UnityEvent deathEvent = new UnityEvent();
    private TakeDamageEvent takeDamageEvent = new TakeDamageEvent();

    public class TakeDamageEvent : UnityEvent<Damage>
    {
    }

    // Events for value changes in health, mana and energy shield.
    public delegate void OnValueChanged();
    private OnValueChanged onHealthChanged;
    private OnValueChanged onManaChanged;
    private OnValueChanged onShieldChanged;

    public void AddOnHealthChangedListener(OnValueChanged onHealthChanged)
    {
        this.onHealthChanged += onHealthChanged;
    }

    public void AddOnManaChangedListener(OnValueChanged onManaChanged)
    {
        this.onManaChanged += onManaChanged;
    }

    public void AddOnShieldChangedListener(OnValueChanged onShieldChanged)
    {
        this.onShieldChanged += onShieldChanged;
    }

    // Might need to add some sort of base-health or change "currentHealth = maxHealth;" to "currentHealth = maxHealth + GetTotalBonusHealthFromItems();" when I add equipment. Same for mana.
    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            if (value < 0)
                currentHealth = 0;
            else if (value > maxHealth)
                currentHealth = maxHealth;
            else
                currentHealth = value;

            onHealthChanged.Invoke();
        }
    }

    public float CurrentMana
    {
        get { return currentMana; }
        set
        {
            if (value < 0)
                currentMana = 0;
            else if (value > maxMana)
                currentMana = maxMana;
            else
                currentMana = value;
            // Move invokes because they cause null references at the start of the game.
            onManaChanged.Invoke();
        }
    }

    public float CurrentShield
    {
        get { return currentShield; }
        set
        {
            if (value < 0)
                currentShield = 0;
            else if (value > maxShield.Value)
                currentShield = maxShield.Value;
            else
                currentShield = value;

            onShieldChanged.Invoke();
        }
    }

    public float HealthRegenThreshold
    {
        get { return healthRegenThreshold; }
        set
        {
            if (value < 0)
                healthRegenThreshold = 0;
            else if (value > 1)
                healthRegenThreshold = 1;
            else
                healthRegenThreshold = value;
        }
    }

    public bool IsAlive
    {
        get { return isAlive; }
        set { isAlive = value; }
    }

    public UnityEvent DeathEvent
    {
        get { return deathEvent; }
        //set { deathEvent = value; }
    }

    public TakeDamageEvent DamageEvent
    {
        get { return takeDamageEvent; }
        //set { takeDamageEvent = value; }
    }

    public float MeleeDamage
    {
        get { return meleeDamage; }
        set { meleeDamage = value; }
    }

    public float MeleeAttackSpeed
    {
        get { return meleeAttackSpeed; }
        //set { meleeAttackAnimSpeed = value; }
    }

    public ModifiableStat<ManaRegenModifier, ManaRegenAdditive> ManaRegen
    {
        get { return manaRegen; }
    }

    public ModifiableStat<EnergyShieldModifier, EnergyShieldAdditive> MaxShield
    {
        get { return maxShield; }
    }

    protected virtual void Awake()
    {
        effectManager = GetComponent<EffectManager>();
        manaRegen = new ModifiableStat<ManaRegenModifier, ManaRegenAdditive>(manaRegenSetter, false, effectManager);
        maxShield = new ModifiableStat<EnergyShieldModifier, EnergyShieldAdditive>(0f, true, effectManager);

        currentHealth = maxHealth;
        currentMana = maxMana;
        currentShield = maxShield.Value;
    }

    // Use this for initialization
    void Start()
    {
        // Can I somehow tie this to the const instead?
        gameObject.layer = LayerMask.NameToLayer("Attackable");
        
        // As mentioned above, replace with delegates or my own event class.
        deathEvent.AddListener(DeathActions);
        takeDamageEvent.AddListener(TakeDamageActions);
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        // Move these back into each property or on value changed methods so that there's no need to check each update.
        if (!isRegeneratingHealth && GetCurrentHealthInPercent() < HealthRegenThreshold && !isHitRecently && CurrentHealth < maxHealth && IsAlive)
            StartCoroutine(RegenHealth());
        if (!isRegeneratingMana && currentMana < maxMana)
            StartCoroutine(RegenMana());
    }

    //private void RegenHealth()
    //{
    //    if (GetCurrentHealthInPercent() < HealthRegenThreshold && !isHitRecently && CurrentHealth < maxHealth && IsAlive)
    //    {
    //        // Start health regen if wounded and health is below threshold and if not hit recently.
    //        // Also maybe regen in chunks at a time using coroutine with a WaitForSeconds of 0.25f to 1f? Same for mana. That way it won't run useless if-statements each update.
    //        CurrentHealth += healthRegen * Time.deltaTime;
    //    }
    //}

    //private void RegenMana()
    //{
    //    if (CurrentMana < maxMana && IsAlive)
    //    {
    //        // Start mana regen if mana is below max and not dead.
    //        CurrentMana += manaRegen * manaRegenModifier * Time.deltaTime;
    //    }
    //}

    private IEnumerator RegenHealth()
    {
        isRegeneratingHealth = true;
        while (GetCurrentHealthInPercent() < HealthRegenThreshold && !isHitRecently && CurrentHealth < maxHealth && IsAlive)
        {
            // Start health regen if wounded and health is below threshold and if not hit recently.
            // Also maybe regen in chunks at a time using coroutine with a WaitForSeconds of 0.25f to 1f? Same for mana. That way it won't run useless if-statements each update.
            CurrentHealth += healthRegen * Time.deltaTime;
            yield return null;
        }
        isRegeneratingHealth = false;
    }

    private IEnumerator RegenMana()
    {
        isRegeneratingMana = true;
        while (CurrentMana < maxMana && IsAlive)
        {
            // Start mana regen if mana is below max and not dead.
            CurrentMana += manaRegen.Value * Time.deltaTime;
            yield return null;
        }
        isRegeneratingMana = false;
    }

    //public void CalculateManaRegenModifier()
    //{
    //    // Create a method called CalculateModifier that takes a Type as argument and the property for that modifier (i.e. manaRegenModifier) and calculates them. Otherwise I will have a million of these.

    //    List<ManaRegenModifier> manaRegenModifiers = effectManager.EffectList.OfType<ManaRegenModifier>().ToList();
    //    if (manaRegenModifiers.Count < 1)
    //    {
    //        manaRegenModifier = 1f;
    //        return;
    //    }

    //    float totalModifier = 1f;
    //    foreach (ManaRegenModifier mrm in manaRegenModifiers)
    //    {
    //        totalModifier *= mrm.Magnitude;
    //    }

    //    if (totalModifier < 0f)
    //        totalModifier = 0f;
    //    manaRegenModifier = totalModifier;
    //}

    //public void CalculateMaxShield()
    //{
    //    // Create a method called CalculateModifier that takes a Type as argument and the property for that modifier (i.e. manaRegenModifier) and calculates them. Otherwise I will have a million of these.
        
    //    List<EnergyShieldAdditive> maxEnergyShieldModifiers = effectManager.EffectList.OfType<EnergyShieldAdditive>().ToList();
    //    if (maxEnergyShieldModifiers.Count < 1)
    //    {
    //        maxShield = 0;
    //        return;
    //    }

    //    float totalModifier = 0f;
    //    foreach (EnergyShieldAdditive esb in maxEnergyShieldModifiers)
    //    {
    //        totalModifier += esb.Magnitude;
    //    }

    //    if (totalModifier < 0f)
    //        totalModifier = 0f;
    //    maxShield = (int)Mathf.Round(totalModifier);
    //}

    // Use struct Damage instead of a float for damage.
    // Struct Damage will have other information, like source, damage type etc. if it will be needed
    public virtual void ApplyDamage(Damage damage)
    {
        
        if (damage.Magnitude < CurrentShield)
        {
            CurrentShield -= Mathf.Abs(damage.Magnitude);
        }
        else
        {
            float healthDamage = CurrentShield - damage.Magnitude;
            CurrentShield -= Mathf.Abs(damage.Magnitude);
            CurrentHealth -= Mathf.Abs(healthDamage);
        }
        
        if( CurrentHealth < 1 && isAlive)
        {
            deathEvent.Invoke();
        }

        // Animate a hit by using blood, fire etc.
        takeDamageEvent.Invoke(damage);
    }

    public virtual void ApplyHeal(Damage damage)
    {
        if (isAlive)
            CurrentHealth += Mathf.Abs(damage.Magnitude);

        // start some heal event.
    }

    // Might make these two methods into properties with only getter instead.
    public float GetCurrentHealthInPercent()
    {
        // Don't need to check if result > 1f since currentHealth can't be above maxHealth. But I do need to check so that maxHealth is not 0 or less for now, since there is no other check for it.
        if (maxHealth <= 0f)
            return 0f;
        else
            return CurrentHealth / maxHealth;
    }

    public float GetCurrentShieldInPercent()
    {
        // Don't need to check if result > 1f since currentHealth can't be above maxHealth. But I do need to check so that maxHealth is not 0 or less for now, since there is no other check for it.
        if (maxShield.Value <= 0f)
            return 0f;
        else
            return CurrentShield / maxShield.Value;
    }

    public float GetCurrentManaInPercent()
    {
        // See above.
        if (maxMana <= 0f)
            return 0f;
        else
            return CurrentMana / maxMana;
    }

    public float GetValueInManaPercent(float manaValue)
    {
        if (manaValue < 0f)
            manaValue = 0f;
        else if (manaValue > maxMana)
            manaValue = maxMana;

        if (maxMana <= 0f)
            return 0f;
        else
            return manaValue / maxMana;
    }

    private void DeathActions()
    {
        Debug.Log("Dead");
        isAlive = false;
        GetComponent<CapsuleCollider>().enabled = false;
        if (GetComponentInChildren<Canvas>() != null)
            GetComponentInChildren<Canvas>().enabled = false;
        GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
    }

    private void TakeDamageActions(Damage damage)
    {
        if (damage.Info.IsDirectHit)
        {
            lastHitTimeStamp = Time.time;
            if (!isHitRecently)
                StartCoroutine(HitRecently());
        }
    }

    private IEnumerator HitRecently()
    {
        isHitRecently = true;
        while (Time.time - lastHitTimeStamp < 4)
        {
            yield return null;            
        }
        isHitRecently = false;
    }
}
