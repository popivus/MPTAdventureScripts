using System.Collections;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUIController : MonoBehaviour
{
    public GameObject pausePanel, mainPanel, settingsPanel, audioSettings, controlSettings, setKeyPanel, continueButton;

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
    [Space]
    public bool settingKey = false;
    private bool controlChange;
    private bool fadeToBlack = false, fadeFromBlack = false;

    public float fadeSpeed = 1f;

    private IEnumerator coroutine;
    
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        sensSlider.value = PlayerPrefs.GetFloat("Sensitivity", 2) * 10;
        musicSlider.value = PlayerPrefs.GetFloat("Music", 0);
        soundSlider.value = PlayerPrefs.GetFloat("Sound", 0);
        XmlSerializer deserializer = new XmlSerializer(typeof(Config));
        try 
        {
            using (FileStream fs = new FileStream("config.xml", FileMode.OpenOrCreate))
            {
                Config conf = (Config)deserializer.Deserialize(fs);
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
        catch
        {
            ControlsController.interact = KeyCode.E;
            ControlsController.drop = KeyCode.G;
            ControlsController.shoot = KeyCode.Mouse0;
            ControlsController.run = KeyCode.LeftShift;
            Config conf = new Config(ControlsController.interact, ControlsController.drop, ControlsController.shoot, ControlsController.run);
            interactButton.transform.GetChild(0).GetComponent<Text>().text = conf.interact.ToString();
            dropButton.transform.GetChild(0).GetComponent<Text>().text = conf.drop.ToString();
            shootButton.transform.GetChild(0).GetComponent<Text>().text = conf.shoot.ToString();
            runButton.transform.GetChild(0).GetComponent<Text>().text = conf.run.ToString();
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            using (FileStream fs = new FileStream("config.xml", FileMode.Create)) serializer.Serialize(fs, conf);
        }
        if (PlayerPrefs.GetInt("New Game", 1) == 1) continueButton.SetActive(false);
        AudioManager.instance.PlayMusic(0);
    }

    void Update()
    {
        if (fadeToBlack)
        {
            blackScreen.GetComponent<Image>().color = new Color(blackScreen.GetComponent<Image>().color.r, blackScreen.GetComponent<Image>().color.g, blackScreen.GetComponent<Image>().color.b, Mathf.MoveTowards(blackScreen.GetComponent<Image>().color.a, 1f, fadeSpeed * Time.deltaTime));
            if (blackScreen.GetComponent<Image>().color.a == 1f) fadeToBlack = false;
        }
        if (fadeFromBlack)
        {
            blackScreen.GetComponent<Image>().color = new Color(blackScreen.GetComponent<Image>().color.r, blackScreen.GetComponent<Image>().color.g, blackScreen.GetComponent<Image>().color.b, Mathf.MoveTowards(blackScreen.GetComponent<Image>().color.a, 0f, fadeSpeed * Time.deltaTime));
            if (blackScreen.GetComponent<Image>().color.a == 0f) fadeFromBlack = false;
        }
    }

    //Меню паузы

    public void OpenSettings()
    {
        mainPanel.SetActive(false);
        settingsPanel.SetActive(true);
        pausePanel.SetActive(true);
    }

    public void CloseSettings()
    {
        mainPanel.SetActive(true);
        settingsPanel.SetActive(false);
        audioSettings.SetActive(false);
        controlSettings.SetActive(false);
        audioButton.color = new Color(0.3113208f, 0.3098039f, 0.3113208f);
        controlsButton.color = new Color(0.3113208f, 0.3098039f, 0.3113208f);
        pausePanel.SetActive(false);
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

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Resume()
    {
        StartCoroutine(StartGame(2));
    }
    
    public void NewGame()
    {
        for (int i = 3; i < 11; i++) PlayerPrefs.SetInt($"Level{i}", 0);
        for (int i = 0; i < 9; i++) PlayerPrefs.SetInt($"bonus{i}", 0);
        PlayerPrefs.SetInt("New Game", 1);
        PlayerPrefs.SetInt("Hub Tip", 1);
        PlayerPrefs.SetInt("Last Level", 0);
        StartCoroutine(StartGame(1));
    }

    private IEnumerator StartGame(int levelNumber)
    {
        blackScreen.SetActive(true);
        fadeToBlack = true;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(levelNumber);
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


    public static MainUIController instance;
    public void Awake()
    {
        instance = this;
    }
}
