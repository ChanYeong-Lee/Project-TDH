using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public NetworkManager network;
    public DefensePlayer defensePlayer;
    public TMP_Text playerNameText;
    public TMP_Text playerClassText;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        if (PhotonNetwork.InRoom == false)
        {
            GameObject debugStarter = new GameObject("DebugStarter");
            debugStarter.AddComponent<DebugStarter>();
        }
        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Class"));

        UIManager.Instance.starter.Init();

        defensePlayer = new GameObject("Player").AddComponent<DefensePlayer>();
        defensePlayer.Init();
        EnemyManager.Instance.Init();

        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Ready"));

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            defensePlayer.singlePlay = true;
        }
        else
        {
            yield return new WaitUntil(() =>
            {
                bool allReady = false;
                foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    if (player.CustomProperties.ContainsKey("Ready"))
                    {
                        allReady = (bool)player.CustomProperties["Ready"];
                    }
                    else
                    {
                        allReady = false;
                        break;
                    }
                }
                return allReady;
            });
        }

        UIManager.Instance.starter.StartGame();
        yield return new WaitForSeconds(2.0f);

        defensePlayer.transform.SetAsFirstSibling();
        defensePlayer.nickName = PhotonNetwork.LocalPlayer.NickName;
        defensePlayer.classType = (ClassType)PhotonNetwork.LocalPlayer.CustomProperties["Class"];
        UpdatePlayerStatus(defensePlayer.nickName, defensePlayer.classType);

        if (defensePlayer.singlePlay)
        {
            CharacterGenerator.Instance.GenerateCharacter(CharacterType.TankT1_Peasant);
            CharacterGenerator.Instance.GenerateCharacter(CharacterType.DealT1_Peasant);
            CharacterGenerator.Instance.GenerateCharacter(CharacterType.HealT1_Peasant);
        }
        else
        {
            PlayerController.Instance.GenerateNewCharacter();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            NetworkManager network = PhotonNetwork.Instantiate("Prefabs/Managers/NetworkManager", Vector3.zero, Quaternion.identity).GetComponent<NetworkManager>();
            network.Init();

            yield return new WaitForSeconds(3.0f);
            EnemySpawner.Instance.StartSpawn();
        }

    }

    public void UpdatePlayerStatus(string name, ClassType classType)
    {
        playerNameText.text = name;

        switch (classType)
        {
            case ClassType.Tank:
                playerClassText.text = "ÅÊÄ¿";
                break;
            case ClassType.Deal:
                playerClassText.text = "µô·¯";
                break;
            case ClassType.Heal:
                playerClassText.text = "Èú·¯";
                break;
        }
    }
}
