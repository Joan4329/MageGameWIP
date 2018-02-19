using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : CombatStats {

    // Moved all GUI to PlayerGUIManager so right now this class is kinda empty but it might be needed later, though probably not.

    //private PlayerSpellManager spellMngr;
    //private PlayerController playerController;

    protected override void Awake()
    {
        base.Awake();
        //spellMngr = GetComponent<PlayerSpellManager>();
        //playerController = GetComponent<PlayerController>();
    }

    protected override void Update()
    {
        base.Update();
    }


    

    public override void ApplyDamage(Damage damage)
    {
        base.ApplyDamage(damage);
        
    }

    public override void ApplyHeal(Damage damage)
    {
        base.ApplyHeal(damage);
        
    }
}
