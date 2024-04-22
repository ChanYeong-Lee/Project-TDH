using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using UnityEditor;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
public class DebugStarter : MonoBehaviourPunCallbacks
{
    private ClientState currentState;

    private IEnumerator Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
        PhotonNetwork.JoinLobby();

        yield return new WaitUntil(() => PhotonNetwork.InLobby);
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: "TestRoom");

        yield return new WaitUntil(() => PhotonNetwork.InRoom);
        yield return new WaitUntil(() => PhotonNetwork.LocalPlayer.GetPlayerNumber() >= 0);

        PhotonNetwork.NickName = $"Player {PhotonNetwork.LocalPlayer.GetPlayerNumber()}";
        PhotonHashtable propertiesToSet = PhotonNetwork.LocalPlayer.CustomProperties;
        propertiesToSet["Class"] = PhotonNetwork.LocalPlayer.GetPlayerNumber();
        PhotonNetwork.LocalPlayer.SetCustomProperties(propertiesToSet);
    }

    private void Update()
    {
        if (PhotonNetwork.NetworkClientState != currentState)
        {
            currentState = PhotonNetwork.NetworkClientState;   
            print(currentState.ToString());
        }
    }
}
