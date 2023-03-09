using UnityEngine;
using TMPro;

public class ScoreItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nicknameText, killsText, deathsText;

    public void SetText(string nickname, int killsCount, int deathsCount)
    {
        nicknameText.text = nickname;
        killsText.text = killsCount.ToString();
        deathsText.text = deathsCount.ToString();
    }
}
