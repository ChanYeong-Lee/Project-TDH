using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    public AnimationClip clip;

    [Header("¼³Á¤")]
    public float moveSpeed;
    public float moveSpeedIncrease;
    public float applyMoveSpeed => Mathf.Clamp(moveSpeed * moveSpeedIncrease, 0.0f, Mathf.Infinity);

    public float angularSpeed;

    private float moreSpeed;
    private float blendSpeed;

    private int hashMoveSpeed = Animator.StringToHash("MoveSpeed");
    private int hashMoreSpeed = Animator.StringToHash("MoreSpeed");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        moveSpeedIncrease = 1.0f;
        blendSpeed = 0.0f;
    }

    private void Update()
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        blendSpeed = Mathf.Lerp(blendSpeed, 1.0f, 10.0f * Time.deltaTime);

        if (agent.hasPath)
        {
            moreSpeed = applyMoveSpeed / (clip.averageSpeed.z * transform.localScale.z);
        }
        else
        {
            moreSpeed = 1.0f;
        }

        animator.SetFloat(hashMoveSpeed, blendSpeed);
        animator.SetFloat(hashMoreSpeed, moreSpeed);
    }

    public void Move(Vector3 direction)
    {
        agent.SetDestination(transform.position + direction);
    }

    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), angularSpeed * moveSpeedIncrease * Time.deltaTime);
        }
    }
}
