using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XOPlate : MonoBehaviour
{
    public int number;
    public string Content = "";

    public GameObject X, O;

    void Start()
    {
        X = transform.Find("X").gameObject;
        O = transform.Find("O").gameObject;
    }

    public void PlaceX()
    {
        transform.parent.GetComponent<XOController>().Play(number);
    }
}
