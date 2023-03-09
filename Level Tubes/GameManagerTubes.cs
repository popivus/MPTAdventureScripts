using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerTubes : MonoBehaviour
{
    public GameObject cutsceneCameraStart, cutsceneCameraEnd;
    private int gridTubesWonCount = 0;

    void Start()
    {
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Хм-м-м...", "Преподаватель", 2.5f),
            new Speech("Странные трубы заполонили весь кабинет", "Преподаватель", 4f),
            new Speech("Мне кажется, если их прокрутить так, чтобы каждая из них была соединена друг с другом, то они отсюда уйдут", "Преподаватель", 9f),
            new Speech("Сможешь справиться с этим, студент?", "Преподаватель", 4f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraStart, speeches, "Соедините все трубы в каждой полосе (0/3)"));
    }

    public void GridTubesWon()
    {
        gridTubesWonCount++;
        UIController.instance.objective.GetComponent<TextMeshProUGUI>().text = $"Соедините все трубы в каждой полосе ({gridTubesWonCount}/3)";
        if (gridTubesWonCount == 3) EndGame();
    }

    private void EndGame()
    {
        Debug.Log("Уровень завершён");
        PlayerPrefs.SetInt("Level5", 1);
        PlayerPrefs.SetInt("Last Level", 3);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Спасибо", "Преподаватель", 1.5f),
            new Speech("Держи свой студенческий билет", "Преподаватель", 4f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraEnd, speeches, "", 2));
    }

    public static GameManagerTubes instance;
    public void Awake()
    {
        instance = this;
    }
}
