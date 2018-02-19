using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TaskManager : MonoBehaviour {

    // This list should later be replaced with some kind of aiBehaviourProfile class instance. Such a class will allow the enemies to have distinct patterns of behaviour.
    // One profile could make the ai only ever patrol and attack enemies he finds, while another could be that the ai guards a position and attacks any enemies and then return to its guard post. There are many examples.
    // for some reason this doesn't show up in inspector in unity even though Task is Serializable. It doesn't matter a whole lot though.
    [SerializeField] private List<Task> possibleTasks;
    private AIController aiController;
    private CombatStats combatStats;

    // This needs to be remade to something other than System.Type. Maybe use enum, but I don't really want to add every single Task I create to that enum by hand.
    private System.Type currentTaskType;

    private void Awake()
    {
        aiController = GetComponent<AIController>();
        combatStats = GetComponent<CombatStats>();
    }

    private void Start()
    {
        possibleTasks = new List<Task>() {new Guard(this.gameObject), new InvestigatePosition(this.gameObject), new MeleeAttack(this.gameObject), new ReturnToOrigin(this.gameObject), new LookForTarget(this.gameObject, 20f) };
        combatStats.DamageEvent.AddListener(TakeDamageActions);
        if (possibleTasks.Count < 1)
            throw new System.Exception("A TaskManager does not have any tasks attached");
    }

    public void StartNewTask<T>(params object[] paramArr) where T : Task
    {
        ChangeTask(null, GetTaskOfType<T>(), paramArr);
    }

    public void ChangeTask(Task currentTask, Task newTask, params object[] paramArr)
    {
        //Debug.Log("Changing Task to: " + newTask.GetType() + " on " + this.gameObject.name);
        //if (currentTask != null)
        //{
        //    currentTask.Kill();
        //}
        //else
        KillAllTasks();
        // For now default to Guard task, but a Task to default back to should be set later in AIBehaviourProfile.
        if (newTask == null)
            GetTaskOfType<Guard>();
        currentTaskType = newTask.GetType();
        Task.NewTaskCallback callback = ChangeTask;
        Run(callback, newTask, paramArr);
    }

    public void ChangeTask<T>(Task currentTask, params object[] paramArr) where T : Task
    {
        ChangeTask(currentTask, GetTaskOfType<T>(), paramArr);
    }

    // This used to be in Task. I might create a Threaded job for this later and remove coroutine and move this method back to Task.
    private void Run(Task.NewTaskCallback onTaskChangeCallback, Task newTask, params object[] paramArr)
    {
        StartCoroutine(newTask.Process(onTaskChangeCallback, paramArr));
    }

    // There might be a problem if it is unintended that some other coroutines started from inside Task will stop when KillAllTasks() is called. (See RotateTowardsTarget in MeleeAttack for example)
    public void KillAllTasks()
    {
        //Debug.Log("Killing tasks at: " + Time.time);
        //foreach (Task t in possibleTasks)
        //    t.Kill();
        StopAllCoroutines();
    }

    // This needs to be rewritten to something better (i.e. don't have to add each and every if statement for all Task Types)
    public void RestartCurrentTask()
    {
        if (currentTaskType == typeof(Guard))
            StartNewTask<Guard>();
        else if (currentTaskType == typeof(LookForTarget))
            StartNewTask<LookForTarget>();
        else if (currentTaskType == typeof(MeleeAttack))
            StartNewTask<MeleeAttack>();
        else if (currentTaskType == typeof(ReturnToOrigin))
            StartNewTask<ReturnToOrigin>();
        else if (currentTaskType == typeof(InvestigatePosition))
            StartNewTask<InvestigatePosition>();
        else
            throw new System.Exception("Unhandled Type in TaskManager.RestartCurrentTask()");
    }

    // This method should probably move to inside AIBehaviourProfile once it is in place.
    public void DetermineAttackTask(Task currentTask)
    {
        if (aiController.HasMeleeAttack)
        {
            // This will be rewritten once AIBehaviourProfile is in place. Normally enemies will only have one melee attack task.
            Task newTask = GetTaskOfType<MeleeAttack>();
            if (newTask == null)
                throw new System.NullReferenceException("There is an AI entity that does not have a melee attack task in its list of Tasks. GameObject name: " + gameObject.name);

            ChangeTask<MeleeAttack>(currentTask);
        }
            
        //else if (aiController.HasRangedAttack)
        //    currentTask = TaskType.AttackRanged;
        //else if (aiController.HasSpellAttack)
        //    currentTask = TaskType.AttackSpell;
        //else if (aiController.HasNonCombatSpells)
        //    currentTask = TaskType.UseNonCombatSpell;
        //else
        //{
        //    Debug.Log("AI entity " + gameObject.name + " does not have any type of attack or spells selected for use");
        //    // Do nothing...
        //}
    }

    public T GetTaskOfType<T>() where T : Task
    {
        List<T> tasks = possibleTasks.OfType<T>().ToList();
        if (tasks.Count < 1)
            return null;
        // Return only first element for now. Will change to something better once AIBehaviourProfile is in place. Also need to handle null return somewhere.
        return tasks[0];
    }

    private void TakeDamageActions(Damage damage)
    {
        if (currentTaskType == typeof(Guard) || currentTaskType == typeof(ReturnToOrigin) || currentTaskType == typeof(LookForTarget) || currentTaskType == typeof(InvestigatePosition))
        {
            StartNewTask<InvestigatePosition>(new object[] { damage.Info.DamageSource.transform.position, true });
        }
    }

    public enum TaskType
    {
        Guard,
        LookingForTarget,
        Patrol,
        ReturnToOrigin,
        InvestigatePosition,
        AttackMelee,
        AttackRanged,
        AttackSpell,
        UseNonCombatSpell,
        //BuffAlly,

    }
}
