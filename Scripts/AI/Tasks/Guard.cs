using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : Task
{
    public Guard(GameObject owner) : base(owner)
    {
    }

    protected override bool ExecuteBehaviour(NewTaskCallback onTaskChangeCallback)
    {
        // Stand stationary until enemy comes close

        bool foundTarget = false;
        foundTarget = aiController.SearchForTarget();
        
        if (foundTarget)
            taskMngr.DetermineAttackTask(this);
        //Debug.Log("Running: " + GetType() + ", on " + owner.name + ", FoundTarget " + foundTarget + " , at " + Time.time);
        // return negated foundTarget so that if it hasn't found a target it keeps iterating.
        return !foundTarget;
    }

    protected override void OnEnd()
    {
        // Do nothing. Might do something later, maybe the monster says "Ayyy, me no like you!" through text or soundbite, giving the player feedback that he has aggroed enemies.
    }

    protected override void OnStart(params object[] paramArr)
    {
        aiController.AlertLevel = AIController.AlertState.Calm;
    }
}
