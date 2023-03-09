using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screamer : MonoBehaviour
{
    [SerializeField] private GameObject ghost, sound;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            ghost.SetActive(true);
            ghost.GetComponent<Animation>().Play();
            sound.GetComponent<AudioSource>().Play();
            Destroy(ghost, 0.5f);
            Destroy(sound, 1.5f);
            Destroy(gameObject);
        }
    }
}
