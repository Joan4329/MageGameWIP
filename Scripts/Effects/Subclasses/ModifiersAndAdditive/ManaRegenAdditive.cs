﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaRegenAdditive : AdditiveEffect
{
    public ManaRegenAdditive(float duration, float magnitude) : base(duration, magnitude)
    {
    }

    public override void OnApplied(GameObject affectedTarget)
    {
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
}
