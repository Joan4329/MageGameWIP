using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUIManager : GUIManager {
    // Determines how many seconds a spell cooldown needs to exceed to show the cooldown bar.
    public const float COOLDOWN_BAR_THRESHOLD = 0.35f;

    [SerializeField] private Image castBar;
    [SerializeField] private Image cooldownBar;

    [SerializeField] private Image playerHealthBarMask;
    [SerializeField] private Image playerHealthBarSurfaceMask;
    [SerializeField] private Image playerManaBarMask;
    [SerializeField] private Image playerManaBarSurfaceMask;
    [SerializeField] private Image playerEnergyShieldBarMask;

    // I don't want CastBar and CooldownBar to be publically available so this is a temporary solution. Right now this is needed because of methods in PlayerSpellManager.
    public Image CastBar
    {
        get { return castBar; }
    }

    public Image CooldownBar
    {
        get { return cooldownBar; }
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();
        spellMngr.AddOnStartCastingListener(OnStartCasting);
        spellMngr.AddOnStopCastingListener(OnStopCasting);
        spellMngr.AddOnChangeSpellListener(OnChangeSelectedSpell);
        spellMngr.AddOnFinishSpellCastListener(ActivateCooldownBar);

        cooldownBar.rectTransform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    public void UpdateHUDHealthBar()
    {
        // Set health bar fill amount.
        if (!combatStats.IsAlive)
            playerHealthBarMask.fillAmount = 0;
        else
            playerHealthBarMask.fillAmount = combatStats.GetCurrentHealthInPercent();

        if (playerHealthBarMask.fillAmount < 0.985f)
            playerHealthBarSurfaceMask.fillAmount = 1 - playerHealthBarMask.fillAmount + 0.015f;
        else
            playerHealthBarSurfaceMask.fillAmount = 1 - playerHealthBarMask.fillAmount;
    }

    public void UpdateHUDShieldBar()
    {
        // Set energy shield bar fill amount
        if (!combatStats.IsAlive)
            playerEnergyShieldBarMask.fillAmount = 0;
        else
            playerEnergyShieldBarMask.fillAmount = combatStats.GetCurrentShieldInPercent();
    }

    public void UpdateHUDManaBar()
    {
        // Set mana bar fill amount.
        if (!combatStats.IsAlive)
            playerManaBarMask.fillAmount = 0;
        else
            playerManaBarMask.fillAmount = combatStats.GetCurrentManaInPercent();

        if (playerController.IsCasting)
        {
            playerManaBarSurfaceMask.fillAmount = 1 - combatStats.GetCurrentManaInPercent() + combatStats.GetValueInManaPercent(spellMngr.CurrentSelectedSpell.ManaCost);
        }
        else
        {
            if (playerManaBarMask.fillAmount < 0.985f)
                playerManaBarSurfaceMask.fillAmount = 1 - combatStats.GetCurrentManaInPercent() + 0.015f;
            else
                playerManaBarSurfaceMask.fillAmount = 1 - combatStats.GetCurrentManaInPercent();
        }
    }

    protected override void UpdateHealthBar()
    {
        base.UpdateHealthBar();
        UpdateHUDHealthBar();
    }

    protected override void UpdateManaBar()
    {
        base.UpdateManaBar();
        UpdateHUDManaBar();
    }

    protected override void UpdateShieldBar()
    {
        base.UpdateShieldBar();
        UpdateHUDShieldBar();
    }

    private void OnChangeSelectedSpell(Spell selectedSpell)
    {
        ActivateCooldownBar(selectedSpell);
    }

    private void OnStartCasting()
    {
        // These are here to make sure that the mana cost is shown on the globes ui when casting spells.
        UpdateHUDManaBar();
        // Show the cast bar.
        castBar.rectTransform.parent.gameObject.SetActive(true);
    }

    private void OnStopCasting()
    {
        UpdateHUDManaBar();
        castBar.rectTransform.parent.gameObject.SetActive(false);
    }

    private void ActivateCooldownBar(Spell selectedSpell)
    {
        if (selectedSpell.cooldownTime > COOLDOWN_BAR_THRESHOLD)
            cooldownBar.rectTransform.parent.gameObject.SetActive(selectedSpell.IsOnCooldown);
    }
}
