using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RevolutionData
{
    public int needCrystalAmount;
    public Vector3Int minimumCrystal;
    public CharacterType revolutionTarget;

    public bool CheckRevolutionable(List<int> crystals)
    {
        bool red = CheckCrystalAmount(crystals, 0) >= minimumCrystal.x;
        bool green = CheckCrystalAmount(crystals, 1) >= minimumCrystal.y;
        bool blue = CheckCrystalAmount(crystals, 2) >= minimumCrystal.z;

        return red && green && blue;    
    }

    private int CheckCrystalAmount(List<int> crystals, int color)
    {
        int amount = 0;
        for (int i = 0; i < crystals.Count; i++)
        {
            if (crystals[i] == color)
            {
                amount++;
            }
        }

        return amount;
    }
}
