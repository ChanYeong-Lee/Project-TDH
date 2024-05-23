using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            for (int i = 0; i < 10; i++)
            {
                GameManager.Instance.defensePlayer.AddRandomCrystal();
            }
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            for (int i = 0; i < 10; i++)
            {
                GameManager.Instance.defensePlayer.AddSpecialCrystal();
            }
        }
    }
}
