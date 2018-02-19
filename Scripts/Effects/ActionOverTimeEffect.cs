using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionOverTimeEffect : Effect
{
    public ActionOverTimeEffect(float duration) : base(duration)
    {
    }

    public abstract void Process(GameObject affectedTarget);
}
