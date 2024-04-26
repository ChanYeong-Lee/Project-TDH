using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

        //yield return new WaitUntil(() => PhotonNetwork.CurrentRoom.PlayerCount == 2);

        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Class"));
        
        defensePlayer = new DefensePlayer();
        defensePlayer.nickName = PhotonNetwork.LocalPlayer.NickName;
        defensePlayer.classType = (ClassType)PhotonNetwork.LocalPlayer.CustomProperties["Class"];

        CharacterGenerator.Instance.GenerateCharacter(CharacterType.DealT1_Peasant);

        if (PhotonNetwork.IsMasterClient)
        {
            //yield return new WaitForSeconds(3.0f);
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
