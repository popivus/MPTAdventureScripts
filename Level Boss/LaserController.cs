using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [SerializeField] private AudioSource shootSound;
    [SerializeField] private GameObject aimLaser, laser;
    [SerializeField] private Transform shootTarget;
    private bool isShooting = false;
    public bool isWorking = true;

    private void Update()
    {
        if (!isWorking) return;

        if (!isShooting) shootTarget.SetPositionAndRotation(MoveController.instance.player.transform.position + MoveController.instance.GetMoveDirection(), MoveController.instance.player.transform.rotation);
        this.transform.LookAt(shootTarget);
    }

    public IEnumerator Shoot()
    {
        aimLaser.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        isShooting = true;
        yield return new WaitForSeconds(0.2f);
        shootSound.Play();
        aimLaser.SetActive(false);
        laser.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        laser.SetActive(false);
        isShooting = false;
    }
}
