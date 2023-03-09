using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class XOController : MonoBehaviour
{
    public List<XOPlate> plates;
    public GameObject winText;

    private string[] winCombinations = new string[]
    {
        "012", "345", "678", "036", "147", "258", "048", "246"
    };

    public void Play(int plateNumber)
    {
        bool changeTurn = true;
        plates[plateNumber].tag = "Untagged";
        plates[plateNumber].Content = "X";
        plates[plateNumber].X.SetActive(true);
        int voidAmount = 0;
        for (int i = 0; i < plates.Count; i++) if (Convert.ToString(plates[i].Content) != "") voidAmount++;
        if (voidAmount == plates.Count)
        {
            winText.GetComponent<TextMeshPro>().text = "Ничья!";
            winText.SetActive(true);
            StartCoroutine(Restart());
        }
        for (int i = 0; i < winCombinations.Length; i++) winFinding(winCombinations, i);
        if (winText.GetComponent<TextMeshPro>().text != "") changeTurn = false;
        for (int i = 0; i < winCombinations.Length; i++) winningBot(winCombinations, i, ref changeTurn);
        for (int i = 0; i < winCombinations.Length; i++) defendBot(winCombinations, i, ref changeTurn);
        for (int i = 0; i < winCombinations.Length; i++) attackBot(winCombinations, i, ref changeTurn);
        if (changeTurn)
        {
            string variantsO = "";
            for (int i = 0; i < plates.Count; i++) if (Convert.ToString(plates[i].Content) == "") variantsO += i;
            System.Random variant = new System.Random();
            int b = Convert.ToInt32(Convert.ToString(variantsO[variant.Next(0, variantsO.Length - 1)]));
            plates[b].Content = "O";
            plates[b].tag = "Untagged";
            plates[b].O.SetActive(true);
            changeTurn = false;
        }
        if (winText.GetComponent<TextMeshPro>().text == "") for (int i = 0; i < winCombinations.Length; i++) winFinding(winCombinations, i);
    }

    private void winFinding(string[] combinations, int combinationNumber)
    {
        if ((Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "X") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "X") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == "X"))
        {
            for (int i = 0; i < plates.Count; i++)
            {
                plates[i].tag = "Untagged";
            }
            winText.GetComponent<TextMeshPro>().text = "Вы победили!";
            winText.SetActive(true);
            Win();
        }
        if ((Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "O") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "O") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == "O"))
        {
            for (int i = 0; i < plates.Count; i++)
            {
                plates[i].tag = "Untagged";
            }
            winText.GetComponent<TextMeshPro>().text = "Победа O!";
            winText.SetActive(true);
            StartCoroutine(Restart());
        }
    }

    private void Win()
    {
        AudioManager.instance.PlaySound(1);
        GameManagerXO.instance.XOWon();
        Destroy(gameObject, 3f);
    }

    private IEnumerator Restart()
    {
        yield return new WaitForSeconds(3.5f);
        foreach (XOPlate plate in plates)
        {
            plate.tag = "XO Plate";
            plate.Content = "";
            plate.X.SetActive(false);
            plate.O.SetActive(false);
            winText.GetComponent<TextMeshPro>().text = "";
            winText.SetActive(false);
        }
    }

    private void defendBot(string[] combinations, int combinationNumber, ref bool turn)
    {
        if (turn && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "X") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "X") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == ""))
        {
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content = "O";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].tag = "Untagged";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].O.SetActive(true);
            turn = false;
        }
        if (turn && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "X") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == "X"))
        {
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content = "O";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].tag = "Untagged";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].O.SetActive(true);
            turn = false;
        }
        if (turn && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "X") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == "X"))
        {
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content = "O";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].tag = "Untagged";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].O.SetActive(true);
            turn = false;
        }
    }

    private void winningBot(string[] combinations, int combinationNumber, ref bool turn)
    {
        if (turn && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "O") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "O") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == ""))
        {
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content = "O";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].tag = "Untagged";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].O.SetActive(true);
            turn = false;
        }
        if (turn && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "O") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == "O"))
        {
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content = "O";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].tag = "Untagged";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].O.SetActive(true);
            turn = false;
        }
        if (turn && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "O") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == "O"))
        {
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content = "O";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].tag = "Untagged";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].O.SetActive(true);
            turn = false;
        }
    }

    private void attackBot(string[] combinations, int combinationNumber, ref bool turn)
    {
        if (turn && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "O") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == ""))
        {
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content = "O";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].tag = "Untagged";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].O.SetActive(true);
            turn = false;
        }
        if (turn && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == "O"))
        {
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content = "O";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].tag = "Untagged";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].O.SetActive(true);
            turn = false;
        }
        if (turn && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content) == "") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][1]))].Content) == "O") && (Convert.ToString(plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][2]))].Content) == ""))
        {
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].Content = "O";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].tag = "Untagged";
            plates[Convert.ToInt32(Convert.ToString(combinations[combinationNumber][0]))].O.SetActive(true);
            turn = false;
        }
    }
}
