using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && GameManagerLevel1.instance.objectiveDone) GameManagerLevel1.instance.EndLevel();
    }
}
