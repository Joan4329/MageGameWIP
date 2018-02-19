using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRegenModifier : ModifierEffect
{
    public ManaRegenModifier(float duration, float magnitude) : base(duration, magnitude)
    {
    }

    public override void OnApplied(GameObject affectedTarget)
    {
        // Need better way to call mana regen update method, so that I don't need to find CombatStats from here maybe?
        // Recalculate mana regen modifiers and bonuses.
        affectedTarget.GetComponent<CombatStats>().ManaRegen.Update();

        // Perform a callback or invoke event that adds particle effects if there isn't already one and extends duration if there is one of the same type.
    }

    public override void OnExit(GameObject affectedTarget)
    {
        // Recalculate mana regen modifiers and bonuses.
        affectedTarget.GetComponent<CombatStats>().ManaRegen.Update();

        // Perform a callback or invoke event that removes particle effects if there isn't another effect of the same type on affectedTarget.
    }


    //private void CalculateManaRegenModifier(GameObject affectedTarget)
    //{
    //    // This may need to be fixed later by having both derive from a Controller class. Maybe even have PlayerController derive from AIController so that it can start AI behaviours when polymorphed etc.
    //    affectedTarget.GetComponent<CombatStats>().CalculateManaRegenModifier();
    //}
}
