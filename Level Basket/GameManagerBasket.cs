using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerBasket : MonoBehaviour
{
    [SerializeField] private GameObject cutsceneCameraStart, cutsceneCameraEnd;
    private int hit = 0;
    [SerializeField] private GameObject[] levels;
    [SerializeField] private GameObject bootle;

    void Start()
    {
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Так, студент, стой на месте!", "Преподаватель", 3f),
            new Speech("Пришёл за студенческим билетом? Сначала заслужи его", "Преподаватель", 5f),
            new Speech("Попади бутылкой в мусорную корзину!", "Преподаватель", 4f),
            new Speech("Смотри не промахнись! Однажды одному студенту за такое поставили 9 двоек в журнал!", "Преподаватель", 6f)
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraStart, speeches, $"Попадите бутылкой в мусорную корзину ({hit}/{levels.Length})"));
        StartCoroutine(MoveController.instance.SetPropDelay(bootle, 18f));
    }

    public void NewHit()
    {
        AudioManager.instance.PlaySound(1);
        hit++;
        UIController.instance.objective.GetComponent<TextMeshProUGUI>().text = $"Попадите бутылкой в мусорную корзину ({hit}/{levels.Length})";
        if (hit < levels.Length) 
        {
            StartCoroutine(ShowNewLevelDelay(hit, 1f));
            StartCoroutine(MoveController.instance.SetPropDelay(bootle, 1f));
        }
        else EndLevel();
    }

    public void FailHit()
    {
        StartCoroutine(MoveController.instance.SetPropDelay(bootle, 0.5f));
    }

    private void EndLevel()
    {
        PlayerPrefs.SetInt("Level10", 1);
        PlayerPrefs.SetInt("Last Level", 8);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Молодец", "Преподаватель", 1.25f),
            new Speech("Возьми студенческий билет", "Преподаватель", 4f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraEnd, speeches, "", 2));
    }

    private IEnumerator ShowNewLevelDelay(int levelNumber, float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (GameObject level in levels) level.SetActive(false);
        levels[levelNumber].SetActive(true);
    }

    public static GameManagerBasket instance;
    public void Awake()
    {
        instance = this;
    }
}
