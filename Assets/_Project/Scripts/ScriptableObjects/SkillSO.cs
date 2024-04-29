using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Character/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    [TextArea(2,5)]
    public string description;

    public string triggerName;
    public float skillSpeed;
    public AnimationClip skillClip;

    [Range(0.0f, 1.0f)] 
    public float defaultPercent;
    public float defaultAmount;
    public float defaultAmountRate;

    public float defaultCooldown;
    public float defaultArea;
    public int defaultTargetNumber;
}
