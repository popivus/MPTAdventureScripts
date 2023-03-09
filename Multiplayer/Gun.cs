using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject plungerPrefab;
    private bool delayed = false;
    [SerializeField] private GameObject gun, shootPlace;
    [SerializeField] private Player player;
    [SerializeField] private AudioSource shootSound;

    void Update()
    {
        if (!player.GetView().IsMine || player.IsDead || UI.instance.isPaused || MultiplayerManager.instance.isTimer) return;

        if (Input.GetKeyUp(ControlsController.shoot) && !delayed)
        {
            player.GetView().RPC("Shoot", RpcTarget.All);
        }
    }

    [PunRPC]
    private void Shoot()
    {
        StartCoroutine(ShootDelay());
        if (player.GetView().IsMine) gun.GetComponent<Animation>().Play();
        var newPlunger = Instantiate(plungerPrefab, shootPlace.transform.position, shootPlace.transform.rotation);
        newPlunger.name = player.GetView().ViewID.ToString();
        newPlunger.GetComponent<Rigidbody>().AddForce(shootPlace.transform.forward * 15, ForceMode.Impulse);
        shootSound.Play();
    }

    private IEnumerator ShootDelay()
    {
        delayed = true;
        yield return new WaitForSeconds(0.5f);
        delayed = false;
    }
}
