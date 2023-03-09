using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManagerRace : MonoBehaviour
{
    [SerializeField] private GameObject cutsceneCameraStart, cutsceneCameraEnd, mainCamera;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private int allLapsCount = 5;
    [SerializeField] private GameObject car;
    [SerializeField] private TextMeshProUGUI timerText;
    private int lapDrivedCount = 0;
    private bool[] checkpointsPassed; 
    private bool isRespawning = false, isRaceFinished = false;

    private void Start()
    {
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Привет!", "Студент-проказник", 1.75f),
            new Speech("Хочешь получить студенческий билет?", "Студент-проказник", 4f),
            new Speech("Тогда проедь эту трассу на время!", "Студент-проказник", 4f),
            new Speech("У тебя всего лишь полторы минуты! Поторопись!", "Студент-проказник", 4.25f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraStart, speeches, $"Пройдите все круги за выделенное время ({lapDrivedCount}/{allLapsCount})"));
        checkpointsPassed = new bool[]
        {
            false, false, false
        };
        StartCoroutine(StartTimer(88, 14.5f));
    }

    public void LapDrived()
    {
        bool allCheckpointPassed = true;
        foreach (bool checkpoint in checkpointsPassed) if (!checkpoint) allCheckpointPassed = false;
        if (allCheckpointPassed)
        {
            lapDrivedCount++;
            UIController.instance.objective.GetComponent<TextMeshProUGUI>().text = $"Пройдите все круги за выделенное время ({lapDrivedCount}/{allLapsCount})";
            AudioManager.instance.PlaySound(1);
            if (lapDrivedCount >= allLapsCount) EndLevel();
        }
    }

    public void CheckpointPassed(int checkpointNumber)
    {
        checkpointsPassed[checkpointNumber] = true;
    }

    private void EndLevel()
    {
        isRaceFinished = true;
        Debug.Log("Уровень завершён");
        PlayerPrefs.SetInt("Level7", 1);
        PlayerPrefs.SetInt("Last Level", 5);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("А ты хорош!", "Студент-проказник", 1.5f),
            new Speech("Заслужил свой студенческий билет!", "Студент-проказник", 4.25f),
        };
        StartCoroutine(RemoveMainCameraDelay(1f));
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraEnd, speeches, "", 2));
    }

    private IEnumerator RemoveMainCameraDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        mainCamera.SetActive(false);
    }
    
    private void Update()
    {
        UIController.instance.SetCornerText($"{ControlsController.interact} - возродиться на старте");
        if (Input.GetKeyUp(ControlsController.interact) && !isRespawning) StartCoroutine(Respawn());
    }

    private IEnumerator StartTimer(int seconds, float delay)
    {
        yield return new WaitForSeconds(delay);
        mainCamera.SetActive(true);
        for (int s = 0; s < seconds; s++)
        {
            timerText.text = $"{(seconds - s) / 60}:{(((seconds - s) % 60).ToString().Length != 1 ? ((seconds - s) % 60).ToString() : ("0" + ((seconds - s) % 60).ToString()))}";
            yield return new WaitForSeconds(1f);
        }
        if (!isRaceFinished)
        {
            timerText.text = "Вы проиграли!";
            yield return new WaitForSeconds(2f);
            UIController.instance.fadeToBlack = true;
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(7);
        }
    }

    private IEnumerator Respawn()
    {
        isRespawning = true;
        UIController.instance.fadeToBlack = true;
        yield return new WaitForSeconds(1f);
        car.GetComponent<Rigidbody>().Sleep();
        car.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
        UIController.instance.fadeFromBlack = true;
        isRespawning = false;
    }

    public static GameManagerRace instance;
    public void Awake()
    {
        instance = this;
    }
}
