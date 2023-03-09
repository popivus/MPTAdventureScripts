using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class ListItem : MonoBehaviour
{
    [SerializeField] Text roomNameText, playersCountText;

    public void SetInfo(RoomInfo room)
    {
        roomNameText.text = room.Name;
        playersCountText.text = $"{room.PlayerCount}/{room.MaxPlayers}";
    }
}
