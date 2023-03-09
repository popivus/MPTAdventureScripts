using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManagerBoss : MonoBehaviour
{
    [SerializeField] private GameObject[] levels;
    private int currentLevelNumber = 0;
    private System.Random random;
    [SerializeField] private GameObject cutsceneLocation, cutsceneCameraEnd, player;

    [Header("Первый босс")]
    [SerializeField] private GameObject cutsceneCamera1;
    public Transform respawn;
    public GameObject bullet;
    [SerializeField] private Boss1Controller boss1;

    [Header("Второй босс")]
    [SerializeField] private GameObject cutsceneCamera2;
    [SerializeField] private GameObject level2UI;
    [SerializeField] private TextMeshProUGUI wordToInputText, wordsCompletedText, timerText;
    [SerializeField] private InputField inputWord;
    [SerializeField] private string[] words;
    private string wordToInput;
    private int wordsInputedCount = 0;

    [Header("Третий босс")]
    [SerializeField] private GameObject gunModel;
    [SerializeField] private GameObject cutsceneCamera3;
    [SerializeField] private Transform respawnFinalBoss;
    [SerializeField] private GameObject bossUI, playerGun;
    [SerializeField] private FinalBossController finalBoss;
    [SerializeField] private Slider playerHPSlider, bossHPSlider;
    [SerializeField] private int playerHP = 100, bossHP = 1000;
    private int maxPlayerHp, maxBossHp;
    private bool isKilled = false;
    

    private void Start()
    {
        timer = TimerWord(5);
        random = new System.Random();
        //boss1.StartShooting();
        //StartCoroutine(EnterSecondLevel(1f));
        //StartCoroutine(finalBoss.StartAttackingDelay(1f));
        StartCoroutine(EnterFirstLevel());
        maxPlayerHp = playerHP;
        maxBossHp = bossHP;
        playerHPSlider.maxValue = maxPlayerHp;
        bossHPSlider.maxValue = maxBossHp;
    }

    private IEnumerator EnterFirstLevel()
    {
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("", "", 3f),
            new Speech("Ха-ха-ха! Вот ты и попался в мою ловушку!", "Злой компьютер", 4f),
            new Speech("Раз уж ты прошёл все мои задания, то и ума у тебя хоть отбавляй", "Злой компьютер", 5f),
            new Speech("То, что мне подходит", "Злой компьютер", 3f),
            new Speech("Я украду все твои знания!", "Злой компьютер", 4f)
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCamera1, speeches, $"Принесите ядро и выстрелите из пушки в компьютер (0/3)"));
        yield return new WaitForSeconds(2f);
        cutsceneLocation.SetActive(false);
        float delay = 0f;
        foreach (Speech speech in speeches) delay += speech.time;
        yield return new WaitForSeconds(delay - 1f);
        boss1.StartShooting();
    }

    private void Update()
    {
        playerHPSlider.value = Mathf.Lerp(playerHPSlider.value, playerHP, Time.deltaTime * 3);
        bossHPSlider.value = Mathf.Lerp(bossHPSlider.value, bossHP, Time.deltaTime * 3);
    }

    public IEnumerator ShowNextLevelDelay(float delay)
    {
        currentLevelNumber++;
        yield return new WaitForSeconds(delay);
        if (currentLevelNumber != levels.Length)
        {
            foreach (GameObject level in levels)
            {
                level.SetActive(false);
            }
            levels[currentLevelNumber].SetActive(true);
        }
    }


    //Второй уровень
    public IEnumerator EnterSecondLevel(float delay)
    {
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Ах ты проказник!", "Злой компьютер", 2.5f),
            new Speech("Ладно... Тебя ждёт ещё одно испытание", "Злой компьютер", 4f),
            new Speech("Проверим твоё умение быстро печатать!", "Злой компьютер", 4.5f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCamera2, speeches, $"Напишите 10 слов подряд без ошибок на скорость"));
        yield return new WaitForSeconds(2f);
        levels[1].SetActive(true);
        yield return new WaitForSeconds(10f);
        UIController.instance.isPaused = true;
        UIController.instance.isCutscene = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        level2UI.SetActive(true);
        levels[0].SetActive(false);
        NewWord();
    }

    private void NewWord()
    {
        inputWord.text = "";
        wordsCompletedText.text = $"{wordsInputedCount}/10";
        timer = TimerWord(5);
        StartCoroutine(timer);
        wordToInput = words[random.Next(0, words.Length - 1)].ToUpper().Trim();
        wordToInputText.text = wordToInput;
    }

    public void InputWord()
    {
        StopCoroutine(timer);
        if (inputWord.text.Trim().ToUpper() == wordToInput)
        {
            AudioManager.instance.PlaySound(2);
            wordsInputedCount++;
            wordsCompletedText.text = $"{wordsInputedCount}/10";
            if (wordsInputedCount >= 10) 
            {
                StartCoroutine(EnterThirdLevel());
                return;
            }
        }
        else
        {
            FailWord();
        }
        NewWord();
    }

    private void FailWord()
    {
        AudioManager.instance.PlaySound(3);
        wordsInputedCount = 0;
    }

    private IEnumerator timer;
    private IEnumerator TimerWord(int seconds)
    {
        timerText.text = seconds.ToString();
        for (int s = 0; s < seconds; s++)
        {
            timerText.text = (seconds - s).ToString();
            yield return new WaitForSeconds(1f);
        }
        timerText.text = "0";
        FailWord();
        NewWord();
    }


    //Третий уровень
    public IEnumerator EnterThirdLevel()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        levels[2].SetActive(true);
        List<Speech> speeches = new List<Speech>
        {
            new Speech("Ах!", "Злой компьютер", 2.5f),
            new Speech("Последний рубеж!", "Злой компьютер", 2.5f),
            new Speech("Пора тебя уничтожить!", "Злой компьютер", 4f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCamera3, speeches, $"Уничтожьте злой компьютер!"));
        yield return new WaitForSeconds(2f);
        level2UI.SetActive(false);
        yield return new WaitForSeconds(10f);
        levels[1].SetActive(false);
        player.SetActive(false);
        playerGun.SetActive(true);
        bossUI.SetActive(true);
        gunModel.SetActive(false);
        StartCoroutine(finalBoss.StartAttackingDelay(1f));
    }

    public void DamageBoss()
    {
        bossHP -= 30;
        finalBoss.Damage();
        if (bossHP <= 0 && !isKilled)
        {
            isKilled = true;
            StartCoroutine(EndLevel());
        }
    }

    private IEnumerator EndLevel()
    {
        finalBoss.Kill();
        yield return new WaitForSeconds(2f);
        List<Speech> speeches = new List<Speech>
        {
            new Speech("Ты меня уничтожил...", "Злой компьютер", 3f),
            new Speech("А теперь...", "Злой компьютер", 3f),
            new Speech("Пора просыпаться...", "Злой компьютер", 3f),
            new Speech("Проснись, студент", "Злой компьютер", 3f),
            new Speech("Проснись!", "Злой компьютер", 2.5f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraEnd, speeches, "", 12));
        bossUI.SetActive(false);
    }

    public void DamagePlayer(int hp)
    {
        playerHP -= hp;
        AudioManager.instance.PlaySound(4);
        if (playerHP <= 0)
        {
            StartCoroutine(Respawn());
        }
    }

    public IEnumerator Respawn()
    {
        UIController.instance.isPaused = true;
        UIController.instance.fadeToBlack = true;
        yield return new WaitForSeconds(1.5f);
        MoveController.instance.player.transform.position = respawnFinalBoss.position;
        playerHP = maxPlayerHp;
        bossHP = maxBossHp;
        StartCoroutine(CheckPositionLast());
    }

    private IEnumerator CheckPositionLast()
    {
        yield return new WaitForSeconds(0.5f);
        while(MoveController.instance.player.transform.position != respawnFinalBoss.transform.position) 
        {
            yield return null;
            MoveController.instance.player.transform.position = respawnFinalBoss.transform.position;
        }
        UIController.instance.fadeFromBlack = true;
        UIController.instance.isPaused = false;
    }

    private IEnumerator CheckPosition()
    {
        yield return new WaitForSeconds(0.5f);
        while(MoveController.instance.player.transform.position != respawn.transform.position) 
        {
            yield return null;
            MoveController.instance.player.transform.position = respawn.transform.position;
        }
        UIController.instance.fadeFromBlack = true;
        UIController.instance.isPaused = false;
    }


    public static GameManagerBoss instance;
    public void Awake()
    {
        instance = this;
    }
}
