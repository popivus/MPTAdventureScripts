using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapDrived : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player") GameManagerRace.instance.LapDrived();
    }
}
