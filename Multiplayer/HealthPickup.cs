using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private GameObject model;
    private bool IsActive => model.activeSelf;
    public int number;

    public GameObject HealModel => model;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && IsActive)
        {
            if (collider.gameObject.GetComponent<Player>().HP < 100)
            {
                collider.gameObject.GetComponent<Player>().Heal(35, number);
            }
        }
    }
}
