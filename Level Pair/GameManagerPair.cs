using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerPair : MonoBehaviour
{
    public GameObject cutsceneCameraStart, cutsceneCameraEnd;
    private GameObject firstCard, secondCard;
    private bool firstCardTurned = false, cardsTurning = false;
    public Transform placeForCards;
    public int pairsCount;
    private int pairsDoneCount = 0;
    
    void Start()
    {
        UIController.instance.blackScreen.GetComponent<Image>().color = new Color(UIController.instance.blackScreen.GetComponent<Image>().color.r, UIController.instance.blackScreen.GetComponent<Image>().color.g, UIController.instance.blackScreen.GetComponent<Image>().color.b, 1f);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Я уже не могу", "Преподаватель", 2.5f),
            new Speech("Студент, помоги мне с этими дискетами", "Преподаватель", 4f),
            new Speech("Надо разобрать их по парам", "Преподаватель", 3.25f),
            new Speech("Все, где есть одинаковые картинки, клади мне на стол", "Преподаватель", 5f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraStart, speeches, "Разберите все дискеты по парам"));
    }


    public IEnumerator SetCard(GameObject card)
    {
        if (!cardsTurning)
        {
            cardsTurning = true;
            if (!firstCardTurned)
            {
                firstCard = card;
                firstCard.GetComponent<Animation>().Play("Card Turn");
                firstCardTurned = true;
            }
            else
            {
                if (firstCard != card)
                {
                    secondCard = card;
                    secondCard.GetComponent<Animation>().Play("Card Turn");
                    yield return new WaitForSeconds(1.5f);
                    if (firstCard.GetComponent<CardController>().numberOfCoupleOfCards == secondCard.GetComponent<CardController>().numberOfCoupleOfCards)
                    {
                        firstCard.tag = "Untagged";
                        secondCard.tag = "Untagged";
                        StartCoroutine(MoveCardOnTable(firstCard, placeForCards.position));
                        StartCoroutine(MoveCardOnTable(secondCard, placeForCards.position + new Vector3(0, 0.045f, 0)));
                        placeForCards.position += new Vector3(0, 0.09f, 0);
                        pairsDoneCount++;
                        if (pairsDoneCount == pairsCount) EndLevel();
                    }
                    else
                    {
                        firstCard.GetComponent<Animation>().Play("Card Unturn");
                        secondCard.GetComponent<Animation>().Play("Card Unturn");
                    }
                    firstCardTurned = false;
                }
            }
            cardsTurning = false;
        }
    }

    public void EndLevel()
    {
        PlayerPrefs.SetInt("Level4", 1);
        PlayerPrefs.SetInt("Last Level", 2);
        List<Speech> speeches = new List<Speech> 
        {
            new Speech("Молодец", "Преподаватель", 1.5f),
            new Speech("Чего ты там хотел, студенческий билет?", "Преподаватель", 4f),
            new Speech("Бери на здоровье", "Преподаватель", 3.25f),
        };
        StartCoroutine(GameManager.instance.Cutscene(cutsceneCameraEnd, speeches, "", 2));
    }

    private IEnumerator MoveCardOnTable(GameObject card, Vector3 position)
    {
        GameObject cardToMove = card;
        Vector3 newPosition = new Vector3();
        newPosition.z = position.z;
        newPosition.x = position.x;
        newPosition.y = position.y;
        while (cardToMove.transform.position != newPosition)
        {
            yield return null;
            cardToMove.transform.position = Vector3.Lerp(cardToMove.transform.position, newPosition, Time.deltaTime * 5);
        }
    }

    public static GameManagerPair instance;
    public void Awake()
    {
        instance = this;
    }
}
