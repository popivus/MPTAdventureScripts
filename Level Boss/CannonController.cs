using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private AudioSource fireSound, shootSound;
    [SerializeField] private ParticleSystem fireEffect, shootEffect;
    [SerializeField] private Animation cannonAnimation;

    private IEnumerator Shoot()
    {
        fireSound.Play();
        fireEffect.Play();
        yield return new WaitForSeconds(1.5f);
        cannonAnimation.Play();
        shootSound.Play();
        shootEffect.Play();
        var shootedBullet = Instantiate(bullet, shootPosition.position, shootPosition.rotation);
        shootedBullet.GetComponent<Rigidbody>().mass = 0.5f;
        shootedBullet.GetComponent<Rigidbody>().AddForce(shootPosition.forward * -15, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Lifted")
        {
            MoveController.instance.DropProp();
            Destroy(collider.gameObject);
            StartCoroutine(Shoot());
        }
    }
}
