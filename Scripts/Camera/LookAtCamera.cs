using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {
    // Used for GUI elements that faces the camera, like floating health bars.

    public bool lockXRotation = false;
    public bool lockYRotation = true;
    public bool lockZRotation = true;
    
	
	// Update is called once per frame
	void LateUpdate () {
        Quaternion newFacing = Quaternion.LookRotation(this.transform.position - Camera.main.transform.position);
        if (lockXRotation)
            newFacing.x = 0;
        if (lockYRotation)
            newFacing.y = 0;
        if (lockZRotation)
            newFacing.z = 0;
        transform.rotation = newFacing;
        
	}
}
