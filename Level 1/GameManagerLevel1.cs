using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManagerLevel1 : MonoBehaviour
{
    public int studentIDs = 0;
    public GameObject zone, cutsceneCameraStart, cutsceneCameraEnd;
    public bool objectiveDone = false;
    void Start()
    {
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Просыпайся, студент", "Преподаватель", 3.25f),
            new Speech("Звонок уже давно прозвенел", "Преподаватель", 3.25f),
            new Speech("У меня для тебя задание", "Преподаватель", 3f),
            new Speech("Тебе необходимо отнести эти студенческие билеты к 313 кабинету", "Преподаватель", 5f),
            new Speech("Только будь аккуратней, там какой-то студент пролил лаву", "Преподаватель", 5f),
            new Speech("Смотри не упади!", "Преподаватель", 3f)
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraStart, speeches, "Отнесите студенческие билеты к 313 кабинету (0/2)"));
    }

    public void EndLevel()
    {
        PlayerPrefs.SetInt("New Game", 0);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Молодец, студент", "Преподаватель", 3.25f),
            new Speech("Но это ещё не всё", "Преподаватель", 3.25f),
            new Speech("Тебе придётся найти остальные билеты во всех других кабинетах на этаже", "Преподаватель", 6.25f),
            new Speech("Ты же хочешь хорошо сдать курсовой проект?", "Преподаватель", 4.25f),
            new Speech("Поэтому надо бы немного помочь любимому техникуму", "Преподаватель", 4.25f),
            new Speech("Можешь подождать здесь, пока не уберут всю лаву из коридора и пойти выполнять свою задачу", "Преподаватель", 7.25f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraEnd, speeches, "", 2));
    }

    public void StudentIDPlus()
    {
        studentIDs++;
        AudioManager.instance.PlaySound(2);
        UIController.instance.objective.GetComponent<TextMeshProUGUI>().text = $"Отнесите студенческие билеты к 313 кабинету ({studentIDs}/2)";
        if (studentIDs == 2)
        {
            UIController.instance.NewObjective("Вернитесь обратно в кабинет");
            objectiveDone = true;
        }
    }

    public void StudentIDMinus()
    {
        studentIDs--;
        UIController.instance.objective.GetComponent<TextMeshProUGUI>().text = $"Отнесите студенческие билеты к 313 кабинету ({studentIDs}/2)";
        if (studentIDs != 2) objectiveDone = false;
    }

    public static GameManagerLevel1 instance;
    public void Awake()
    {
        instance = this;
    }
}
