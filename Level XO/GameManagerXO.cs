using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerXO : MonoBehaviour
{
    public GameObject cutsceneCameraStart, cutsceneCameraEnd;
    private int XOWonCount = 0;
    void Start()
    {
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Здравствуй, студент", "Преподаватель", 2.25f),
            new Speech("Какие-то проказники написали на наши большие компьютеры \"Крестики-нолики\" таким образом, что без победы в этой игре против их бота невозможно выключить эти компьютеры", "Преподаватель", 12.75f),
            new Speech("Как разберёшься с этими компьютерами, получишь студенческий билет", "Преподаватель", 6f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraStart, speeches, "Выиграйте в \"Крестики-нолики\" на всех компьютерах (0/6)"));
    }

    public void XOWon()
    {
        XOWonCount++;
        UIController.instance.objective.GetComponent<TextMeshProUGUI>().text = $"Выиграйте в \"Крестики-нолики\" на всех компьютерах ({XOWonCount}/6)";
        if (XOWonCount == 6) EndGame();
    }

    private void EndGame()
    {
        Debug.Log("Уровень завершён");
        PlayerPrefs.SetInt("Level6", 1);
        PlayerPrefs.SetInt("Last Level", 4);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Молодец", "Преподаватель", 1.5f),
            new Speech("Теперь можешь забрать студенческий билет", "Преподаватель", 4.25f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraEnd, speeches, "", 2));
    }

    public static GameManagerXO instance;
    public void Awake()
    {
        instance = this;
    }
}
