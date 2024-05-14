using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stat", menuName = "Character/Character Stat")]
public class CharacterSO : ScriptableObject
{
    public string characterName;
    public Sprite characterIcon;

    public AnimationClip walkingClip;
    public float defaultMoveSpeed;
    
    public AttackType attackType;
    public AnimationClip attackClip;

    public Projectile projectilePrefab;

    public float defaultDamage;
    public float defaultTrueDamagePercent;
    public float defaultAttackSpeed;
    public float defaultAttackDelay;
    public float defaultAttackRange;

    public int defaultTargetNumber;
    public float defaultAttackArea;

    public List<RevolutionData> revolutionData;
}
