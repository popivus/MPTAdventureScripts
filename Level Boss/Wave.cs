using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            GameManagerBoss.instance.DamagePlayer(12);
        }
    }

}
