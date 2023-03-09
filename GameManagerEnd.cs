using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManagerEnd : MonoBehaviour
{
    [SerializeField] private GameObject cutsceneCamera;
    [SerializeField] private TextMeshProUGUI[] titles;
    private bool isBlack = false;
    void Start()
    {
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Проснись, студент!", "Преподаватель", 3f),
            new Speech("Хватит спать на паре", "Преподаватель", 4f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCamera, speeches, ""));
        StartCoroutine(StartTitles());
    }

    private IEnumerator StartTitles()
    {
        yield return new WaitForSeconds(9.01f);
        titles[1].text = $"Вы собрали {GameManager.instance.bonusesCollected} из {GameManager.instance.bonusAmount} секретных лекций";
        isBlack = true;
        AudioManager.instance.PlayMusic(0);
        UIController.instance.fadeFromBlack = false;
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        foreach (TextMeshProUGUI title in titles)
        {
            StartCoroutine(ShowAndHideUI(title));
            yield return new WaitForSeconds(5.5f);
        }
        SceneManager.LoadScene(0);
    }

    private IEnumerator ShowAndHideUI(TextMeshProUGUI ui)
    {
        while (ui.color.a <= 0.97f)
        {
            yield return null;
            ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, Mathf.MoveTowards(ui.color.a, 1f, Time.deltaTime));
        }
        yield return new WaitForSeconds(3f);
        while (ui.color.a != 0f)
        {
            yield return null;
            ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, Mathf.MoveTowards(ui.color.a, 0f, Time.deltaTime));
        }
    }

    void Update()
    {
        UIController.instance.isPaused = true;
        cutsceneCamera.SetActive(true);
        if (isBlack) UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
    }
}
