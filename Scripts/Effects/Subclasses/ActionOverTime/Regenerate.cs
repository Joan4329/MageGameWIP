using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regenerate : ActionOverTimeEffect
{
    private Damage heal;
    
    public Regenerate(float duration, Damage heal) : base(duration)
    {
        this.heal = heal;
    }

    public override void OnApplied(GameObject affectedTarget)
    {
        
    }

    public override void OnExit(GameObject affectedTarget)
    {
        
    }

    public override void Process(GameObject affectedTarget)
    {
        affectedTarget.GetComponent<CombatStats>().ApplyHeal(heal);
    }
}
