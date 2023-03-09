using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HubManager : MonoBehaviour
{
    [SerializeField] private GameObject cutsceneCamera, bossDoor;
    public int levelCount;
    public TextMeshProUGUI levelEndedText;
    public Transform[] levelDoors;
    private List<bool> levelsEnded = new List<bool>();

    void Start()
    {
        for (int i = 3; i < levelCount + 3; i++)
        {
            levelsEnded.Add(PlayerPrefs.GetInt($"Level{i}", 0) != 0 ? true : false);
        }
        levelEndedText.text = $"{levelsEnded.Where(l => l).Count()}/{levelCount}";
        MoveController.instance.player.transform.SetPositionAndRotation(levelDoors[PlayerPrefs.GetInt("Last Level", 0)].position, levelDoors[PlayerPrefs.GetInt("Last Level", 0)].rotation);
        MoveController.instance.player.transform.Rotate(new Vector3(0, 270, 0));
        UIController.instance.fadeFromBlack = true;
        if (PlayerPrefs.GetInt("Hub Tip", 1) == 1)
        {
            UIController.instance.ShowTip();
            PlayerPrefs.SetInt("Hub Tip", 0);
        }
        if (levelsEnded.Where(l => l).Count() == levelCount)
        {
            UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
            List<Speech> speeches = new List<Speech>
            {
                new Speech("*Теперь пора в 313 кабинет*", "Вы", 5f),
            };
            StartCoroutine(GameManager.instance.Cutscene(cutsceneCamera, speeches, "Идите в 313 кабинет"));
            bossDoor.GetComponent<Animation>().Play();
        }
        else UIController.instance.NewObjective("Соберите все студенческие билеты из всех кабинетов");
    }
}
