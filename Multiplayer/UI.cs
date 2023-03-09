using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using System.Collections;
using System.Xml.Serialization;
using Photon.Pun;
using System.Collections.Generic;

public class UI : MonoBehaviourPunCallbacks
{    
    public bool isPaused = false;
    [SerializeField] private GameObject pausePanel, mainPanel, settingsPanel, audioSettings, controlSettings, setKeyPanel;

    [Header("Кнопки управления")]
    [SerializeField] private GameObject interactButton;
    [SerializeField] private GameObject dropButton;
    [SerializeField] private GameObject shootButton;
    [SerializeField] private GameObject runButton;

    [Space]
    [SerializeField] private Image controlsButton;
    [SerializeField] private Image audioButton;

    [Header("Слайдеры")]
    [SerializeField] private Slider sensSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject blackScreen;
    [SerializeField] private GameObject nicknameText;
    [Space]

    [Header("Фид")]
    [SerializeField] private Transform feedParent;
    [SerializeField] private FeedItem feedItemPrefab;

    [Header("Таблица счёта")]
    [SerializeField] private GameObject scoreboardPanel;
    [SerializeField] private Transform tableParent;
    [SerializeField] private ScoreItem scoreItemPrefab;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Разминка")]
    [SerializeField] private GameObject warmupPanel;
    [SerializeField] private TextMeshProUGUI waitText;
    [SerializeField] private GameObject unready, ready;

    [Header("Экран получения урона")]
    [SerializeField] private Image[] screenComponents;
    private bool isDamageScreen = false;

    private Player player;

    public void SetCurrentPlayer(Player player)
    {
        this.player = player;
    }

    private bool fadeToBlack = false, fadeFromBlack = false;
    private bool settingKey = false;
    private IEnumerator coroutine;
    private IEnumerator coroutineDamageScreen;

    private void Start()
    {
        coroutineDamageScreen = ShowDamageScreen();
        sensSlider.value = PlayerPrefs.GetFloat("Sensitivity", 2) * 10;
        musicSlider.value = PlayerPrefs.GetFloat("Music", 0);
        soundSlider.value = PlayerPrefs.GetFloat("Sound", 0);
        XmlSerializer serializer = new XmlSerializer(typeof(Config));
        using (FileStream fs = new FileStream("config.xml", FileMode.OpenOrCreate))
        {
            Config conf = (Config)serializer.Deserialize(fs);
            ControlsController.interact = conf.interact;
            ControlsController.drop = conf.drop;
            ControlsController.shoot = conf.shoot;
            ControlsController.run = conf.run;
            interactButton.transform.GetChild(0).GetComponent<Text>().text = conf.interact.ToString();
            dropButton.transform.GetChild(0).GetComponent<Text>().text = conf.drop.ToString();
            shootButton.transform.GetChild(0).GetComponent<Text>().text = conf.shoot.ToString();
            runButton.transform.GetChild(0).GetComponent<Text>().text = conf.run.ToString();
        }
    }

    private void Update()
    {
        if (fadeToBlack)
        {
            blackScreen.GetComponent<Image>().color = new Color(blackScreen.GetComponent<Image>().color.r, blackScreen.GetComponent<Image>().color.g, blackScreen.GetComponent<Image>().color.b, Mathf.MoveTowards(blackScreen.GetComponent<Image>().color.a, 1f, Time.deltaTime));
            if (blackScreen.GetComponent<Image>().color.a == 1f) fadeToBlack = false;
        }
        if (fadeFromBlack)
        {
            blackScreen.GetComponent<Image>().color = new Color(blackScreen.GetComponent<Image>().color.r, blackScreen.GetComponent<Image>().color.g, blackScreen.GetComponent<Image>().color.b, Mathf.MoveTowards(blackScreen.GetComponent<Image>().color.a, 0f, Time.deltaTime));
            if (blackScreen.GetComponent<Image>().color.a == 0f) fadeFromBlack = false;
        }

        if (isDamageScreen)
        {
            foreach(Image component in screenComponents) 
                component.color = new Color(component.color.r, component.color.g, component.color.b, Mathf.Lerp(component.color.a, 1f, 2 * Time.deltaTime));
        }
        else
        {
            foreach(Image component in screenComponents) 
                component.color = new Color(component.color.r, component.color.g, component.color.b, Mathf.Lerp(component.color.a, 0f, 2 * Time.deltaTime));
        }

        if ((Input.GetKeyUp(KeyCode.Escape)) && !settingKey)
        {
            if (!settingsPanel.activeSelf)
            {
                SetPause(!isPaused);
                scoreboardPanel.SetActive(false);
            }
            else 
            {
                CloseSettings();
            }
        }

        if (MultiplayerManager.instance.isEnded) 
        {
            scoreboardPanel.SetActive(true);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab)) scoreboardPanel.SetActive(true);
        if (Input.GetKeyUp(KeyCode.Tab)) scoreboardPanel.SetActive(false);
    }

    public void DamageScreen()
    {
        StopCoroutine(coroutineDamageScreen);
        coroutineDamageScreen = ShowDamageScreen();
        StartCoroutine(coroutineDamageScreen);
    }

    public void ShowNickname(string nickname)
    {
        nicknameText.GetComponent<TextMeshProUGUI>().text = nickname;
        nicknameText.SetActive(true);
    }

    private IEnumerator ShowDamageScreen()
    {
        isDamageScreen = true;
        Debug.Log("Экран робитэ");
        yield return new WaitForSeconds(0.3f);
        isDamageScreen = false;
    }

    public void StartVote()
    {
        unready.SetActive(true);
    }

    public void StopVote()
    {
        unready.SetActive(false);
        ready.SetActive(false);
    }

    public void SetWaitText(string text)
    {
        waitText.text = text;
    }

    public void SetTimerText(int secondsLeft)
    {
        timerText.text = $"{secondsLeft / 60}:{((secondsLeft % 60).ToString().Length != 1 ? (secondsLeft % 60).ToString() : ("0" + (secondsLeft % 60).ToString()))}";
    }

    public void SetTimerText(string text)
    {
        timerText.text = text;
    }

    public void SetReady(bool ready)
    {
        this.ready.SetActive(ready);
        unready.SetActive(!ready);
    }

    public void StopWarmup()
    {
        ready.SetActive(false);
        unready.SetActive(false);
        StartCoroutine(TimerToStart());
    }

    private IEnumerator TimerToStart()
    {
        AudioManager.instance.PlaySound(0);
        waitText.text = "Начало отсчёта!";
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlaySound(1);
        waitText.text = "3";
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlaySound(1);
        waitText.text = "2";
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlaySound(1);
        waitText.text = "1";
        yield return new WaitForSeconds(1f);
        warmupPanel.SetActive(false);
    }

    public void RefreshScoreboard(List<Player> players)
    {
        foreach(Transform item in tableParent.GetComponentsInChildren(typeof(Transform), true)) if (tableParent != item) Destroy(item.gameObject);
        foreach(Player player in players)
        {
            ScoreItem scoreItem = Instantiate(scoreItemPrefab, tableParent);
            if (scoreItem != null) scoreItem.SetText(player.GetView().Owner.NickName, player.kills, player.deaths);
        }
    }

    public void Feed(string text)
    {
        FeedItem newFeedItem = Instantiate(feedItemPrefab, feedParent);
        if (newFeedItem != null) newFeedItem.SetText(text);
    }

    public void HideNickname()
    {
        nicknameText.SetActive(false);
    }

    public void SetHPValue(int value)
    {
        hpSlider.value = value;
    }

    public void EnableBlackScreen()
    {
        fadeFromBlack = false;
        blackScreen.GetComponent<Image>().color = new Color(blackScreen.GetComponent<Image>().color.r, blackScreen.GetComponent<Image>().color.g, blackScreen.GetComponent<Image>().color.b, 0f);
        fadeToBlack = true;
    }

    public void DisableBlackScreen()
    {
        fadeToBlack = false;
        blackScreen.GetComponent<Image>().color = new Color(blackScreen.GetComponent<Image>().color.r, blackScreen.GetComponent<Image>().color.g, blackScreen.GetComponent<Image>().color.b, 1f);
        fadeFromBlack = true;
    }

    private void SetPause(bool pause)
    {
        pausePanel.SetActive(pause);
        Cursor.visible = pause;
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        isPaused = pause;
    }

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        audioSettings.SetActive(false);
        controlSettings.SetActive(false);
        audioButton.color = new Color(0.3113208f, 0.3098039f, 0.3113208f);
        controlsButton.color = new Color(0.3113208f, 0.3098039f, 0.3113208f);
    }

    public void OpenAudio()
    {
        controlsButton.color = new Color(0.3113208f, 0.3098039f, 0.3113208f);
        audioButton.color = new Color(0.6235294f, 0.6235294f, 0.6235294f);
        controlSettings.SetActive(false);
        audioSettings.SetActive(true);
    }

    public void OpenControls()
    {
        controlsButton.color = new Color(0.6235294f, 0.6235294f, 0.6235294f);
        audioButton.color = new Color(0.3113208f, 0.3098039f, 0.3113208f);
        controlSettings.SetActive(true);
        audioSettings.SetActive(false);
    }

    public void ChangeSens()
    {
        if (player != null) player.sensitivity = sensSlider.value / 10;
        PlayerPrefs.SetFloat("Sensitivity", sensSlider.value / 10);
    }

    public void ChangeMusic()
    {
        AudioManager.instance.SetMusicLevel(musicSlider.value);
        PlayerPrefs.SetFloat("Music", musicSlider.value);
    }

    public void ChangeSound()
    {
        AudioManager.instance.SetSoundLevel(soundSlider.value);
        PlayerPrefs.SetFloat("Sound", soundSlider.value);
    }

    public void ExitLevel()
    {
        MultiplayerManager.instance.LeaveRoom();
    }

    public void Resume()
    {
        SetPause(false);
    }

    public void SetInteractKey()
    {
        setKeyPanel.SetActive(true);
        coroutine = WaitForKey(0);
        StartCoroutine(coroutine);
    }

    public void SetDropKey()
    {
        setKeyPanel.SetActive(true);
        coroutine = WaitForKey(1);
        StartCoroutine(coroutine);
    }

    public void SetShootKey()
    {
        setKeyPanel.SetActive(true);
        coroutine = WaitForKey(2);
        StartCoroutine(coroutine);
    }

    public void SetRunKey()
    {
        setKeyPanel.SetActive(true);
        coroutine = WaitForKey(3);
        StartCoroutine(coroutine);
    }

    private IEnumerator WaitForKey(int keyNumber)
    {
        settingKey = true;
        while(true)
		{
			yield return null;
			if(Input.GetKeyUp(KeyCode.Escape))
			{
				setKeyPanel.SetActive(false);
                settingKey = false;
				StopCoroutine(coroutine);
			}

			foreach(KeyCode k in KeyCode.GetValues(typeof(KeyCode)))
			{
				if(Input.GetKeyDown(k) && !Input.GetKeyDown(KeyCode.Escape))
				{
                    switch(keyNumber)
                    {
                        case 0:
                        {
                            ControlsController.interact = k;
                            interactButton.transform.GetChild(0).GetComponent<Text>().text = k.ToString();
                            break;
                        }
                        case 1:
                        {
                            ControlsController.drop = k;
                            dropButton.transform.GetChild(0).GetComponent<Text>().text = k.ToString();
                            break;
                        }
                        case 2:
                        {
                            ControlsController.shoot = k;
                            shootButton.transform.GetChild(0).GetComponent<Text>().text = k.ToString();
                            break;
                        }
                        case 3:
                        {
                            ControlsController.run = k;
                            runButton.transform.GetChild(0).GetComponent<Text>().text = k.ToString();
                            break;
                        }
                    }
                    setKeyPanel.SetActive(false);
                    settingKey = false;
                    Config conf = new Config(ControlsController.interact, ControlsController.drop, ControlsController.shoot, ControlsController.run);
                    XmlSerializer serializer = new XmlSerializer(typeof(Config));
                    using (FileStream fs = new FileStream("config.xml", FileMode.Create)) serializer.Serialize(fs, conf);
					StopCoroutine(coroutine);
				}
			}
		}
    }

    public static UI instance;
    public void Awake()
    {
        instance = this;
    }
}
