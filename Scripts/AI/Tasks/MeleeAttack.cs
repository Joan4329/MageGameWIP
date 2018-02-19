using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Task
{
    public MeleeAttack(GameObject owner) : base(owner)
    {
    }

    protected override bool ExecuteBehaviour(NewTaskCallback onTaskChangeCallback)
    {
        // Chase target and attack in melee


        if (aiController.CombatTarget != null && aiController.CombatTarget.GetComponent<CombatStats>().IsAlive)
        {
            float distance = Vector3.Distance(owner.transform.position, aiController.CombatTarget.transform.position);

            if (distance > aiController.SearchRadius)
            {
                aiController.IsAttackingMelee = false;
                // Later, make this event based. All animations should be handled within AIController.
                characterAnimator.SetBool("IsAttackingMelee", aiController.IsAttackingMelee);
                aiController.CombatTarget = null;
                navAgent.isStopped = false;
                //StartCoroutine(LookForTargetTask());
                onTaskChangeCallback.Invoke(this, taskMngr.GetTaskOfType<LookForTarget>());
                return false;
            }
            else if (distance > (aiController.MeleeDistance - 1f))
            {
                aiController.IsAttackingMelee = false;
                characterAnimator.SetBool("IsAttackingMelee", aiController.IsAttackingMelee);
                if (navAgent.destination != aiController.CombatTarget.transform.position)
                    navAgent.destination = aiController.CombatTarget.transform.position;
                navAgent.isStopped = false;
            }
            else
            {
                if (!aiController.IsAttackingMelee)
                {
                    // Set melee enabled should probably also be some kind of event.
                    aiController.SetMeleeEnabled(true);
                    characterAnimator.SetFloat("MeleeAttackSpeed", combatStats.MeleeAttackSpeed);

                    // Need some kind of way to stop this once the the task is completed.
                    taskMngr.StartCoroutine(aiController.RotateTowardsTarget());
                }
            }
        }
        else
        {
            aiController.CombatTarget = null;
            aiController.SetMeleeEnabled(false);
            //StartCoroutine(LookForTargetTask());
            onTaskChangeCallback.Invoke(this, taskMngr.GetTaskOfType<LookForTarget>());
            return false;
        }
        return true;
    }

    protected override void OnEnd()
    {
        // Do nothing.
    }

    protected override void OnStart(params object[] paramArr)
    {
        aiController.AlertLevel = AIController.AlertState.Combat;
    }
}
