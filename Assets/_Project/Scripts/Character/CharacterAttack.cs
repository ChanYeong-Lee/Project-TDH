using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AttackType
{
    Single, // 정해진 객체 수 만큼 공격
    Area    // 타겟 주위 정해진 범위만큼 공격
}

public class CharacterAttack : MonoBehaviour
{
    public AttackType type;

    public float damage;            // 캐릭터 기본 스탯 + 생성 변수
    public float damageIncrease;    // 기본값 = 1.0, 버프에 따라 변화

    public float attackSpeed;
    public float attackSpeedIncrease;

    public float attackRange;
    public float attackRangeIncrease;   

    public int targetNumber;
    public int targetNumberIncrease; // 기본값 = 0, 버프에 따라 추가

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

    private void SingleAttack() // 단일 공격
    {

    }
    
    private void AreaAttack() // 범위 공격
    {

    }
}
