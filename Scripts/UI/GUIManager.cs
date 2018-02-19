using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour {
    private static GameObject floatingBarPrefab;

    // Make a prefab of the canvas with floating bars and instantiate it for each entity instead. Also add an offset control for the canvas along the Y axis. Use a static string with the filepath to the canvas prefab?
    // Then figure out how it determines what constitutes a health bar or mana bar and so on. Use GetComponentsInChildren<Image> somehow? Maybe have a seperate script on the parent object of the prefab that stores such things that adds a listener to CombatStats? I don't like that too much though.
    private Image floatingHealthBar;
    //[SerializeField] private bool useFloatingHealthBar = true;
    private Image floatingManaBar;
    //[SerializeField] private bool useFloatingManaBar = true;
    [SerializeField] private bool useFloatingBars = true;
    [SerializeField] private float floatingBarOffsetY = 2.5f;
    [SerializeField] private float floatingBarScale = 1f;
    // Add these in later.
    //[SerializeField] private Image floatingShieldBar;
    //[SerializeField] private bool useFloatingShieldBar = true;

    protected CombatStats combatStats;
    protected PlayerSpellManager spellMngr;
    protected PlayerController playerController;

    protected virtual void Awake()
    {
        if (floatingBarPrefab == null)
            floatingBarPrefab = Resources.Load<GameObject>("Prefabs/UI/FloatingBars");

        combatStats = GetComponent<CombatStats>();
        spellMngr = GetComponent<PlayerSpellManager>();
        playerController = GetComponent<PlayerController>();
    }

    // Use this for initialization
    protected virtual void Start () {
        if (useFloatingBars == true)
        {
            GameObject instantiatedFloatingBar = Instantiate(floatingBarPrefab, this.gameObject.transform.position, Quaternion.identity, this.gameObject.transform);
            instantiatedFloatingBar.transform.position += new Vector3(0, floatingBarOffsetY, 0);
            instantiatedFloatingBar.transform.localScale = Vector3.one * floatingBarScale;
            // Get the health bar and mana bar images. This should be improved later. Right now it makes a few assumptions. Could use tags but I don't really want to use tags.
            Image[] barImages = instantiatedFloatingBar.GetComponentsInChildren<Image>();
            floatingHealthBar = barImages[1];
            floatingManaBar = barImages[2];
        }
            

        // Hook up methods to events
        combatStats.AddOnHealthChangedListener(UpdateHealthBar);
        combatStats.AddOnManaChangedListener(UpdateManaBar);
        combatStats.AddOnShieldChangedListener(UpdateShieldBar);

        // Update bars.
        UpdateHealthBar();
        UpdateManaBar();
        UpdateShieldBar();
    }

    // Update is called once per frame
    protected virtual void Update () {
		
	}

    protected virtual void UpdateHealthBar()
    {
        if (useFloatingBars)
        {
            // Set health bar fill amount
            if (!combatStats.IsAlive)
                floatingHealthBar.fillAmount = 0;
            else
            {
                floatingHealthBar.fillAmount = combatStats.GetCurrentHealthInPercent();
            }
        }
    }

    protected virtual void UpdateManaBar()
    {
        //Debug.Log("Something!");
        if (useFloatingBars)
        {
            // Set mana bar fill amount
            if (!combatStats.IsAlive)
                floatingManaBar.fillAmount = 0;
            else
            {
                floatingManaBar.fillAmount = combatStats.GetCurrentManaInPercent();
            }
        }
    }

    protected virtual void UpdateShieldBar()
    {
        // Implement this later
        //if (useFloatingManaBar)
        //{
        //    // Set mana bar fill amount
        //    if (!combatStats.IsAlive)
        //        floatingManaBar.fillAmount = 0;
        //    else
        //    {
        //        floatingManaBar.fillAmount = combatStats.GetCurrentManaInPercent();
        //    }
        //}
    }
}
