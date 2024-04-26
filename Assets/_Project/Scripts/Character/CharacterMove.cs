using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMove : MonoBehaviourPun
{
    private Animator animator;
    private NavMeshAgent agent;

    public AnimationClip clip;
    
    [Header("����")]
    public float moveSpeed;
    public float moveIncrease;
    public float angularSpeed;

    [Header("����")]
    public bool tryMove;
    public bool isMoving;
    
    public Vector3 position;
    public Vector3 velocity;

    public Quaternion rotation;
    public float angularVelocity;

    private float moreSpeed;
    private float blendSpeed;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        moveIncrease = 1.0f;
        blendSpeed = 0.0f;
        agent.SetDestination(Vector3.right);
        // TODO : �ٸ� ������ ĳ���͸� �ǵ帱 �� ������, 
        // ĳ���Ͱ� �� ������ ����,
        // agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        // �׷��� ���� ����
        // agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        // priority�� �ٲٴ� �͵� �����غ�����.
    }

    private void Update()
    {
        CheckVelocity();

        float applyMoveSpeed = moveSpeed * moveIncrease;

        if (agent.hasPath)
        {
            blendSpeed = Mathf.Lerp(blendSpeed, 1.0f, 10.0f * Time.deltaTime);
            moreSpeed = applyMoveSpeed / clip.averageSpeed.z;
        }
        else
        {
            blendSpeed = Mathf.Lerp(blendSpeed, 0.0f, 20.0f * Time.deltaTime);
            moreSpeed = 1.0f;
        }

        animator.SetFloat("MoveSpeed", blendSpeed);
        animator.SetFloat("MoreSpeed", moreSpeed);
        animator.SetBool("TryMove", tryMove);
    }

    private void CheckVelocity()
    {
        velocity = (transform.position - position) / Time.deltaTime;
        position = transform.position;
     
        angularVelocity = Quaternion.Angle(rotation, transform.rotation);
        rotation = transform.rotation;

        isMoving = velocity != Vector3.zero || angularVelocity != 0.0f;
    }

    public void Move(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            tryMove = true;
            agent.SetDestination(transform.position + direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), angularSpeed * moveIncrease * Time.deltaTime);
        }
        else
        {
            tryMove = false;
            agent.ResetPath();
        }
    }

    public void Rotate(Vector3 direction)
    {
        direction.y = 0.0f;
        direction.Normalize();

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), angularSpeed * moveIncrease * Time.deltaTime);
        }
    }
}