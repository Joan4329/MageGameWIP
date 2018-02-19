using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSpellManager : MonoBehaviour {

    private Spell currentSelectedSpell;
    private Spell[] knownSpells;
    private int spellSlots = 6;
    private int queuedSpellSlot = -1;

    //private bool isCasting = false;
    private Animator playerCharacterAnimator;
    private PlayerController playerController;
    private PlayerStats playerStats;
    private PlayerGUIManager playerGUIMngr;

    // Casting events, one for initiating a spellcast and one for stopping.
    public delegate void OnCasting();
    private OnCasting onStartCasting;
    private OnCasting onStopCasting;
    // Change spell event. Takes one spell as parameter so that other classes implementing it won't need to check currently selected spell from this class. Might do the same with onStartCasting and onStopCasting. I don't think I need to care about what spell was selected before this was called, but it might be worth thinking about.
    public delegate void OnChangeSelectedSpell(Spell newSelectedSpell);
    private OnChangeSelectedSpell onChangeSpell;
    // I'm not exactly sure what the standard is for naming delegates if all of these are basically the same. (i.e. I don't need 3 different delegates for OnCasting, OnChangeSelectedSpell and OnFinishSpellCast if they all have the same signature, but what should I call it then)
    // Event that is called when a spell cast has been completed and successful.
    public delegate void OnFinishSpellCast(Spell spell);
    private OnFinishSpellCast onSpellFinish;


    public void AddOnStartCastingListener(OnCasting onStartCasting)
    {
        this.onStartCasting += onStartCasting;
    }

    public void AddOnStopCastingListener(OnCasting onStopCasting)
    {
        this.onStopCasting += onStopCasting;
    }

    public void AddOnChangeSpellListener(OnChangeSelectedSpell onChangeSpell)
    {
        this.onChangeSpell += onChangeSpell;
    }

    public void AddOnFinishSpellCastListener(OnFinishSpellCast onSpellFinish)
    {
        this.onSpellFinish += onSpellFinish;
    }

    public Spell CurrentSelectedSpell
    {
        get { return currentSelectedSpell; }
    }

    private void Awake()
    {
        playerCharacterAnimator = GetComponentInChildren<Animator>();
        playerController = GetComponent<PlayerController>();
        playerStats = GetComponent<PlayerStats>();
        playerGUIMngr = GetComponent<PlayerGUIManager>();
    }

    // Use this for initialization
    void Start () {
        knownSpells = new Spell[spellSlots];
        knownSpells[0] = new Fireball(1.5f, 1.0f, 30f, Spell.SpellAnimation.Standard, 0f);
        knownSpells[1] = new Blink(0.6f, 6f, 40f, Spell.SpellAnimation.AreaAttack, 1f, 15f);
        knownSpells[2] = new Rejuvenation(2f, 12f, 50f, Spell.SpellAnimation.Buff, -0.3f);
        knownSpells[3] = new LunarBeam(1f, 1.5f, 10f, Spell.SpellAnimation.Beam, 0f, 20f);
        //knownSpells[3] = new LunarBeam(0.3f, 0.2f, 5f, Spell.SpellAnimation.Beam, 0f, 5f);
        knownSpells[4] = new ForceShield(1f, 30f, 10f, Spell.SpellAnimation.Buff, 0f, 100f);
        currentSelectedSpell = knownSpells[0];

        AddOnStartCastingListener(StartCasting);
        AddOnStopCastingListener(StopCasting);
        AddOnFinishSpellCastListener(FinishSpellCast);
    }
	
	// Update is called once per frame
	void Update () {
        // Also for this and PlayerController Update methods; add listeners instead even for movement. No point in processing shit every single frame.

        if (playerStats.IsAlive)
        {
            // Start casting currently selected spell. Press and hold to keep casting spell. Release to stop casting. Will need to be rewritten since onStopCasting is invoked every frame now, I just realized.
            if (Input.GetAxis("Fire1") > 0 && !IsCurrentSpellOnCooldown() && currentSelectedSpell.ManaCost <= playerStats.CurrentMana)
            {
                if (!currentSelectedSpell.IsCasting)
                    onStartCasting.Invoke();
            }
            else
                onStopCasting.Invoke();   

            // Delayed switch to the spell that was selected while casting a spell.
            if (!currentSelectedSpell.IsCasting && queuedSpellSlot != -1)
            {
                currentSelectedSpell = knownSpells[queuedSpellSlot];
                onChangeSpell.Invoke(currentSelectedSpell);
                queuedSpellSlot = -1;
            }

            // Change spell with number keys 1-6 or however many spell slots there are.
            for (int i = 0; i < spellSlots; i++)
                if (Input.GetKeyUp((KeyCode)(i + 49)))
                    if (knownSpells[i] != null)
                    {
                        if(!currentSelectedSpell.IsCasting)
                        {
                            currentSelectedSpell = knownSpells[i];
                            onChangeSpell.Invoke(currentSelectedSpell);
                        }
                        else
                        {
                            queuedSpellSlot = i;
                        }
                    }
        }
        else
        {
            // Give some feedback to player, like a text saying "Can't do that when dead".
        }
    }

    private bool IsCurrentSpellOnCooldown()
    {
        return currentSelectedSpell.IsOnCooldown;
    }


    private void StartCasting()
    {
        playerController.IsCasting = true;
        playerCharacterAnimator.SetBool("Casting", true);
        playerCharacterAnimator.SetFloat("CastSpeed", Mathf.Clamp((1.5f / currentSelectedSpell.castTime) + currentSelectedSpell.AnimationSpeedOffset, 0.1f, 5f));
        playerCharacterAnimator.SetInteger("CastAnimation", currentSelectedSpell.SpellAnimationInt);

        currentSelectedSpell.IsCasting = true;
        StartCoroutine(StartCasting(currentSelectedSpell));
    }


    private IEnumerator StartCasting(Spell spell)
    {
        // I think that this method might cause issues later.

        while (playerController.IsCasting && playerStats.IsAlive)
        {
            spell.CastingTimer += Time.deltaTime;
            // Temporary. Add an event or something later. "WhileCasting" event.
            playerGUIMngr.CastBar.fillAmount = spell.CastingTimer / spell.castTime;

            // Cast spell if cast time exceeded and if player has enough mana.
            if (spell.CastingTimer >= spell.castTime && playerStats.CurrentMana >= currentSelectedSpell.ManaCost)
            {
                onSpellFinish.Invoke(spell);
            }
            // Wait until next frame.
            yield return null;
        }

        // If casting is stopped from outside source we need to reset castingTimer.
        if (!playerController.IsCasting)
        {
            spell.CastingTimer = 0;
            spell.IsCasting = false;
        }
    }

    private void FinishSpellCast(Spell spell)
    {
        // Perform spell effects.
        spell.Cast(transform, playerController.PrevMouseLookRotation);

        // Start spell cooldown.
        spell.IsOnCooldown = true;
        StartCoroutine(StartCooldown(spell));

        // Reset casting state.
        spell.CastingTimer = 0;

        // Deduct mana cost from currentMana.
        playerStats.CurrentMana -= currentSelectedSpell.ManaCost;

        // There might be an issue here where the StopCasting method sets currentSelectedSpell.IsCasting = false; while before this method used spell.IsCasting = false;
        onStopCasting.Invoke();
    }

    private void StopCasting()
    {
        playerCharacterAnimator.SetBool("Casting", false);
        playerController.IsCasting = false;
        currentSelectedSpell.IsCasting = false;        
    }


    private IEnumerator StartCooldown(Spell spell)
    {
        while (spell.IsOnCooldown)
        {
            spell.CurrentCDTimer += Time.deltaTime;
            // Temporary. Add an event or something later. "WhileOnCooldown".
            if (currentSelectedSpell == spell && spell.cooldownTime > PlayerGUIManager.COOLDOWN_BAR_THRESHOLD)
                playerGUIMngr.CooldownBar.fillAmount = 1 - (spell.CurrentCDTimer / spell.cooldownTime);

            if (spell.CurrentCDTimer >= spell.cooldownTime)
            {
                spell.CurrentCDTimer = 0;
                spell.IsOnCooldown = false;
            }
            yield return null;
        }

        // Temporary. Add an event or something later. "OnCooldownFinish".
        if (currentSelectedSpell == spell)
            playerGUIMngr.CooldownBar.rectTransform.parent.gameObject.SetActive(false);
    }
}
