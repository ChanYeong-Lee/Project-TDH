using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Character/Skill")]
public class SkillSO : ScriptableObject
{
    [Header("����")]
    public string skillName;
    [TextArea(2,5)]
    public string description;
    public SkillType skillType;
    public TargetType mainTargetType;

    [Header("����")]
    [Range(0.0f, 1.0f)]
    public float defaultPercent;
    public float defaultCooldown;

    [Header("�ִϸ��̼�")]
    public string triggerName;
    public float skillSpeed;
    public AnimationClip skillClip;
}