using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stat", menuName = "Character/Character Stat")]
public class CharacterSO : ScriptableObject
{
    public string characterName;

    public float defaultMoveSpeed;

    public float defaultDamage;
    public float defaultTrueDamagePercent;
    public float defaultAttackSpeed;
    public float defaultAttackDelay;
    public float defaultAttackRange;

    public int defaultTargetNumber;
    public float defaultAttackArea;
}
