using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public NavMeshAgent agent;

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector2 move = new Vector2(x, y).normalized;

        if (move != Vector2.zero)
        {
            agent.SetDestination(agent.transform.position + new Vector3(move.x, 0.0f, move.y));
        }
        else
        {
            agent.ResetPath();
        }
    }
}
