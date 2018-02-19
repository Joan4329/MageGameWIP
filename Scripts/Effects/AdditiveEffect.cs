using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AdditiveEffect : Effect
{
    // Right now this is nigh identical to ModifierEffect but can take negative magnitude.

    private float magnitude;

    public float Magnitude
    {
        get { return magnitude; }
        //set { magnitude = value; }
    }

    public AdditiveEffect(float duration, float magnitude) : base(duration)
    {
        this.magnitude = magnitude;
    }
}
