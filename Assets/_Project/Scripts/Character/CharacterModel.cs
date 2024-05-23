using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IModel { }

public enum CharacterType
{
    TankT1_Peasant      = 00,
    TankT2_Warrior      = 01,
    TankT3_Viking       = 02,
    TankT3_Knight       = 03,
    TankT3_General      = 04,
    TankT3_King         = 05,
    
    DealT1_Peasant      = 10,
    DealT2_Archer       = 11,
    DealT3_Ranger       = 12,
    DealT3_Captain      = 13,
    DealT3_Wizzard      = 14,
    DealT3_Hero         = 15,
 
    HealT1_Peasant      = 20,
    HealT2_Gypsy        = 21,
    HealT3_Druid        = 22,
    HealT3_Princess     = 23,
    HealT3_Witch        = 24,
    HealT3_Queen        = 25,
}

public class CharacterModel : MonoBehaviourPun, IModel
{
    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public CharacterMove move;
    [HideInInspector] public CharacterAttack attack;
    [HideInInspector] public CharacterSkill skill;
    [HideInInspector] public CharacterUI ui;

    [Header("설정")]
    public CharacterType type;
    public int tier;
    public CharacterSO defaultStat;

    public Action<CharacterModel> onDisable;
    public Action onSelected;
    public Action onDeselected;

    public Dictionary<string, Buff> buffDictionary;

    [Header("상태")]
    public List<int> crystals;
    public int crystalAmount => crystals.Count;
    public Vector3Int crystalVector
    {
        get
        {
            int red = 0;
            int green = 0;
            int blue = 0;

            for (int i = 0; i < crystals.Count; i++)
            {
                switch (crystals[i])
                {
                    case 0:
                        red++;
                        break;
                    case 1:
                        blue++;
                        break;
                    case 2:
                        green++;
                        break;
                }
            }

            return new Vector3Int(red, green, blue);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();   

        move = GetComponent<CharacterMove>();   
        attack = GetComponent<CharacterAttack>();
        skill = GetComponent<CharacterSkill>();
        ui = Instantiate(Resources.Load<CharacterUI>("Prefabs/Characters/CharacterUI"), transform);
        //ui = GetComponentInChildren<CharacterUI>();

        skill.Init();

        crystals = new List<int>();

        buffDictionary = new Dictionary<string, Buff>();
    }

    private void OnEnable()
    {
        if (photonView.IsMine == false)
        {
            animator.applyRootMotion = false;
            StateMachineBehaviour[] behaviours = animator.GetBehaviours<StateMachineBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {       
                Destroy(behaviours[i]);
            }

            foreach (Skill skill in skill.skills)
            {
                Destroy(skill.gameObject);
            }

            move.enabled = false;
            skill.enabled = false;
            ui.gameObject.SetActive(false);
        }
     
        CharacterManager.Instance.AddCharacter(this);
    }

    private void OnDisable()
    {
        if (PhotonNetwork.IsConnected == false)
        {
            return;
        }

        onDisable?.Invoke(this);    
        CharacterManager.Instance.RemoveCharacter(this);
    }

    public void SetStats()
    {
        move.SetMoveStats(defaultStat);
        attack.SetAttackStats(defaultStat);
        skill.SetSkillStats();
    }

    public void SetGenerationCrystals(List<int> crystals)
    {
        for (int i = 0; i < crystals.Count; i++)
        {
            attack.AddCrystal(crystals[i]);
            move.AddCrystal(crystals[i]);
            skill.AddCrystal(crystals[i]);
        }
    }

    public void AddCrystal(int color)
    {
        this.crystals.Add(color);

        attack.AddCrystal(color);
        move.AddCrystal(color);
        skill.AddCrystal(color);

        foreach (RevolutionData revolutionDatum in defaultStat.revolutionData)
        {
            if (crystalAmount >= revolutionDatum.needCrystalAmount)
            {
                if (revolutionDatum.CheckRevolutionable(this.crystals))
                {
                    CharacterManager.Instance.UpgradeCharacter(this, revolutionDatum.revolutionTarget);
                }
            }
        }
    }

    [PunRPC]
    public void AddBuff(int casterId, string buffName, BuffType buffType, StatType statType, float increaseAmount, float limitTime, PhotonMessageInfo info)
    {
        if (photonView.IsMine == false)
        {
            return;
        }

        if (buffDictionary.ContainsKey(buffName))
        {
            if (buffDictionary[buffName].buffType == BuffType.Limit)
            {
                StopCoroutine(buffDictionary[buffName].buffCoroutine);
            }
            buffDictionary[buffName].Deactivate();
        }

        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);

        Buff newBuff = new Buff();
        CharacterModel caster = CharacterManager.Instance.wholeCharacters.Find((model) => model.photonView.ViewID == casterId);

        if (caster == null)
        {
            return;
        }

        newBuff.SetBuff(caster, this, buffName, buffType, statType, increaseAmount, limitTime - lag);
        buffDictionary[buffName] = newBuff;

        switch (buffType)
        {
            case BuffType.Permanant:
                newBuff.Activate();
                break;
            case BuffType.Limit:
                newBuff.buffCoroutine = StartCoroutine(newBuff.BuffCoroutine());
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
            if (buffDictionary[buffName].buffType == BuffType.Limit)
            {
                StopCoroutine(buffDictionary[buffName].buffCoroutine);
            }
            buffDictionary[buffName].Deactivate();
        }

        Buff newBuff = new Buff();
        CharacterModel caster = CharacterManager.Instance.wholeCharacters.Find((model) => model.photonView.ViewID == casterId);

        if (caster == null)
        {
            return;
        }

        newBuff.SetBuff(caster, this, buffName, buffType, statType, integerIncreaseAmount, limitTime);
        buffDictionary[buffName] = newBuff;

        switch (buffType)
        {
            case BuffType.Permanant:
                newBuff.Activate();
                break;
            case BuffType.Limit:
                newBuff.buffCoroutine = StartCoroutine(newBuff.BuffCoroutine());
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
        buffDictionary.Remove(buffName);
    }
}