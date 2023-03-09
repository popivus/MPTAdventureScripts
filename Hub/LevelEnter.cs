using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnter : MonoBehaviour
{
    public int levelNumber;
    public bool isCompleted;
    public GameObject levelCompletedMark, levelUncompletedMark;
    void Start()
    {
        isCompleted = PlayerPrefs.GetInt($"Level{levelNumber}", 0) == 0 ? false : true;
        if (isCompleted) 
        {
            gameObject.GetComponent<Outline>().OutlineColor = Color.green;
            levelCompletedMark.SetActive(true);
        }
        else levelUncompletedMark.SetActive(true);
    }
}
