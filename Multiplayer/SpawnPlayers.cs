using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        PhotonNetwork.Instantiate(player.name, spawnPoints[Random.Range(0, spawnPoints.Length - 1)].position, Quaternion.identity);
        Application.targetFrameRate = 150;
    }

    public void Respawn(GameObject player)
    {
        player.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].position;
    }

    public static SpawnPlayers instance;
    public void Awake()
    {
        instance = this;
    }
}
