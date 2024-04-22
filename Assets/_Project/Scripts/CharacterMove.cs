using UnityEngine;
using UnityEngine.AI;

public class CharacterMove : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    public AnimationClip clip;
    
    [Header("����")]
    public float moveSpeed;
    public float angularSpeed;

    [Header("����")]
    public Vector3 position;
    public Vector3 velocity;

    private float moreSpeed;

    private float checkTime;
    private float checkDelay = 0.5f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
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

        Vector3 direction = (agent.destination - transform.position).normalized;
        float forwardValue = Vector3.Dot(transform.forward, direction);

        if (agent.hasPath)
        {
            moreSpeed = moveSpeed / clip.averageSpeed.z;
        }
        else
        {
            moreSpeed = 1.0f;
        }

        animator.SetFloat("MoveSpeed", forwardValue);
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
        if (direction == Vector3.zero)
        {
            agent.ResetPath();
        }
        else
        {
            agent.SetDestination(transform.position + direction);
        }
    }

    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), angularSpeed * Time.deltaTime);
        }
    }
}