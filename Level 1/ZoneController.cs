using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "StudentID") 
        {
            GameManagerLevel1.instance.StudentIDPlus();
            collider.GetComponent<Outline>().OutlineWidth = 0;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.name == "StudentID")
        {
            GameManagerLevel1.instance.StudentIDMinus();
            collider.GetComponent<Outline>().OutlineWidth = 3;
        }
    }
}
