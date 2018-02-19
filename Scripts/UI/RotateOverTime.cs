using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour {

    public float rotationSpeed = 10f;
    public bool reverse = false;
    private Vector3 rotationVector = Vector3.forward;

    void Awake()
    {
        if (reverse)
            rotationVector = Vector3.back;
    }

    void Update () {
        GetComponent<RectTransform>().Rotate(rotationVector, rotationSpeed * Time.deltaTime, Space.World);
    }
}
