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
    public bool tryMoving;
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

        if (direction != Vector3.zero)
        {
            blendSpeed = Mathf.Lerp(blendSpeed, 1.0f, 10.0f * Time.deltaTime);
        }
        else
        {
            blendSpeed = Mathf.Lerp(blendSpeed, 0.0f, 20.0f * Time.deltaTime);
        }

        if (agent.hasPath)
        {
            moreSpeed = moveSpeed / clip.averageSpeed.z;
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
        velocity = (transform.position - position) / Time.deltaTime;
        position = transform.position;
     
        angularVelocity = Quaternion.Angle(rotation, transform.rotation);
        rotation = transform.rotation;

        isMoving = velocity != Vector3.zero || angularVelocity != 0.0f;

        tryMoving = animator.GetFloat("MoveSpeed") > 0.1f || animator.GetBool("Rotate");
    }

    public void Move(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            agent.SetDestination(transform.position + direction);
        }
        else
        {
            agent.ResetPath();
        }
    }

    public void Rotate(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), angularSpeed * Time.deltaTime);
            animator.SetBool("Rotate", true);
        }
        else
        {
            animator.SetBool("Rotate", false);
        }
    }

    public void RotateWithoutNotify(Vector3 direction)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), angularSpeed * Time.deltaTime);
    }
}