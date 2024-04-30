using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IModel { }

public enum CharacterType
{
    TankT1_Peasant      = 00,
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

public class CharacterModel : MonoBehaviourPun, IModel
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public CharacterMove move;
    [HideInInspector] public CharacterAttack attack;
    [HideInInspector] public CharacterSkill skill;
    
    public Action onDisable;
    public Dictionary<string, Buff> buffDictionary;
    
    [Header("¼³Á¤")]
    public CharacterType type;
    public int tier;
    public CharacterSO defaultStat;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();   

        move = GetComponent<CharacterMove>();   
        attack = GetComponent<CharacterAttack>();
        skill = GetComponent<CharacterSkill>();

        buffDictionary = new Dictionary<string, Buff>();
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
        onDisable?.Invoke();    
    }

    [PunRPC]
    public void AddBuff(int casterId, string buffName, BuffType buffType, StatType statType, float increaseAmount, float limitTime)
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (buffDictionary.ContainsKey(buffName))
        {
            return;
        }

        Buff newBuff = new Buff();
        CharacterModel caster = CharacterManager.Instance.wholeCharacters.Find((model) => model.photonView.ViewID == casterId);

        if (caster == null)
        {
            return;
        }

        newBuff.SetBuff(caster, this, buffName, buffType, statType, increaseAmount, limitTime);
        switch (buffType)
        {
            case BuffType.Permanant:
                newBuff.Activate();
                break;
            case BuffType.Limit:
                StartCoroutine(newBuff.BuffCoroutine());
                break;
        }
    }

    [PunRPC]
    public void AddBuff(int casterId, string buffName, BuffType buffType, StatType statType, int integerIncreaseAmount, float limitTime)
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (buffDictionary.ContainsKey(buffName))
        {
            return;
        }

        Buff newBuff = new Buff();
        CharacterModel caster = CharacterManager.Instance.wholeCharacters.Find((model) => model.photonView.ViewID == casterId);

        if (caster == null)
        {
            return;
        }

        newBuff.SetBuff(caster, this, buffName, buffType, statType, integerIncreaseAmount, limitTime);
        switch (buffType)
        {
            case BuffType.Permanant:
                newBuff.Activate();
                break;
            case BuffType.Limit:
                StartCoroutine(newBuff.BuffCoroutine());
                break;
        }
    }

    [PunRPC]
    public void RemoveBuff(string buffName)
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (buffDictionary.ContainsKey(buffName) == false)
        {
            return;
        }

        buffDictionary[buffName].Deactivate();
    }
}