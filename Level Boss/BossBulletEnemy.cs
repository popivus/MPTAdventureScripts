using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletEnemy : MonoBehaviour
{
    [SerializeField] private GameObject effect;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            GameManagerBoss.instance.DamagePlayer(6);
        }
        if (collision.tag == "Map")
        {
            Destroy(Instantiate(effect, transform.position, transform.rotation), 2f);
            Destroy(gameObject);
        }
    }
}
