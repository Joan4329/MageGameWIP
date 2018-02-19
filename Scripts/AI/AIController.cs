using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class AIController : MovementController {

    private NavMeshAgent navAgent;
    private TaskManager taskMngr;

    // Might need to separate combatTarget from the movement destination (i.e. flank target and cast a spell). In that case add the variable below.
    //private Vector3 navAgentTargetDestination;
    private GameObject combatTarget;

    private float previousDistanceFromPlayer = 0f;

    // Details what kind of task the AI is trying to perform.
    private AlertState alertLevel = AlertState.Calm;
    [SerializeField] private float searchRadius = 10f;
    // Move melee distance to CombatStats.
    [SerializeField] private float meleeDistance = 3f;
    private Vector3 originPosition;

    // Maybe need some better system later on. At least a system that auto-detects each of these things.
    [SerializeField] private bool hasMeleeAttack = true;
    [SerializeField] private bool hasRangedAttack = false;
    [SerializeField] private bool hasSpellAttack = false;
    [SerializeField] private bool hasNonCombatSpells = false;

    public float PreviousDistanceFromPlayer
    {
        get { return previousDistanceFromPlayer; }
        set { previousDistanceFromPlayer = value; }
    }

    public float SearchRadius
    {
        get
        {
            switch (alertLevel)
            {
                case AlertState.Aware:
                    return searchRadius * 1.5f;
                case AlertState.Combat:
                    return searchRadius * 2f;
                case AlertState.Calm:
                default:
                    return searchRadius;
            }
        }    
        //set { searchRadius = value; }
    }

    public AlertState AlertLevel
    {
        get { return alertLevel; }
        set { alertLevel = value; }
    }

    public bool HasMeleeAttack
    {
        get { return hasMeleeAttack; }
    }

    public bool HasRangedAttack
    {
        get { return hasRangedAttack; }
    }

    public bool HasSpellAttack
    {
        get { return hasSpellAttack; }
    }

    public bool HasNonCombatSpells
    {
        get { return hasNonCombatSpells; }
    }

    public GameObject CombatTarget
    {
        get { return combatTarget; }
        set { combatTarget = value; }
    }

    public float MeleeDistance
    {
        get { return meleeDistance; }
        set { meleeDistance = value; }
    }

    public Vector3 OriginPosition
    {
        get { return originPosition; }
        // There may be reason later to allow the origin position to change. i.e. a Patrol task may be constructed in such a way that it always sets originPosition to allow it to return to the position from which it left its patrol route.
        //set { originPosition = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        navAgent = GetComponent<NavMeshAgent>();
        taskMngr = GetComponent<TaskManager>();
    }

    // Use this for initialization
    protected override void Start () {
        base.Start();

        originPosition = transform.position;
        // Right now, I trust myself to add each and every parameter to each animation controller. That might not be too smart...
        // Instead, later, add some sort of list for parameters that we can check against before setting its state.
        // Ex. if we have an enemy that cannot die, I'd rather not have an IsAlive parameter in its animation controller that is never used... but in all others I want it.
        characterAnimator.SetBool("IsAlive", true);
        AIManager.Instance.AddAIEntityToList(this.gameObject);

        characterAnimator.SetFloat("MeleeAttackSpeed", combatStats.MeleeAttackSpeed);
        taskMngr.StartNewTask<Guard>();
    }
	
	// Update is called once per frame
	protected override void Update ()
    {
        base.Update();
        // Move from update later. Only needs to update whenever the value changes.
        navAgent.speed = movementSpeed.Value;
        characterAnimator.SetFloat("Speed", navAgent.velocity.magnitude);
	}


    //public IEnumerator ReturnToOriginTask()
    //{
    //    //Debug.Log("Returning...");
    //    currentTask = TaskType.ReturnToOrigin;
    //    alertLevel = AlertState.Calm;

    //    navAgent.destination = originPosition;

    //    while (currentTask == TaskType.ReturnToOrigin && combatStats.IsAlive && enabled)
    //    {
    //        if (!SearchForTarget())
    //        {
    //            if (Vector3.Distance(transform.position, navAgent.destination) < 1f)
    //            {
    //                StartCoroutine(GuardTask());
    //            }
    //        }

    //        yield return new WaitForSeconds(AIManager.TASK_UPDATE_RATE);
    //    }
    //}

    //public IEnumerator InvestigatePositionTask(Vector3 point, bool hasTakenDamage)
    //{
    //    currentTask = TaskType.InvestigatePosition;
    //    if (hasTakenDamage)
    //        alertLevel = AlertState.Combat;
    //    else
    //        alertLevel = AlertState.Aware;

    //    navAgent.destination = point;

    //    while (currentTask == TaskType.InvestigatePosition && combatStats.IsAlive && enabled)
    //    {
    //        if (!SearchForTarget())
    //        {
    //            if (Vector3.Distance(transform.position, navAgent.destination) < 1f)
    //            {
    //                StartCoroutine(LookForTargetTask());
    //            }
    //        }

    //        yield return new WaitForSeconds(AIManager.TASK_UPDATE_RATE);
    //    }
    //}

    //public IEnumerator LookForTargetTask()
    //{
    //    //Debug.Log("Looking...");
    //    currentTask = TaskType.LookingForTarget;
    //    alertLevel = AlertState.Aware;

    //    // Look around for target for a bit
    //    while (currentTask == TaskType.LookingForTarget && combatStats.IsAlive && enabled)
    //    {
            
    //        //Debug.Log("SearchTimer: " + searchTimer);

    //        if (searchTimer >= searchTime)
    //        {
    //            searchTimer = 0f;
    //            StartCoroutine(ReturnToOriginTask());
    //            // Set to return to start position.
    //        }
    //        else
    //        {
    //            if (!SearchForTarget())
    //            {
    //                if (Vector3.Distance(transform.position, navAgent.destination) > 1f)
    //                {
    //                    // Do nothing?
    //                    //Debug.Log("Distance more than 1f");
    //                }
    //                else
    //                {
    //                    //Debug.Log("Trying to find new destination...");
    //                    Vector3 newDestination;
    //                    if (GetRandomVectorInSearchRadius(out newDestination))
    //                    {
    //                        //Debug.Log("Found new destination! At vector: " + newDestination);
    //                        navAgent.destination = newDestination;
    //                    }
    //                }
    //            }
    //        }
    //        yield return new WaitForSeconds(AIManager.TASK_UPDATE_RATE);
    //        searchTimer += AIManager.TASK_UPDATE_RATE;
    //    }
        
    //}

    //public IEnumerator GuardTask()
    //{
    //    //Debug.Log("Guarding...");
    //    currentTask = TaskType.Guard;
    //    alertLevel = AlertState.Calm;

    //    // Stand stationary until enemy comes close
    //    while (currentTask == TaskType.Guard && combatStats.IsAlive && enabled)
    //    {
    //        SearchForTarget();
    //        yield return new WaitForSeconds(AIManager.TASK_UPDATE_RATE);
    //    }
    //}

    //public IEnumerator MeleeAttackTask()
    //{
    //    //Debug.Log("Attacking...");
    //    currentTask = TaskType.AttackMelee;
    //    // Chase target and attack in melee
    //    while (currentTask == TaskType.AttackMelee && combatStats.IsAlive && enabled)
    //    {
    //        if (combatTarget != null && combatTarget.GetComponent<CombatStats>().IsAlive)
    //        {
    //            float distance = Vector3.Distance(transform.position, combatTarget.transform.position);

    //            if (distance > SearchRadius)
    //            {
    //                IsAttackingMelee = false;
    //                characterAnimator.SetBool("IsAttackingMelee", IsAttackingMelee);
    //                combatTarget = null;
    //                navAgent.isStopped = false;
    //                StartCoroutine(LookForTargetTask());
    //            }
    //            else if (distance > (meleeDistance - 1f))
    //            {
    //                IsAttackingMelee = false;
    //                characterAnimator.SetBool("IsAttackingMelee", IsAttackingMelee);
    //                if (navAgent.destination != combatTarget.transform.position)
    //                    navAgent.destination = combatTarget.transform.position;
    //                navAgent.isStopped = false;
    //            }
    //            else
    //            {
    //                if (!IsAttackingMelee)
    //                {
    //                    SetMeleeEnabled(true);
    //                    characterAnimator.SetFloat("MeleeAttackSpeed", combatStats.MeleeAttackSpeed);
    //                    StartCoroutine(RotateTowardsTarget());
    //                }
    //            }
    //        }
    //        else
    //        {
    //            combatTarget = null;
    //            SetMeleeEnabled(false);
    //            StartCoroutine(LookForTargetTask());
    //        }

    //        yield return new WaitForSeconds(AIManager.TASK_UPDATE_RATE);
    //    }
    //}

    public void SetMeleeEnabled(bool enabled)
    {
        IsAttackingMelee = enabled;
        characterAnimator.SetBool("IsAttackingMelee", enabled);
        navAgent.isStopped = enabled;
    }

    // Move this to combatStats?
    private void CauseMeleeDamage()
    {
        // This method is called through animation event.

        if (combatTarget != null)
            combatTarget.GetComponent<CombatStats>().ApplyDamage(new Damage(new Damage.DamageInfo(this.gameObject, DamageType.Physical, null, true), null, combatStats.MeleeDamage));
        IsAttackingMelee = false;
    }

    public IEnumerator RotateTowardsTarget()
    {
        while (IsAttackingMelee && combatTarget.GetComponent<CombatStats>().IsAlive && combatStats.IsAlive)
        {        
            Quaternion newLookRotation = Quaternion.LookRotation(combatTarget.transform.position - transform.position);
            newLookRotation.x = 0;
            newLookRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, newLookRotation, rotationSmoothness * Time.deltaTime);
            yield return null;
        }
    }

    public bool GetRandomVectorInSearchRadius(out Vector3 returnVector)
    { 
        for (int i = 0; i < 10; i++)
        {
            Vector3 testPoint = transform.position + (Random.insideUnitSphere * SearchRadius);
            testPoint.y = 0;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(testPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                returnVector = hit.position;
                return true;
            }
        }
        returnVector = Vector3.zero;
        return false;
    }

    public bool SearchForTarget()
    {
        combatTarget = FindTarget(TargetSearchType.Closest);
        return combatTarget != null;
    }

    private GameObject FindTarget(TargetSearchType searchType)
    {
        List<Collider> colliders = new List<Collider>(Physics.OverlapSphere(transform.position, SearchRadius, CombatStats.ATTACKABLE_LAYER))
                                    .FindAll((Collider col) => col.gameObject.GetComponent<MovementController>().Faction != Faction);

        foreach (Collider col in colliders)
            Debug.Log(this.gameObject.name + ": " + col.gameObject.name);

        if (colliders.Count < 1)
            return null;

        switch (searchType)
        {
            case TargetSearchType.First:
                return colliders[0].gameObject;
            case TargetSearchType.Random:
                return colliders[Random.Range(0, colliders.Count - 1)].gameObject;
            case TargetSearchType.Closest:
                GameObject closestTarget = null;
                float closestDistance = Mathf.Infinity;

                foreach (Collider hit in colliders)
                {
                    float distance = Vector3.Distance(transform.position, hit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestTarget = hit.gameObject;
                    }
                }
                return closestTarget;
            case TargetSearchType.FocusPlayer:
                foreach (Collider hit in colliders)
                {
                    if (hit.gameObject.GetComponent<PlayerController>() != null)
                        return hit.gameObject;
                }
                // Return first if player not found.
                return colliders[0].gameObject;
        }
        return null;
    }

    protected override void DeathActions()
    {
        base.DeathActions();
        navAgent.isStopped = true;
        StopAllCoroutines();
    }

    protected override void TakeDamageActions(Damage damage)
    {
        base.TakeDamageActions(damage);
        // play flinching animation?
    }

    public enum AlertState
    {
        Calm,
        Aware,
        Combat
    }

    private enum TargetSearchType
    {
        First,
        Closest,
        Random,
        FocusPlayer
    }
}
