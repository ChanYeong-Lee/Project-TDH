using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCrystalsUI : MonoBehaviour
{
    // TODO: Wave �����͸� ������Ʈ �Ϸ��� Master ���� ���� Wave Data�� �˰� �ְų� RPC�� ������ UI�� ������Ʈ �ؾ� �ϴµ�...
    // ����� �������� �ʽ��ϴ�.

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
