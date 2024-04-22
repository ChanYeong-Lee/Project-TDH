using UnityEngine;
using UnityEngine.AI;

public class CharacterMove : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    public AnimationClip clip;
    
    [Header("설정")]
    public float moveSpeed;
    public float angularSpeed;

    [Header("상태")]
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
        // TODO : 다른 유저의 캐릭터를 건드릴 수 없도록, 
        // 캐릭터가 내 소유일 때는,
        // agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        // 그렇지 않을 때는
        // agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        // priority를 바꾸는 것도 생각해볼만함.
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