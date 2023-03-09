using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject loadingScreen;

    public void Connect()
    {
        loadingScreen.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("ru");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        SceneManager.LoadScene(13);
    }
}
