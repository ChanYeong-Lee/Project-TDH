using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ClassType 
{ 
    Tank,
    Deal,
    Heal
}

public class DefensePlayer : MonoBehaviour
{
    public Action<Vector3Int> onCrystalsChange;
    public Action<int> onSpecialCrystalChange;

    public string nickName;
    public ClassType classType;

    public bool singlePlay;

    public Vector3Int crystals;
    public Vector3Int Crystals
    {
        get { return crystals; }
        set
        {
            crystals = value;
            onCrystalsChange?.Invoke(crystals);
        }
    }

    public int specialCrystal;
    public int SpecialCrystal
    {
        get
        {
            return specialCrystal;
        }
        set
        {
            specialCrystal = value;
            onSpecialCrystalChange?.Invoke(specialCrystal);
        }
    }

    public void Init()
    {
        onCrystalsChange += UIManager.Instance.playerCrystals.UpdateCrystals;
        onSpecialCrystalChange += UIManager.Instance.playerCrystals.UpdateSpecialCrystal;
    }

    public void AddRandomCrystal()
    {
        int randomInteger = Random.Range(0, 3);
        Vector3Int crystals = Vector3Int.zero;

        switch (randomInteger)
        {
            case 0:
                crystals = new Vector3Int(1, 0, 0);
                break;
            case 1:
                crystals = new Vector3Int(0, 1, 0);
                break;
            case 2:
                crystals = new Vector3Int(0, 0, 1);
                break;
        }

        this.Crystals += crystals;
    }

    public void AddSpecialCrystal()
    {
        SpecialCrystal++;
    }

    public bool UseCrystal(int color)
    {
        bool haveCrystal = false;
        switch (color)
        {
            case 0:
                if (crystals.x > 0)
                {
                    Crystals -= new Vector3Int(1, 0, 0);
                    haveCrystal = true;
                }
                break;
            case 1:
                if (crystals.y > 0)
                {
                    Crystals -= new Vector3Int(0, 1, 0);
                    haveCrystal = true;
                }
                break;
            case 2:
                if (crystals.z > 0)
                {
                    Crystals -= new Vector3Int(0, 0, 1);
                    haveCrystal = true;
                }
                break;
        }

        if (haveCrystal)
        {
            return true;
        }
        else if (SpecialCrystal > 0)
        {
            SpecialCrystal--;
            return true;
        }
        else
        {
            return false;
        }
    }
}