using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAdditive : AdditiveEffect
{
    public MoveAdditive(float duration, float magnitude) : base(duration, magnitude)
    {
    }

    public override void OnApplied(GameObject affectedTarget)
    {
        // Recalculate movement speed modifier. Need a better way to call the update method
        affectedTarget.GetComponent<MovementController>().MovementSpeed.Update();

        // Perform a callback or invoke event that adds particle effects if there isn't already one and extends duration if there is one of the same type.
    }


    public override void OnExit(GameObject affectedTarget)
    {
        // Recalculate movement speed modifier. Need a better way to call the update method
        affectedTarget.GetComponent<MovementController>().MovementSpeed.Update();

        // Perform a callback or invoke event that removes particle effects if there isn't another effect of the same type on affectedTarget.
    }
}
