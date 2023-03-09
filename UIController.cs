using System.Collections;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    public GameObject textObject, cornerText, cornerPanel, pausePanel, mainPanel, settingsPanel, audioSettings, controlSettings, setKeyPanel, hint, objective, mainUI, tipPanel;
    
    [Header("Панель бонуса")]
    public GameObject bonusPanel;
    public TextMeshProUGUI bonusText;

    [Header("Катсцена")]
    public TextMeshProUGUI speechText;
    public TextMeshProUGUI nameText;
    public GameObject cutsceneUI;
    [Space]

    private Image laptopImage;

    [Header("Кнопки управления")]
    public GameObject interactButton;
    public GameObject dropButton;
    public GameObject shootButton;
    public GameObject runButton;

    [Space]
    public Image controlsButton;
    public Image audioButton;
    public GameObject blackScreen;

    [Header("Слайдеры")]
    public Slider sensSlider;
    public Slider musicSlider;
    public Slider soundSlider;
    public Slider tipSlider;
    [Space]
    public bool isPaused = false, settingKey = false, pauseClosed = true, fadeToBlack = false, fadeFromBlack = false, isCutscene = false;
    private bool controlChange, bonusPanelDisappear, isTip;

    private IEnumerator coroutine;
    
    public void SetText(string text)
    {
        textObject.GetComponent<Text>().text = text;
        textObject.SetActive(true);
        textObject.transform.GetChild(0).GetComponent<Text>().text = text;
    }

    void Start()
    {
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
        laptopImage = bonusPanel.transform.GetChild(0).GetComponent<Image>();
    }

    void Update()
    {
        if (!isCutscene && (Input.GetKeyUp(KeyCode.Escape)) && !settingKey)
        {
            if (!settingsPanel.activeSelf)
            {
                SetPause(!isPaused);
            }
            else 
            {
                CloseSettings();
            }
        }
        if (bonusPanelDisappear)
        {
            laptopImage.color = new Color(laptopImage.color.r, laptopImage.color.g, laptopImage.color.b, Mathf.MoveTowards(laptopImage.color.a, 0f, Time.deltaTime));
            bonusText.color = new Color(bonusText.color.r, bonusText.color.g, bonusText.color.b, Mathf.MoveTowards(bonusText.color.a, 0f, Time.deltaTime));
            if (laptopImage.color.a == 0f) 
            {
                bonusPanel.SetActive(false);
                laptopImage.color = new Color(laptopImage.color.r, laptopImage.color.g, laptopImage.color.b, 1f);
                bonusText.color = new Color(bonusText.color.r, bonusText.color.g, bonusText.color.b, 1f);
                bonusPanelDisappear = false;
            }
        }
        if (fadeToBlack)
        {
            blackScreen.GetComponent<Image>().color = new Color(blackScreen.GetComponent<Image>().color.r, blackScreen.GetComponent<Image>().color.g, blackScreen.GetComponent<Image>().color.b, Mathf.MoveTowards(blackScreen.GetComponent<Image>().color.a, 1f, 2 * Time.deltaTime));
            if (blackScreen.GetComponent<Image>().color.a == 1f) fadeToBlack = false;
        }
        if (fadeFromBlack)
        {
            blackScreen.GetComponent<Image>().color = new Color(blackScreen.GetComponent<Image>().color.r, blackScreen.GetComponent<Image>().color.g, blackScreen.GetComponent<Image>().color.b, Mathf.MoveTowards(blackScreen.GetComponent<Image>().color.a, 0f, 2 * Time.deltaTime));
            if (blackScreen.GetComponent<Image>().color.a == 0f) fadeFromBlack = false;
        }
        if (isTip)
        {
            if (Input.GetKey(KeyCode.F)) tipSlider.value = Mathf.Lerp(tipSlider.value, 100, Time.deltaTime * 4);
            else tipSlider.value = Mathf.Lerp(tipSlider.value, 0, Time.deltaTime * 6);
            if (tipSlider.value > 99) 
            {
                isTip = false;
                tipPanel.SetActive(false);
                SetPause(false);
            }
        }
    }

    public void ShowTip()
    {
        isPaused = true;
        isTip = true;
        tipPanel.SetActive(true);
    }

    public void NewObjective(string obj)
    {
        objective.GetComponent<TextMeshProUGUI>().text = obj;
        objective.GetComponent<Animation>().Play();
    }

    public void Hint(string text)
    {
        hint.GetComponent<TextMeshProUGUI>().text = text;
        StartCoroutine(ShowHint());
    }

    private IEnumerator ShowHint()
    {
        hint.SetActive(true);
        yield return new WaitForSeconds(4f);
        hint.SetActive(false);
    }

    public void SetCornerText(string text)
    {
        cornerText.GetComponent<Text>().text = text;
        cornerPanel.SetActive(true);
    }

    //Меню паузы

    public void SetPause(bool pauseOrNot)
    {
        if (isTip) return;
        if (pauseOrNot) pauseClosed = false;
        Time.timeScale = pauseOrNot ? 0f : 1f;
        pausePanel.SetActive(pauseOrNot);
        if (!isCutscene) isPaused = pauseOrNot;
        Cursor.visible = pauseOrNot;
        Cursor.lockState = pauseOrNot ? CursorLockMode.None : CursorLockMode.Locked;
        if (pauseOrNot)
        {
            foreach (AudioSource sound in AudioManager.instance.sounds) sound.Stop();
        }
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
        if (MoveController.instance != null) MoveController.instance.sensitivity = sensSlider.value / 10;
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
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
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

    public void NewSpeech(string text, string name)
    {
        speechText.text = "";
        nameText.text = name;
        StartCoroutine(SpeechTextFilling(text));
    }

    private IEnumerator SpeechTextFilling(string text)
    {
        foreach(char ch in text)
        {
            speechText.text += ch;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void BonusCollected()
    {
        StartCoroutine(ShowBonusPanel());
    }

    private IEnumerator ShowBonusPanel()
    {
        bonusText.text = $"СОБРАНО {GameManager.instance.bonusesCollected} ИЗ {GameManager.instance.bonusAmount} СЕКРЕТНЫХ ЛЕКЦИЙ";
        bonusPanel.SetActive(true);
        bonusPanel.GetComponent<Animation>().Play();
        yield return new WaitForSeconds(3.5f);
        bonusPanelDisappear = true;
    }

    public static UIController instance;
    public void Awake()
    {
        instance = this;
    }

}
