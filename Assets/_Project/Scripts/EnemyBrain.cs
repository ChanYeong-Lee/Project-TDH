using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    public EnemyModel model;
    public float stopDistance;

    private Path currentPath;

    private void Awake()
    {
        // 원래라면 hiearachy에서 model 하위에 속하여 model을 다른 방식으로 참조하여 역할을 나눴겠지만 일단 이렇게 진행
        model = GetComponent<EnemyModel>();
    }

    private void OnEnable()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            this.enabled = false;
        }

        currentPath = EnemyManager.Instance.enemyPaths[0];
        Vector3 direction = currentPath.endPos.position - transform.position;
            model.move.Move(direction);
    }

    private void Update()
    {
        Vector3 direction = currentPath.endPos.position - transform.position;

        // 흠.. 매 프레임마다 agent.setdestination을 하면 메모리를 많이 쓰는데,, 
        // 근데 회전각을 내가 컨트롤해서 혹시 setdestination을 한번만 했을 때 영원히 목적지에 도달을 못할 수도 있어서,,

        model.move.Rotate(direction);

        if (direction.magnitude < stopDistance)
        {
            currentPath = currentPath.nextPath;
            Vector3 nextDirection = currentPath.endPos.position - transform.position;
            model.move.Move(nextDirection);
        }
    }
}
