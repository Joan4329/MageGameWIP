using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForTarget : Task
{
    private float searchTime = 20f;
    private float searchTimer = 0f;

    public LookForTarget(GameObject owner, float searchTime) : base(owner)
    {
        this.searchTime = searchTime;
    }

    protected override bool ExecuteBehaviour(NewTaskCallback onTaskChangeCallback)
    {
        // Look around for target for a bit by walking to random points within the AI entity's search radius.

        bool foundTarget = false;

        if (searchTimer >= searchTime)
        {
            searchTimer = 0f;
            // Set to return to start position.
            onTaskChangeCallback.Invoke(this, taskMngr.GetTaskOfType<ReturnToOrigin>());
            return true;

        }
        else
        {
            foundTarget = aiController.SearchForTarget();

            if (!foundTarget)
            {
                if (Vector3.Distance(owner.transform.position, navAgent.destination) > 1f)
                {
                    // Do nothing?
                    //Debug.Log("Distance more than 1f");
                }
                else
                {
                    //Debug.Log("Trying to find new destination...");
                    Vector3 newDestination;
                    if (aiController.GetRandomVectorInSearchRadius(out newDestination))
                    {
                        //Debug.Log("Found new destination! At vector: " + newDestination);
                        navAgent.destination = newDestination;
                    }
                }
            }
        }
        searchTimer += AIManager.TASK_UPDATE_RATE;
        //Debug.Log("Running: " + GetType() + ", on " + owner.name + ", foundTarget " + foundTarget + " , at " + Time.time + ", SearchTimer at " + searchTimer);
        if (foundTarget)
            taskMngr.DetermineAttackTask(this);
        return !foundTarget;
    }

    protected override void OnEnd()
    {
        // Do nothing
    }

    protected override void OnStart(params object[] paramArr)
    {
        aiController.AlertLevel = AIController.AlertState.Aware;
    }
}
