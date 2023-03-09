using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject respawnPoint;
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player") respawnPoint.transform.position = this.transform.position;
    }
}
