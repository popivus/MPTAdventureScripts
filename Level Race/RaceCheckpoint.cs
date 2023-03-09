using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceCheckpoint : MonoBehaviour
{
    [SerializeField] private int checkpointNumber;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player") GameManagerRace.instance.CheckpointPassed(checkpointNumber);
    }
}
