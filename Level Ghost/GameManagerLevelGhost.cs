using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerLevelGhost : MonoBehaviour
{
    public GameObject ghost, spawnPoint1, spawnPoint2, gunInCutscene;

    [Header("Камеры")]
    public GameObject cutsceneCameraStart;
    public GameObject cutsceneCameraEnd;
    public GameObject gunCamera;
    [Space]
    public Slider HPSlider;
    public int HP = 100, ghostsKilledCount = 0, ghostsSpawned = 0;
    void Start()
    {
        gunCamera.SetActive(false);
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("*я и не думал, что у нас есть серверная*", "Вы", 3.75f),
            new Speech("Эй, студент!", "Преподаватель", 3f),
            new Speech("Здесь завелись призраки!", "Преподаватель", 3.25f),
            new Speech("Бери вантузную пушку с пола и уничтожай их! Быстрее!", "Преподаватель", 5f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraStart, speeches, "Скорее устраните всех призраков!"));
        StartCoroutine(StartGame());
    }

    public void GhostKilled()
    {
        ghostsKilledCount++;
        if (ghostsKilledCount >= 40) EndLevel();
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(16.5f);
        gunCamera.SetActive(true);
        Destroy(gunInCutscene);
        StartCoroutine(SpawnGhost(spawnPoint1, 2f));
        StartCoroutine(SpawnGhost(spawnPoint2, 2.825f));
    }

    public void EndLevel()
    {
        gunCamera.SetActive(false);
        PlayerPrefs.SetInt("Level3", 1);
        PlayerPrefs.SetInt("Last Level", 1);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Спасибо, студент", "Преподаватель", 2.5f),
            new Speech("Мне сказали, что тебе нужен студенческий билет", "Преподаватель", 4f),
            new Speech("Забирай и быстрее беги!", "Преподаватель", 6.25f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraEnd, speeches, "", 2));
    }

    void Update()
    {
        HPSlider.value = Mathf.Lerp(HPSlider.value, HP, Time.deltaTime * 3);
    }

    private IEnumerator SpawnGhost(GameObject spawnPoint, float delay)
    {
        while(ghostsSpawned < 40)
        {
            ghostsSpawned++;
            Instantiate(ghost, spawnPoint.transform.position, spawnPoint.transform.rotation);
            yield return new WaitForSeconds(delay);
        }
        yield return null;
    }

    public void DamagePlayer(int damage)
    {
        HP -= damage;
        AudioManager.instance.PlaySound(2);
        if (HP <= 0) StartCoroutine(KillPlayer());
    }

    private IEnumerator KillPlayer()
    {
        UIController.instance.isPaused = true;
        UIController.instance.fadeToBlack = true;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(3);
    }

    public static GameManagerLevelGhost instance;
    public void Awake()
    {
        instance = this;
    }
}
