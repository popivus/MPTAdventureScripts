using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using TMPro;

public class MultiplayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;
    private List<Player> players = new List<Player>();
    public GameObject[] heals;

    public bool isWarmup = true;
    public bool isTimer = false;
    public bool isEnded = false;
    public bool CanVote => players.Count >= 2 ;

    [SerializeField] private TextMeshPro leaderText, leadersKillsText;

    private IEnumerator gameTimerCoroutine;

    private void Start()
    {
        gameTimerCoroutine = StartTimer();
        var newPlayer = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints[Random.Range(0, spawnPoints.Length - 1)].position, Quaternion.identity);
        Application.targetFrameRate = 150;
        for (int i = 0; i < heals.Length; i++) heals[i].GetComponent<HealthPickup>().number = i;
    }

    private void Update()
    {
        if (isTimer) return;

        if (isWarmup)
        {
            if (players.Count >= 2 && players.Count == players.Where(p => p.isReady).Count()) StartCoroutine(StartGame());
        }
        else
        {
            foreach (Player player in players)
            {
                if (player.kills >= 25) StartCoroutine(WinGame(player));
            }
        }
    }

    private IEnumerator StartGame()
    {
        isWarmup = false;
        isTimer = true;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        UI.instance.StopWarmup();
        players = players.OrderBy(p => p.GetView().ViewID).ToList();
        for (int i = 0; i < players.Count; i++) players[i].transform.position = spawnPoints[i].position;
        yield return new WaitForSeconds(4f);
        isTimer = false;
        StartCoroutine(gameTimerCoroutine);
    }

    private IEnumerator WinGame(Player player)
    {
        StopCoroutine(gameTimerCoroutine);
        isTimer = true;
        isEnded = true;
        AudioManager.instance.PlaySound(0);
        UI.instance.SetTimerText($"{player.GetView().Owner.NickName} победитель!");

        var currentPlayer = players.First(p => p.GetView().IsMine);
        var playerModel = API.GET<PlayerModel>($"players/{PlayerPrefs.GetInt("PlayerID", 0)}");

        playerModel.killsCount += currentPlayer.kills;
        playerModel.deathsCount += currentPlayer.deaths;
        playerModel.gamesCount++;
        if (currentPlayer == player) playerModel.gamesWonCount++;

        API.PUT("players", playerModel, playerModel.iD_Player);

        yield return new WaitForSeconds(10f);
        LeaveRoom();
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.ConnectToRegion("ru");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        SceneManager.LoadScene(13);
    }

    private IEnumerator StartTimer()
    {
        int seconds = 600;
        for (int s = 0; s < seconds; s++)
        {
            UI.instance.SetTimerText(seconds - s);
            yield return new WaitForSeconds(1f);
        }
        Player winner = players
            .OrderBy(p => p.kills)
            .Reverse()
            .First();
        StartCoroutine(WinGame(winner));
    }

    public void KilledPlayer(int viewIdKiller, int viewIdDead)
    {
        var killer = players.First(p => p.GetView().ViewID == viewIdKiller);
        var dead = players.First(p => p.GetView().ViewID == viewIdDead);
        killer.kills++;
        dead.deaths++;
        StartCoroutine(RefreshScoreboardDelay());
    }

    private IEnumerator RefreshScoreboardDelay()
    {
        yield return new WaitForSeconds(0.15f);
        players = players
            .OrderBy(p => p.kills)
            .Reverse()
            .ToList();
        UI.instance.RefreshScoreboard(players);

        if (leaderText != null && !isWarmup)
        {
            leaderText.text = players[0].GetView().Owner.NickName;
            leadersKillsText.text = players[0].kills.ToString() + " убийств";
        }
    }

    public Player GetPlayerByViewId(int id)
    {
        return players.First(p => p.GetView().ViewID == id);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UI.instance.Feed($"{newPlayer.NickName} присоединился");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UI.instance.Feed($"{otherPlayer.NickName} отключился");
        StartCoroutine(RemoveLeftedPlayer());
    }

    private IEnumerator RemoveLeftedPlayer()
    {
        yield return new WaitForSeconds(0.15f);
        players.Remove(players.First(p => p == null));
        UI.instance.RefreshScoreboard(players);
        
        if (isWarmup)
        {
            if (players.Count == 1)
            {
                UI.instance.StopVote();
                foreach(Player player in players) player.isReady = false;
            }
            UI.instance.SetWaitText($"ОЖИДАНИЕ ({players.Count}/8)");
        }
        else
        {
            if (players.Count == 1) StartCoroutine(WinGame(players.First()));
        }
    }

    public void AddPlayer(Player newPlayer)
    {
        players.Add(newPlayer);
        UI.instance.RefreshScoreboard(players);

        if (isWarmup)
        {
            if (players.Count == 2) UI.instance.StartVote();
            UI.instance.SetWaitText($"ОЖИДАНИЕ ({players.Count}/8)");
        }
    }

    public void Respawn(GameObject playerToRespawn)
    {
        Vector3 currentSpawnPointPosition = new Vector3();
        float distanceToNearPlayer = 0f;
        foreach (Transform spawnPoint in spawnPoints)
        {
            float distanceToPlayer = Mathf.Infinity;
            foreach (Player player in players)
            {
                if (playerToRespawn.GetComponent<Player>().GetView().ViewID != player.GetView().ViewID)
                {
                    float distance = Vector3.Distance(spawnPoint.position, player.transform.position);
                    if (distanceToPlayer > distance) distanceToPlayer = distance;
                }
            }

            if (distanceToNearPlayer < distanceToPlayer)
            {
                distanceToNearPlayer = distanceToPlayer;
                currentSpawnPointPosition = spawnPoint.position;
            }
        }

        playerToRespawn.transform.position = currentSpawnPointPosition;
    }

    public static MultiplayerManager instance;
    public void Awake()
    {
        instance = this;
    }
}
