using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigatePosition : Task
{
    private bool hasTakenDamage = false;
    private Vector3 position;

    public InvestigatePosition(GameObject owner) : base(owner)
    {
    }

    protected override bool ExecuteBehaviour(NewTaskCallback onTaskChangeCallback)
    {
        if (!aiController.SearchForTarget())
        {
            if (Vector3.Distance(owner.transform.position, navAgent.destination) < 1f)
            {
                onTaskChangeCallback.Invoke(this, taskMngr.GetTaskOfType<LookForTarget>());
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
        ArgumentException newExcep = new ArgumentException(this.GetType() + " tasks require the following arguments: bool hasTakenDamage, Vector3 position");
        //if (!list.Contain type bool and type vector3)
        if (paramArr == null || paramArr.Length < 2)
            throw newExcep;
        // This is just ugly... Find a better way to deal with these parameters. Create a OnStart<T, U>() where T : bool, where U : Vector3 ??? But I'd like to have a variable amount of parameters. Create a TaskParameter Class?
        bool foundBool = false;
        bool foundVector3 = false;
        for (int i = 0; i < paramArr.Length; i++)
        {
            if (paramArr[i].GetType() == typeof(bool))
            {
                hasTakenDamage = (bool)paramArr[i];
                foundBool = true;
            }
            else if (paramArr[i].GetType() == typeof(Vector3))
            {
                position = (Vector3)paramArr[i];
                foundVector3 = true;
            }
        }
        if (!foundBool || !foundVector3)
            throw newExcep;
            
        


        if (hasTakenDamage)
            aiController.AlertLevel = AIController.AlertState.Combat;
        else
            aiController.AlertLevel = AIController.AlertState.Aware;

        navAgent.destination = position;
    }
}
