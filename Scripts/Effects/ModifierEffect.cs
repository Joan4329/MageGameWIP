using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModifierEffect : Effect {

    private float magnitude;

    public float Magnitude
    {
        get { return magnitude; }
        //set { magnitude = value; }
    }

    public ModifierEffect(float duration, float magnitude) : base(duration)
    {
        if (magnitude < 0)
            this.magnitude = 0;
        this.magnitude = magnitude;
    }
}
