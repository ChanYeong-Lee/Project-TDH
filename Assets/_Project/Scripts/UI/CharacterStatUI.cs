using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterStatUI : MonoBehaviour
{
    [Header("설정")]
    public TMP_Text nameText;
    public TMP_Text tierText;
    public TMP_Text attackTypeText;
    public TMP_Text damageText;
    public TMP_Text trueDamagePercentText;
    public TMP_Text attackDelayText;
    public TMP_Text moveSpeedText;

    [Header("상태")]
    public CharacterModel character;

    private void Update()
    {
        if (character == null)
        {
            return;
        }

        UpdateInfo();
    }

    private void UpdateInfo()
    {
        if (character == null)
        {
            return;
        }

        nameText.text = character.defaultStat.characterName;
        tierText.text = $"{character.tier}티어";

        switch (character.defaultStat.attackType)
        {
            case AttackType.Single:
                attackTypeText.text = "단일";
                break;
            case AttackType.Area:
                attackTypeText.text = "범위";
                break;
        }

        damageText.text = $"{character.attack.damage:F0} <color=green>+ {character.attack.applyDamage - character.attack.damage:F0}</color>";
        trueDamagePercentText.text = $"{character.attack.trueDamagePercent:P0} <color=green>+ {character.attack.applyTrueDamagePercent - character.attack.trueDamagePercent:P0}</color>";
        attackDelayText.text = $"{character.attack.attackDelay:F2} <color=green>+ {character.attack.applyAttackDelay - character.attack.attackDelay:F2}</color>";
        moveSpeedText.text = $"{character.move.moveSpeed:F2} <color=green>+ {character.move.applyMoveSpeed - character.move.moveSpeed:F2}</color>";
    }

    public void SetCharacter(CharacterModel character)
    {
        this.character = character;
        UpdateInfo();
    }
}
