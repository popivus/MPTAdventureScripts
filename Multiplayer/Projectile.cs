using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.transform.tag)
        {
            case "Player":
            {
                if (collision.gameObject.GetComponent<PhotonView>().ViewID.ToString() == gameObject.name) return;

                Destroy(gameObject);
                if (!MultiplayerManager.instance.isWarmup) collision.gameObject.GetComponent<Player>().MakeDamage(20, int.Parse(gameObject.name));
                Debug.Log("Попал " + gameObject.name);
                break;
            }
            case "Bullets": break;
            default:
            {
                Destroy(gameObject.GetComponent<BoxCollider>());
                Destroy(gameObject.GetComponent<Rigidbody>());
                Destroy(gameObject, 1.5f);
                break;
            }
        }
    }
}
