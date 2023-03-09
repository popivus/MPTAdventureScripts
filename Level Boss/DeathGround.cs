using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathGround : MonoBehaviour
{
    private Transform respawn;

    private void Start()
    {
        respawn = GameManagerBoss.instance.respawn;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            StartCoroutine(Respawn());
            MoveController.instance.DropProp();
        }
        if (collider.tag == "Lifted")
        {
            Destroy(collider.gameObject, 4f);
        }
    }

    public IEnumerator Respawn()
    {
        UIController.instance.isPaused = true;
        UIController.instance.fadeToBlack = true;
        yield return new WaitForSeconds(1.5f);
        MoveController.instance.player.transform.position = respawn.position;
        StartCoroutine(CheckPosition());
    }

    private IEnumerator CheckPosition()
    {
        yield return new WaitForSeconds(0.5f);
        while(MoveController.instance.player.transform.position != respawn.transform.position) 
        {
            yield return null;
            MoveController.instance.player.transform.position = respawn.transform.position;
        }
        UIController.instance.fadeFromBlack = true;
        UIController.instance.isPaused = false;
    }


    public static DeathGround instance;
    void Awake()
    {
        instance = this;
    }
}
