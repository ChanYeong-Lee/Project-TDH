using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text classText;
    public TMP_Text characterNameText;
    public TMP_Text damageText;

    public void UpdateDamageIndicator(CharacterModel model, float damage)
    {
        if (model == null)
        {
            return;
        }

        playerNameText.text = model.photonView.Owner.NickName;
        characterNameText.text = model.defaultStat.characterName;

        switch ((int)model.type / 10)
        {
            case 0:
                classText.text = "ÅÊÄ¿";
                break;
            case 1:
                classText.text = "µô·¯";
                break;
            case 2:
                classText.text = "Èú·¯";
                break;
        }

        damageText.text = damage.ToString("F2");
    }
}
