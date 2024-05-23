using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUI : MonoBehaviour
{
    public RectTransform content;
    public List<DamageIndicator> indicators;
    public DamageIndicator indicatorPrefab;

    public void UpdateDamageUI()
    {
        while (indicators.Count < CharacterManager.Instance.wholeCharacters.Count)
        {
            indicators.Add(Instantiate(indicatorPrefab, content));
        }

        while (indicators.Count > CharacterManager.Instance.wholeCharacters.Count)
        {
            Destroy(indicators[indicators.Count - 1].gameObject);
            indicators.Remove(indicators[indicators.Count - 1]);
        }

        int tempCount = 0;
        for (int i = 0; i < indicators.Count; i++)
        {
            if (i + tempCount > DamageManager.Instance.damageList.Count - 1)
            {
                indicators[i].gameObject.SetActive(false);
                continue;
            }
            else
            {
                indicators[i].gameObject.SetActive(true);
            }

            int viewID = DamageManager.Instance.damageList[i + tempCount].viewID;
            CharacterModel model = CharacterManager.Instance.wholeCharacters.Find((a) => a.photonView.ViewID == viewID);
            if (model == null)
            {
                i--;
                tempCount++;
                continue;
            }

            indicators[i].UpdateDamageIndicator(model, DamageManager.Instance.GetDamage(viewID));
        }
    }
}
