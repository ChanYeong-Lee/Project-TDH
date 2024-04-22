using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    TankdT1_Peasant     = 00,
    TankT2_Warrior      = 01,
    TankT3_Viking       = 02,
    TankT3_General      = 03,
    TankT3_Knight       = 04,
    TankT3_King         = 05,
    
    DealT1_Peasant      = 10,
    DealT2_Archer       = 11,
    DealT3_Ranger       = 12,
    DealT3_Pirate       = 13,
    DealT3_Wizzard      = 14,
    DealT3_Hero         = 15,
 
    HealT1_Peasant      = 20,
    HealT2_Gypsy        = 21,
    HealT3_Druid        = 22,
    HealT3_Witch        = 23,
    HealT3_Princess     = 24,
    HealT3_Queen        = 25,
}

public class CharacterModel : MonoBehaviour
{
    public CharacterType type;
    public CharacterMove move;

    private void Awake()
    {
        move = GetComponent<CharacterMove>();   
    }
}