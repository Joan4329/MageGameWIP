using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public abstract class Task {

    public delegate bool TaskBehaviour(NewTaskCallback onTaskChangeCallback);
    public delegate void NewTaskCallback(Task currentTask, Task newTask, params object[] paramArr);

    private TaskBehaviour taskBehaviour;
    // This is needed at least until I change coroutines to threads. This is because I can't use StopAllCoroutines() in TaskManager when changing task, because it is delayed and so also kills the new task.
    //private bool isRunning = false;

    protected GameObject owner;
    // Create a better event based system with callbacks for use later. Especially Animator and NavMeshAgent.
    protected AIController aiController;
    protected CombatStats combatStats;
    protected TaskManager taskMngr;
    protected Animator characterAnimator;
    protected NavMeshAgent navAgent;

    public Task(GameObject owner)
    {
        this.owner = owner;
        aiController = owner.GetComponent<AIController>();
        combatStats = owner.GetComponent<CombatStats>();
        taskMngr = owner.GetComponent<TaskManager>();
        characterAnimator = owner.GetComponent<Animator>();
        navAgent = owner.GetComponent<NavMeshAgent>();
        taskBehaviour = ExecuteBehaviour;
    }

    // I used to have a Run method in Task but since Task is no longer a monoBehaviour the coroutine is now started on TaskManager, so Process is public now. I might remove coroutines and create a thread instead, it would look better to have Process be private and Task have a public Run method instead.
    public IEnumerator Process(NewTaskCallback onTaskChangeCallback, params object[] paramArr)
    {
        //isRunning = false;
        OnStart(paramArr);
        bool running = true;

        while (running && combatStats.IsAlive)
        {
            running = taskBehaviour(onTaskChangeCallback);
            yield return new WaitForSeconds(AIManager.TASK_UPDATE_RATE);
        }

        // Need some check to see if it should run this. Maybe some behaviours in this method should not run when a task is killed but only when it is finished.
        OnEnd();
    }

    //public void Kill()
    //{
    //    StopAllCoroutines();
    //}

    public void ChangeBehaviour(TaskBehaviour newBehaviour)
    {
        taskBehaviour = newBehaviour;
    }

    // Maybe there's no need for this?
    public void AddBehaviour(TaskBehaviour behaviour)
    {
        taskBehaviour += behaviour;
    }

    // Maybe there's no need for this?
    public void RemoveBehaviour(TaskBehaviour behaviour)
    {
        taskBehaviour -= behaviour;
    }

    // returning true means keep running the task, returning false means to stop the task.
    protected abstract bool ExecuteBehaviour(NewTaskCallback onTaskChangeCallback);
    // I'm not convinced that this is the best way to pass parameters to the Start() function (so that individual tasks can use external information when starting, see InvestigatePosition as an example).
    // The problem is that each Task, such as InvestigatePosition can't tell the user what parameters it needs. InvestigatePosition requires a bool and a Vector3 to function, so how do we tell the user that?
    // Maybe through throw new exception message?
    protected abstract void OnStart(params object[] paramArr);
    protected abstract void OnEnd();
}
