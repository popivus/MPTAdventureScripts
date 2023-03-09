using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    [SerializeField] private GameObject effect;
    private bool isCollapsed = false;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isCollapsed || collision.tag == "Finish") return;
        if (collision.tag == "Player")
        {
            AudioManager.instance.PlaySound(4);
            StartCoroutine(DeathGround.instance.Respawn());
            MoveController.instance.DropProp();
            Destroy(gameObject, 1.6f);
            isCollapsed = true;
        }
        else Destroy(gameObject);
        Destroy(Instantiate(effect, transform.position, transform.rotation), 2f);
    }
}
