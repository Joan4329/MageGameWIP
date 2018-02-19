using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToOrigin : Task
{
    public ReturnToOrigin(GameObject owner) : base(owner)
    {
    }

    protected override bool ExecuteBehaviour(NewTaskCallback onTaskChangeCallback)
    {
        if (!aiController.SearchForTarget())
        {
            if (Vector3.Distance(owner.transform.position, navAgent.destination) < 1f)
            {
                onTaskChangeCallback.Invoke(this, taskMngr.GetTaskOfType<Guard>());
                return false;
            }
        }
        else
        {
            taskMngr.DetermineAttackTask(this);
            return false;
        }
        return true;
    }

    protected override void OnEnd()
    {
        // Do nothing
    }

    protected override void OnStart(params object[] paramArr)
    {
        aiController.AlertLevel = AIController.AlertState.Calm;
        navAgent.destination = aiController.OriginPosition;
    }
}
