using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackZone : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player") 
        {
            GameManagerLevelGhost.instance.DamagePlayer(10);
        }
    }
}
