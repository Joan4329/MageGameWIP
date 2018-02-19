using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    //private Camera playerCamera;
    public GameObject player;

    public float followSmoothness = 4f;

    private void Awake()
    {
        //playerCamera = this.GetComponent<Camera>();
        if (player == null)
            throw new System.Exception("player GameObject not set in PlayerCamera.");
    }

    // Use this for initialization
    void Start () {

        // Initial camera position.
        this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 8, player.transform.position.z - 8);
        this.transform.LookAt(player.transform);
        
    }
	
	// Update is called once per frame
	void Update () {
        // Later this should be centered around the gameplay area and move when player moves and also take into account that there may be two local players on screen.
        
        // Move camera after player
        Vector3 destination = new Vector3(player.transform.position.x, player.transform.position.y + 8, player.transform.position.z - 8) - this.transform.position;
        this.transform.Translate(destination * followSmoothness * Time.deltaTime, Space.World);
    }
}
