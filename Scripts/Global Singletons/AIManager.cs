using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIManager : MonoBehaviour {
    // Used in a lot of tasks to determine how often a task checks conditions etc.
    public const float TASK_UPDATE_RATE = 0.5f;

    // Might just implement this as a fully singleton class.
    private static AIManager instance;

    private List<GameObject> aiEntities = new List<GameObject>();
    private float activationDistance = 50f;

    // Needs to change when adding more players. Maybe to camera instead unless player gameobject is needed for other things.
    [SerializeField] private GameObject player;

    public static AIManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null)
            Debug.Log("Looks like there's more than one AIManager, the static instance variable grabs the latest manager created.");

        instance = this;
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(ChangeEnabledStateByDistance());
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void EnableBehaviours(GameObject o, bool enable)
    {
        // I think this needs to be made more efficient since each GetComponent iterates through all components until the right one is found.
        // Can probably be fixed by moving this into AIController (if it's possible to run a method in an inactive script that can reactivate itself...) and storing each of these components in a variable that loads on Start()

        // Interesting behaviour I need to look into; if I fire a spell at the troll creature when it is inactive it stacks all damage and effects and runs them after code is enabled again. This might cause issues even with a far activationDistance.

        //o.SetActive(enable);
        AIController aiC = o.GetComponent<AIController>();
        aiC.enabled = enable;
        o.GetComponent<NavMeshAgent>().enabled = enable;
        o.GetComponent<CombatStats>().enabled = enable;
        o.GetComponent<CapsuleCollider>().enabled = enable;
        o.GetComponentInChildren<Animator>().enabled = enable;
        if (o.GetComponentInChildren<Canvas>() != null)
            o.GetComponentInChildren<Canvas>().enabled = enable;

        if (enable)
            o.GetComponent<TaskManager>().RestartCurrentTask();
        else
            o.GetComponent<TaskManager>().KillAllTasks();
            
    }


    public void AddAIEntityToList(GameObject o)
    {
        if (o.GetComponent<AIController>() != null)
            aiEntities.Add(o);
    }

    private IEnumerator ChangeEnabledStateByDistance()
    {
        // Maybe add a way to turn this off? (And be able to restart it later)
        while (true)
        {    

            // Create a lambda that selects all alive units and maybe even checks their distance and only picks those who's behaviour should change.
            foreach (GameObject o in aiEntities)
            {
                if (o.GetComponent<CombatStats>().IsAlive)
                {
                    float distanceFromPlayer = Vector3.Distance(o.transform.position, player.transform.position);
                    AIController aiController = o.GetComponent<AIController>();

                    if (distanceFromPlayer < activationDistance)
                    {
                        if (aiController.PreviousDistanceFromPlayer > activationDistance)
                        {
                            EnableBehaviours(o, true);
                        }
                    }
                    else
                    {
                        if (aiController.PreviousDistanceFromPlayer < activationDistance)
                        {
                            EnableBehaviours(o, false);
                        }
                    }
                    aiController.PreviousDistanceFromPlayer = distanceFromPlayer;
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }   
}
