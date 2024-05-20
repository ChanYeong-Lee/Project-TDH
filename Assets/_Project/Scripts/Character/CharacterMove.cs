using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class CharacterMove : MonoBehaviourPun
{
    private Animator animator;
    private NavMeshAgent agent;

    [Header("설정")]
    public float angularSpeed;

    [Header("상태")]
    public AnimationClip walkingClip;

    public float moveSpeed;
    public float moveSpeedIncrease;
    public float applyMoveSpeed => Mathf.Clamp(moveSpeed * moveSpeedIncrease, 0.0f, Mathf.Infinity);

    public bool tryMove;
    public bool isMoving;
    
    public Vector3 position;
    public Vector3 velocity;

    public Quaternion rotation;
    public float angularVelocity;

    private float moreSpeed;
    private float blendSpeed;

    private int hashMoveSpeed = Animator.StringToHash("MoveSpeed");
    private int hashMoreSpeed = Animator.StringToHash("MoreSpeed");
    private int hashTryMove = Animator.StringToHash("TryMove");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        blendSpeed = 0.0f;
        // TODO : 다른 유저의 캐릭터를 건드릴 수 없도록, 
        // 캐릭터가 내 소유일 때는,
        // agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
        // 그렇지 않을 때는
        // agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        // priority를 바꾸는 것도 생각해볼만함.
    }

    private void Update()
    {
        if (agent.hasPath)
        {
            blendSpeed = Mathf.Lerp(blendSpeed, 1.0f, 10.0f * Time.deltaTime);
            moreSpeed = applyMoveSpeed / walkingClip.averageSpeed.z;
        }
        else
        {
            blendSpeed = Mathf.Lerp(blendSpeed, 0.0f, 20.0f * Time.deltaTime);
            moreSpeed = 1.0f;
        }

        animator.SetFloat(hashMoveSpeed, blendSpeed);
        animator.SetFloat(hashMoreSpeed, moreSpeed);
        animator.SetBool(hashTryMove, tryMove);
    }

    public void SetMoveStats(CharacterSO defaultStat)
    {
        this.walkingClip = defaultStat.walkingClip;
        this.moveSpeed = defaultStat.defaultMoveSpeed;

        moveSpeedIncrease = 1.0f;
    }

    public void AddCrystal(int color)
    {
        //if (color == 1)
        //{
        //    moveSpeed += moveSpeed * 0.1f;
        //}
    }

    public void Move(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            tryMove = true;
            agent.SetDestination(transform.position + direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), angularSpeed * moveSpeedIncrease * Time.deltaTime);
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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), angularSpeed * moveSpeedIncrease * Time.deltaTime);
        }
    }
}