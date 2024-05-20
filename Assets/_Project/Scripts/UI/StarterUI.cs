using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class StarterUI : MonoBehaviour
{
    public Image backGroundImage;
    public Button readyButton;

    private void Awake()
    {
        readyButton.gameObject.SetActive(false);
    }

    public void Init()
    {
        readyButton.gameObject.SetActive(true);
        readyButton.onClick.AddListener(OnReadyButtonClick);
    }

    public void StartGame()
    {
        Color disableColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        backGroundImage.DOColor(disableColor, 2.0f).OnComplete(() => gameObject.SetActive(false));
    }

    public void OnReadyButtonClick()
    {
        PhotonHashtable propertiesToSet = PhotonNetwork.LocalPlayer.CustomProperties;
        propertiesToSet["Ready"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(propertiesToSet);
        
        readyButton.gameObject.SetActive(false);
    }
}
