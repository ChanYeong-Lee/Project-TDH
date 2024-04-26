using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterType
{
    TankT1_Peasant     = 00,
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

public class CharacterModel : MonoBehaviourPun
{
    public Animator animator;
    public NavMeshAgent agent;

    public int tier;
    public CharacterType type;
    public CharacterSO defaultStat;
    public CharacterMove move;
    public CharacterAttack attack;
    public CharacterSkill skill;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();   

        move = GetComponent<CharacterMove>();   
        attack = GetComponent<CharacterAttack>();
        skill = GetComponent<CharacterSkill>();
    }

    private void OnEnable()
    {
        if (photonView.IsMine)
        {
            CharacterManager.Instance.ownCharacters.Add(this);
            agent.avoidancePriority = 50 + (10 - tier);
        }
        else
        {
            animator.applyRootMotion = false;
            StateMachineBehaviour[] behaviours = animator.GetBehaviours<StateMachineBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {       
                Destroy(behaviours[i]);
            }

            move.enabled = false;
            attack.enabled = false;
            skill.enabled = false;
        }

        CharacterManager.Instance.wholeCharacters.Add(this);
    }

    private void OnDisable()
    {
        if (photonView.IsMine)
        {
            CharacterManager.Instance.ownCharacters.Remove(this);
        }
        CharacterManager.Instance.wholeCharacters.Remove(this);
    }
}