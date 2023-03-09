using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCannon : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform shootPosition;
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private ParticleSystem shootEffect;
    [SerializeField] private float force = 15;
    public bool isWorking = true;
    public IEnumerator shooting;

    private void Start()
    {
        shooting = Shooting();
    }

    void Update()
    {
        if (isWorking) this.transform.LookAt(MoveController.instance.player.transform);
    }

    private IEnumerator Shooting()
    {
        while(true)
        {
            yield return new WaitForSeconds(4f);
            shootSound.Play();
            shootEffect.Play();
            var shootedBullet = Instantiate(bullet, shootPosition.position, shootPosition.rotation);
            shootedBullet.GetComponent<Rigidbody>().mass = 0.5f;
            shootedBullet.GetComponent<Rigidbody>().AddForce(shootPosition.forward * -1 * force, ForceMode.Impulse);
        }
    }

    public void Shoot()
    {
        shootSound.Play();
        shootEffect.Play();
        var shootedBullet = Instantiate(bullet, shootPosition.position, shootPosition.rotation);
        shootedBullet.GetComponent<Rigidbody>().mass = 0.5f;
        shootedBullet.GetComponent<Rigidbody>().AddForce(shootPosition.forward * -1 * force, ForceMode.Impulse);
    }
}
