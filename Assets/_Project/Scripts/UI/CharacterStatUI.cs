using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterStatUI : MonoBehaviour
{
    [Header("����")]
    public TMP_Text nameText;
    public TMP_Text tierText;
    public TMP_Text attackTypeText;
    public TMP_Text damageText;
    public TMP_Text trueDamagePercentText;
    public TMP_Text attackDelayText;
    public TMP_Text moveSpeedText;

    [Header("����")]
    public CharacterModel character;

    private void Update()
    {
        if (character == null)
        {
            return;
        }

        nameText.text = character.defaultStat.characterName;
        tierText.text = $"{character.tier}Ƽ��";

        switch (character.defaultStat.attackType)
        {
            case AttackType.Single:
                attackTypeText.text = "����";
                break;
            case AttackType.Area:
                attackTypeText.text = "����";
                break;
        }

        damageText.text = $"{character.attack.applyDamage:F0}";
        trueDamagePercentText.text = $"{character.attack.applyTrueDamagePercent:P0}";
        attackDelayText.text = $"{character.attack.applyAttackDelay:F2}";
        moveSpeedText.text = $"{character.move.applyMoveSpeed:F2}";
    }

    public void SetCharacter(CharacterModel character)
    {
        this.character = character;   
    }
}
