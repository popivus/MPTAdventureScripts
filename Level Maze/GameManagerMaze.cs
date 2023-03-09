using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManagerMaze : MonoBehaviour
{
    [Header("Камеры")]
    [SerializeField] private GameObject torchCamera;
    [SerializeField] private GameObject cutsceneStartCamera;
    [SerializeField] private GameObject cutsceneEndCamera;
    [Space]
    [SerializeField] private GameObject finishZone;
    [SerializeField] private int bulbsToCollectCount = 3;
    private int bulbsCollected = 0;
    void Start()
    {
        torchCamera.SetActive(false);
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Здравствуй, студент", "Преподаватель", 2.5f),
            new Speech("У нас перегорели все лампочки, а все запасные находятся в том лабиринте", "Преподаватель", 5f),
            new Speech("Сходи и принеси их, а взамен получишь нужный тебе студенческий билет", "Преподаватель", 5f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneStartCamera, speeches, $"Соберите все лампочки в лабиринте ({bulbsCollected}/{bulbsToCollectCount})"));
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(13f);
        torchCamera.SetActive(true);
    }

    public void BulbCollected()
    {
        AudioManager.instance.PlaySound(1);
        bulbsCollected++;
        UIController.instance.objective.GetComponent<TextMeshProUGUI>().text = $"Соберите все лампочки в лабиринте ({bulbsCollected}/{bulbsToCollectCount})";
        if (bulbsCollected >= bulbsToCollectCount)
        {
            UIController.instance.NewObjective("Вернитесь обратно к преподавателю");
            finishZone.SetActive(true);
        }
    }

    public void EndLevel()
    {
        torchCamera.SetActive(false);
        PlayerPrefs.SetInt("Level8", 1);
        PlayerPrefs.SetInt("Last Level", 6);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Молодец", "Преподаватель", 1.5f),
            new Speech("Держи студенческий билет, тут я дальше сам", "Преподаватель", 4.5f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneEndCamera, speeches, "", 2));
    }

    public static GameManagerMaze instance;
    public void Awake()
    {
        instance = this;
    }
}
