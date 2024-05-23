using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image classImage;

    public TMP_Text characterNameText;
    public TMP_Text damageText;
    public TMP_Text dpsText;

    public void UpdateDamageIndicator(CharacterModel model, DamageInfo info)
    {
        if (model == null)
        {
            return;
        }

        characterNameText.text = model.defaultStat.characterName;

        switch ((int)model.type / 10)
        {
            case 0:
                classImage.color = new Color(0.0f, 0.0f, 1.0f, 0.1f);
                break;
            case 1:
                classImage.color = new Color(1.0f, 0.0f, 0.0f, 0.1f);
                break;
            case 2:
                classImage.color = new Color(0.0f, 1.0f, 0.0f, 0.1f);
                break;
        }

        damageText.text = info.damage.ToString("N0");
        dpsText.text = info.damagePerSeconds.ToString("N0");
    }
}
