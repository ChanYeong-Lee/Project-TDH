using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Character/Skill")]
public class SkillSO : ScriptableObject
{
    [Header("정보")]
    public string skillName;
    [TextArea(2,5)]
    public string description;
    public SkillType skillType;
    public TargetType mainTargetType;

    [Header("설정")]
    [Range(0.0f, 1.0f)]
    public float defaultPercent;
    public float defaultCooldown;

    [Header("애니메이션")]
    public string triggerName;
    public float skillSpeed;
    public AnimationClip skillClip;
}