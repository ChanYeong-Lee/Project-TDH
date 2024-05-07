using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    MoveSpeed,
    Damage,
    TrueDamagePercent,
    AttackDelay,
    Defense,
    TargetNumber,
    AttackArea,
}

public enum BuffType
{
    Permanant,
    Limit,
}

public class Buff
{
    public CharacterModel caster;
    public string buffName;

    public BuffType buffType;
    public StatType statType;

    public float increaseAmount;
    public int integerIncreaseAmount;
    public float limitTime;
    public Coroutine buffCoroutine;

    private CharacterModel allyTarget;
    private EnemyModel enemyTarget;

    public void SetBuff(CharacterModel caster, IModel target, string buffName, BuffType buffType, StatType statType, float increaseAmount, float limitTime)
    {
        this.caster = caster;
        this.buffName = buffName;
        this.buffType = buffType;
        this.statType = statType;
        this.increaseAmount = increaseAmount;
        this.limitTime = limitTime;

        if (target is CharacterModel)
        {
            allyTarget = (CharacterModel)target;
        }
        else if (target is EnemyModel)
        {
            enemyTarget = (EnemyModel)target;
        }

        if (buffType == BuffType.Permanant)
        {
            caster.onDisable -= Deactivate;
            caster.onDisable += Deactivate;
        }
    }

    public void SetBuff(CharacterModel caster, IModel target, string buffName, BuffType buffType, StatType statType, int integerIncreaseAmount, float limitTime)
    {
        this.caster = caster;
        this.buffName = buffName;
        this.buffType = buffType;
        this.statType = statType;
        this.integerIncreaseAmount = integerIncreaseAmount;
        this.limitTime = limitTime;

        if (target is CharacterModel)
        {
            allyTarget = (CharacterModel)target;
        }
        else if (target is EnemyModel)
        {
            enemyTarget = (EnemyModel)target;
        }

        if (buffType == BuffType.Permanant)
        {
            caster.onDisable -= Deactivate;
            caster.onDisable += Deactivate;
        }
    }

    public IEnumerator BuffCoroutine()
    {
        Activate();
        yield return new WaitForSeconds(this.limitTime);
        Deactivate();
        buffCoroutine = null;
    }

    public void Activate()
    {
        if (allyTarget != null)
        {
            switch (statType)
            {
                case StatType.MoveSpeed:
                    allyTarget.move.moveSpeedIncrease += increaseAmount;
                    break;
                case StatType.Damage:
                    allyTarget.attack.damageIncrease += increaseAmount;
                    break;
                case StatType.TrueDamagePercent:
                    allyTarget.attack.trueDamagePercentIncrease += increaseAmount;
                    break;
                case StatType.AttackDelay:
                    allyTarget.attack.attackDelayIncrease += increaseAmount;
                    break;
                case StatType.TargetNumber:
                    allyTarget.attack.targetNumberIncrease += integerIncreaseAmount;
                    break;
                case StatType.AttackArea:
                    allyTarget.attack.attackAreaIncrease += increaseAmount;
                    break;
            }
        }
        else if (enemyTarget != null)
        {
            switch (statType)
            {
                case StatType.MoveSpeed:
                    enemyTarget.move.moveSpeedIncrease += increaseAmount;
                    break;
                case StatType.Defense:
                    enemyTarget.health.defenseIncrease += increaseAmount;
                    break;
            }
        }
    }

    public void Deactivate()
    {
        if (allyTarget != null)
        {
            if (allyTarget.buffDictionary.ContainsKey(buffName) == false)
            {
                return;
            }

            allyTarget.buffDictionary.Remove(buffName);
            switch (statType)
            {
                case StatType.MoveSpeed:
                    allyTarget.move.moveSpeedIncrease -= increaseAmount;
                    break;
                case StatType.Damage:
                    allyTarget.attack.damageIncrease -= increaseAmount;
                    break;
                case StatType.TrueDamagePercent:
                    allyTarget.attack.trueDamagePercentIncrease -= increaseAmount;
                    break;
                case StatType.AttackDelay:
                    allyTarget.attack.attackDelayIncrease -= increaseAmount;
                    break;
                case StatType.TargetNumber:
                    allyTarget.attack.targetNumberIncrease -= integerIncreaseAmount;
                    break;
                case StatType.AttackArea:
                    allyTarget.attack.attackAreaIncrease -= increaseAmount;
                    break;
            }
        }
        else if (enemyTarget != null)
        {
            if (enemyTarget.buffDictionary.ContainsKey(buffName) == false)
            {
                return;
            }

            enemyTarget.buffDictionary.Remove(buffName);

            switch (statType)
            {
                case StatType.MoveSpeed:
                    enemyTarget.move.moveSpeedIncrease -= increaseAmount;
                    break;
                case StatType.Defense:
                    enemyTarget.health.defenseIncrease -= increaseAmount;
                    break;
            }
        }
    }
}
