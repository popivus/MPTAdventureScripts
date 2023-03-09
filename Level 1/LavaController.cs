using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaController : MonoBehaviour
{
    public GameObject respawn, respawnID, firedSound;
    public void OnTriggerEnter(Collider collider)
    {
        switch (collider.tag)
        {
            case "Player":
                AudioManager.instance.PlaySound(1);
                StartCoroutine(Respawn());
                break;
            case "Lifted":
                Instantiate(firedSound, collider.transform.position, new Quaternion()).GetComponent<AudioSource>().Play();
                if (collider.name == "StudentID") 
                {
                    collider.transform.position = respawnID.transform.position;
                    collider.GetComponent<Rigidbody>().Sleep();
                    UIController.instance.Hint("Студенческий билет вернулся обратно на стол");
                }
                else StartCoroutine(Fire(collider));
                break;
        }
    }

    private IEnumerator Fire(Collider collider)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(collider.gameObject);
    }

    private IEnumerator Respawn()
    {
        UIController.instance.isPaused = true;
        UIController.instance.fadeToBlack = true;
        yield return new WaitForSeconds(1.5f);
        MoveController.instance.player.transform.position = respawn.transform.position;
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
}
