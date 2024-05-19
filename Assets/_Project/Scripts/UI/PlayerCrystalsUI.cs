using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCrystalsUI : MonoBehaviour
{
    // TODO: Wave 데이터를 업데이트 하려면 Master 말고도 현재 Wave Data를 알고 있거나 RPC를 날려서 UI를 업데이트 해야 하는데...
    // 방법이 떠오르지 않습니다.

    public TMP_Text waveText;
    public TMP_Text enemyCountText;

    public TMP_Text redCrystalText;
    public TMP_Text greenCrystalText;
    public TMP_Text blueCrystalText;

    public TMP_Text specialCrystalText;

    public void UpdateEnemyCount(int enemyCount)
    {
        enemyCountText.text = enemyCount.ToString(); 
    }

    public void UpdateCrystals(Vector3Int crystals)
    {
        redCrystalText.text = crystals.x.ToString();
        greenCrystalText.text = crystals.y.ToString();
        blueCrystalText.text = crystals.z.ToString();
    }

    public void UpdateSpecialCrystal(int specialCrystal)
    {
        specialCrystalText.text = specialCrystal.ToString();
    }

    public void UpdateWave(int wave)
    {
        waveText.text = $"{wave} / {EnemySpawner.Instance.waveDatas.Count}";
    }
}
