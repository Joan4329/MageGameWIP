using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MovementController {

    //private PlayerSpellManager playerSpellManager;

    private Vector3 previousLookRotation = new Vector3();
    private Quaternion prevMouseLookRotation;


    public Quaternion PrevMouseLookRotation
    {
        get { return prevMouseLookRotation; }
        //set { prevMouseLookRotation = value; }
    }

    public PlayerStats PlayerStats
    {
        get { return combatStats as PlayerStats; }
    }

    protected override void Awake()
    {
        base.Awake();
        
        //playerSpellManager = GetComponent<PlayerSpellManager>();
        //PlayerStats = GetComponent<PlayerStats>();
    }


    // Use this for initialization
    protected override void Start () {
        base.Start();
        prevMouseLookRotation = transform.rotation;
        characterAnimator.SetBool("IsAlive", true);

        
    }
	

	// Update is called once per frame
	protected override void Update () {
        base.Update();
        // Change this later. Each player should take one input source. E.g. player1 will play mouse and keyboard, player2 will play xbox controller.

        if (PlayerStats.IsAlive)
        {
            // Different behavour while casting and not casting. No movement while casting and character rotates towards mouse.
            if (IsCasting)
            {
                // maybe move character slightly if player was running before so that he doesn't stop abruptly.

                // Create a plane at players position. Get raycast from mouse position on screen. Check for hit against created plane.
                Plane mousePlane = new Plane(Vector3.up, transform.position);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                float hitDistance;

                if (mousePlane.Raycast(ray, out hitDistance))
                {
                    // Look at the direction that the mouse is pointing
                    Quaternion mouseLookRotation = Quaternion.LookRotation(ray.GetPoint(hitDistance) - transform.position);
                    mouseLookRotation.x = 0;
                    mouseLookRotation.z = 0;
                    transform.rotation = Quaternion.Slerp(transform.rotation, mouseLookRotation, rotationSmoothness * Time.deltaTime);
                    // This will keep the current rotation after casting is completed, because no new rotation is applied when stationary and previousLookRotation is vector3.zero.
                    previousLookRotation = Vector3.zero;
                    // This is used when casting spells to determine direction of cast.
                    prevMouseLookRotation = mouseLookRotation;
                }
            }
            else
            {
                // Also fix so that diagonal movement is not faster than normal movement.
                Vector3 moveDir = new Vector3();

                // Get movement input
                moveDir.z = Input.GetAxis("Vertical");
                moveDir.x = Input.GetAxis("Horizontal");

                // Set movement in animator
                // Later; change so it animates slower if the player moves slowely, probably also base speed param on moveSpeed * movespeedmodifier * deltatime like below, so that if a player is slowed by an effect he may start a walking animation instead.
                characterAnimator.SetFloat("Speed", Mathf.Abs(moveDir.z));
                if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.z))
                    characterAnimator.SetFloat("Speed", Mathf.Abs(moveDir.x));

                // if not moving look in previous direction otherwise look in direction of movement.
                if (moveDir == Vector3.zero)
                {
                    if (previousLookRotation != Vector3.zero)
                        this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(previousLookRotation), rotationSmoothness * Time.deltaTime); ;
                }
                else
                {
                    this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), rotationSmoothness * Time.deltaTime); ;
                    previousLookRotation = moveDir;
                }

                // Move character.
                // As mentioned above, fix abrupt stopping and starting. May be fixed by adding velocity that gets added to while moving. Velocity should then have a ceiling. This may cause sluggish movement so it needs to have a heavy "friction" and reach top speed quickly.
                if (moveDir != Vector3.zero)
                {
                    moveDir = moveDir.normalized;
                    moveDir *= movementSpeed.Value * Time.deltaTime;
                    this.transform.Translate(moveDir, Space.World);
                }
            }
        }
        else
        {
            
        }

        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    protected override void DeathActions()
    {
        base.DeathActions();

        GetComponent<NavMeshAgent>().enabled = false;
    }
}
