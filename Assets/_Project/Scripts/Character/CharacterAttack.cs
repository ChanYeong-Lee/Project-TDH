using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AttackType
{
    Single, // ������ ��ü �� ��ŭ ����
    Area    // Ÿ�� ���� ������ ������ŭ ����
}

public class CharacterAttack : MonoBehaviour
{
    public AttackType type;

    public float damage;            // ĳ���� �⺻ ���� + ���� ����
    public float damageIncrease;    // �⺻�� = 1.0, ������ ���� ��ȭ

    public float attackSpeed;
    public float attackSpeedIncrease;

    public float attackRange;
    public float attackRangeIncrease;   

    public int targetNumber;
    public int targetNumberIncrease; // �⺻�� = 0, ������ ���� �߰�

    public float attackArea;
    public float attackAreaIncrease;
    public Action onAttack;
    
    public List<EnemyModel> targets;
    public EnemyModel mainTarget;

    public void Attack()
    {
        switch (type)
        {
            case AttackType.Single:
                SingleAttack();
                break;
            case AttackType.Area:
                AreaAttack();
                break;
        }
        onAttack?.Invoke();
    }

    private void SingleAttack() // ���� ����
    {

    }
    
    private void AreaAttack() // ���� ����
    {

    }
}
