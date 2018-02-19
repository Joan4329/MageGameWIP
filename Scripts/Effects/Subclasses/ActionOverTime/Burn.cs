using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : ActionOverTimeEffect {
    // Might need to make sure that for example a 4 second duration Burn always gets of 4 ticks of damage within that timeframe.

    private Damage damage;

    public Burn(float duration, Damage damage) : base(duration)
    {
        this.damage = damage;
    }

    public override void OnApplied(GameObject affectedTarget)
    {
        // Perform a callback or invoke event that adds particle effects if there isn't already one and extends duration if there is one of the same type.
    }

    public override void OnExit(GameObject affectedTarget)
    {
        // Perform a callback or invoke event that removes particle effects if there isn't another effect of the same type on affectedTarget.
    }

    public override void Process(GameObject affectedTarget)
    {
        affectedTarget.GetComponent<CombatStats>().ApplyDamage(damage);
    }
}
