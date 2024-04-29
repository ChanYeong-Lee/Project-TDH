using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForwardObject : MonoBehaviour
{
    private void Update()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0.0f;
        cameraForward.Normalize();

        transform.up = -cameraForward;
    }
}