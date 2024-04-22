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
        // ������� hiearachy���� model ������ ���Ͽ� model�� �ٸ� ������� �����Ͽ� ������ ���������� �ϴ� �̷��� ����
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

        // ��.. �� �����Ӹ��� agent.setdestination�� �ϸ� �޸𸮸� ���� ���µ�,, 
        // �ٵ� ȸ������ ���� ��Ʈ���ؼ� Ȥ�� setdestination�� �ѹ��� ���� �� ������ �������� ������ ���� ���� �־,,

        model.move.Rotate(direction);

        if (direction.magnitude < stopDistance)
        {
            currentPath = currentPath.nextPath;
            Vector3 nextDirection = currentPath.endPos.position - transform.position;
            model.move.Move(nextDirection);
        }
    }
}
