using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    public AnimationClip clip;

    [Header("설정")]
    public float moveSpeed;
    public float moveSpeedIncrease;
    public float angularSpeed;

    [Header("상태")]
    public Vector3 position;
    public Vector3 velocity;

    private float moreSpeed;
    private float blendSpeed;
    private float checkTime;
    private float checkDelay = 0.5f;

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
        CheckVelocity();

        Vector3 direction = (agent.destination - transform.position).normalized;

        blendSpeed = Mathf.Lerp(blendSpeed, 1.0f, 10.0f * Time.deltaTime);

        if (agent.hasPath)
        {
            moreSpeed = (moveSpeed * moveSpeedIncrease) / (clip.averageSpeed.z * transform.localScale.z);
        }
        else
        {
            moreSpeed = 1.0f;
        }

        animator.SetFloat("MoveSpeed", blendSpeed);
        animator.SetFloat("MoreSpeed", moreSpeed);
    }

    private void CheckVelocity()
    {
        checkTime -= Time.deltaTime;
        if (checkTime < 0.0f)
        {
            velocity = (transform.position - position) / (checkDelay - checkTime);
            position = transform.position;
            checkTime = checkDelay;
        }
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
