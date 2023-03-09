using Photon.Pun;
using Photon.Realtime; 
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class MenuMultiplayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField joinInput, createInput;
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;
    
    [Header("Список комнат")]
    [SerializeField] private Transform gridContent;
    [SerializeField] private ListItem itemPrefab;

    [Header("Никнейм")]
    [SerializeField] private TMP_InputField nicknameInput;
    [SerializeField] private GameObject nicknamePanel;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Комнат: " + PhotonNetwork.CountOfRooms.ToString());
        if (PlayerPrefs.GetString("Nickname", "") == "")
        {
            nicknamePanel.SetActive(nicknamePanel);
        }
        else PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname", "");
    }

    public void SetNickname()
    {
        if (string.IsNullOrWhiteSpace(nicknameInput.text)) return;
        
        string nickname = nicknameInput.text.Trim();

        var playerList = API.GET<PlayerModelList>("players");
        if (playerList.playerModels.Where(p => p.nickname == nickname).Count() != 0)
        {
            ShowMessageBox("Такой никнейм уже занят");
            return;
        }

        PlayerModel newPlayerModel = new PlayerModel();
        newPlayerModel.nickname = nickname;
        newPlayerModel.killsCount = 0;
        newPlayerModel.deathsCount = 0;
        newPlayerModel.gamesWonCount = 0;
        newPlayerModel.gamesCount = 0;
        var newPlayer = API.POST("players", newPlayerModel);

        PlayerPrefs.SetString("Nickname", newPlayer.nickname);
        PlayerPrefs.SetInt("PlayerID", newPlayer.iD_Player);
        nicknamePanel.SetActive(false);
        PhotonNetwork.NickName = nickname;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform item in gridContent.GetComponentsInChildren(typeof(Transform), true)) if (gridContent != item) Destroy(item.gameObject);
        foreach (RoomInfo room in roomList)
        {
            ListItem listItem = Instantiate(itemPrefab, gridContent);
            if (listItem != null) 
            {
                listItem.SetInfo(room);
                listItem.gameObject.GetComponent<Button>().onClick.AddListener(delegate {RoomClicked(room.Name);});
            }
        }
    }

    public void RoomClicked(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrWhiteSpace(createInput.text))
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 8;
            roomOptions.IsVisible = true;
            PhotonNetwork.CreateRoom(createInput.text.Trim(), roomOptions, TypedLobby.Default);
        }
    }

    public void JoinRoom()
    {
        if (!string.IsNullOrWhiteSpace(joinInput.text)) PhotonNetwork.JoinRoom(joinInput.text.Trim());
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(14);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ShowMessageBox(message);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ShowMessageBox(message);
    }

    private void ShowMessageBox(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene(0);
    }

    public void ShowPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void HidePanel(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void MainMenu()
    {
        PhotonNetwork.Disconnect();
    }
}
