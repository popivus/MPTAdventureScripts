using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plunger : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Enemy")
        {
            collision.gameObject.GetComponent<GhostController>().Kill();
            Destroy(gameObject);            
        }
        else if (collision.transform.tag == "Boss")
        {
            GameManagerBoss.instance.DamageBoss();
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject.GetComponent<BoxCollider>());
            Destroy(gameObject.GetComponent<Rigidbody>());
            Destroy(gameObject, 1.5f);
        }
    }
}
