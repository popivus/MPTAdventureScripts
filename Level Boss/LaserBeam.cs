using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            GameManagerBoss.instance.DamagePlayer(4);
        }
    }
}
