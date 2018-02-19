using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect {
    public const float PERMANENT_DURATION = 0f;

    private float duration = 0f;
    protected GameObject effectParticles;

    public float Duration
    {
        get { return duration; }
        set
        {
            if (value < 0f)
                duration = PERMANENT_DURATION;
            else
                duration = value;
        }
    }

    public Effect(float duration)
    {
        this.duration = duration;
    }

    // Use these to apply particle effects and other stuff that should be processed when the effect starts and when it is removed.
    public abstract void OnApplied(GameObject affectedTarget);
    public abstract void OnExit(GameObject affectedTarget);
}


// The values in this enum should be determined at run-time through reflection way way later using a string in effect class for the name.
// Basically, load enum values dynamically from the effect subclass string names.
// This is used to check stacking effects and so on. E.g. when a Burn is applied it can check if it should apply a burning particle effect or just extend duration on the current particle system.
//public enum EffectType
//{
//    Burn,
//    EnergyShieldBonus,
//    ManaRegenModifier,
//    MoveModifier,
//    Regenerate
//}