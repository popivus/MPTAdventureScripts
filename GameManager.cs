using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int bonusAmount = 10, bonusesCollected = 0;
    public bool showTip;
    private List<int> bonuses;

    public void BonusCollected(int bonusNumber)
    {
        bonuses[bonusNumber] = 1;
        bonusesCollected++;
        PlayerPrefs.SetInt($"bonus{bonusNumber}", 1);
        UIController.instance.BonusCollected();
        AudioManager.instance.PlaySound(0);
    }

    void Start()
    {
        bonuses = new List<int>();
        for (int i = 0; i < bonusAmount - 1; i++)
        {
            bonuses.Add(PlayerPrefs.GetInt($"bonus{i}", 0));
        }
        bonusesCollected = bonuses.Where(b => b == 1).Count();
    }
    
    public IEnumerator LoadLevel(int levelNumber)
    {
        UIController.instance.fadeToBlack = true;
        AudioManager.instance.PlaySound(1);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(levelNumber);
    }

    public IEnumerator Cutscene(GameObject cutsceneCamera, List<Speech> speeches, string objective = "", int sceneNumber = 0)
    {
        if (UIController.instance.blackScreen.GetComponent<Image>().color.a != 1f)
        {
            UIController.instance.fadeToBlack = true;
            yield return new WaitForSeconds(2f);
        }
        cutsceneCamera.SetActive(true);
        UIController.instance.mainUI.SetActive(false);
        UIController.instance.cutsceneUI.SetActive(true);
        UIController.instance.isPaused = true;
        UIController.instance.isCutscene = true;
        UIController.instance.fadeFromBlack = true;
        cutsceneCamera.GetComponent<Animation>().Play();

        foreach (Speech speech in speeches)
        {
            UIController.instance.NewSpeech(speech.speech, speech.name);
            yield return new WaitForSeconds(speech.time);
        }

        UIController.instance.fadeToBlack = true;
        yield return new WaitForSeconds(2f);
        if (sceneNumber != 0) SceneManager.LoadScene(sceneNumber);
        cutsceneCamera.SetActive(false);
        UIController.instance.mainUI.SetActive(true);
        UIController.instance.cutsceneUI.SetActive(false);
        UIController.instance.isPaused = false;
        UIController.instance.isCutscene = false;
        UIController.instance.fadeFromBlack = true;
        yield return new WaitForSeconds(0.5f);
        UIController.instance.NewObjective(objective);
        yield return new WaitForSeconds(0.5f);
        if (showTip) UIController.instance.ShowTip();
    }

    public static GameManager instance;
    public void Awake()
    {
        instance = this;
    }
}
