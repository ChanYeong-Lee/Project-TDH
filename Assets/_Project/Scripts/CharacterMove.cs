using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMove : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    public AnimationClip clip;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    public float speed;
    float moreSpeed;
    private float checkTime;
    private Vector3 pos;
    float count;
    private void Update()
    {

        count += Time.deltaTime;
        if (checkTime + 2.0f < Time.time)
        {
            checkTime = Time.time;
            print(((transform.position - pos)/count).magnitude);
            pos = transform.position;
            count = 0.0f;
        }
        Vector3 direction = (agent.destination - transform.position).normalized;
        float forwardValue = Vector3.Dot(transform.forward, direction);

        if (agent.hasPath)
        {
            moreSpeed = speed / clip.averageSpeed.z;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 10.0f * Time.deltaTime);
        }
        else
        {
            moreSpeed = 1.0f;
        }

        animator.SetFloat("MoveSpeed", forwardValue, 0.1f, Time.deltaTime);
        animator.SetFloat("MoreSpeed", moreSpeed);
    }
}
