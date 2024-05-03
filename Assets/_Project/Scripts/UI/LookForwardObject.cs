using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForwardObject : MonoBehaviour
{
    private void Update()
    {
        transform.up = -Vector3.forward;
    }
}