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

    public bool CheckRevolutionable(Vector3Int crystal)
    {
        bool red = crystal.x >= minimumCrystal.x;
        bool green = crystal.y >= minimumCrystal.y;
        bool blue = crystal.z >= minimumCrystal.z;

        return red && green && blue;    
    }
}
