using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCollect : MonoBehaviour
{
    public int bonusNumber;

    private GameObject screen;

    void Start()
    {
        screen = this.transform.GetChild(0).gameObject;
        if (PlayerPrefs.GetInt($"bonus{bonusNumber}", 0) == 1)
        {
            screen.SetActive(false);
            this.tag = "Untagged";
        }
    }

    //Сбор бонусного предмета
    public void Collect()
    {
        screen.SetActive(false);
        this.tag = "Untagged";
        GameManager.instance.BonusCollected(bonusNumber);
    }
}
