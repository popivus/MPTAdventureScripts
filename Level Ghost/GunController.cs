using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    private bool delayed = false;
    public GameObject gun, plunger, cam;

    void Update()
    {
        if (UIController.instance.isPaused) return;

        if (Input.GetKeyUp(ControlsController.shoot) && !delayed)
        {
            StartCoroutine(ShootDelay());
            AudioManager.instance.PlaySound(1);
            gun.GetComponent<Animation>().Play();
            var newPlunger = Instantiate(plunger, cam.transform.position, cam.transform.rotation);
            newPlunger.transform.Rotate(new Vector3(-90, 0, 0));
            newPlunger.GetComponent<Rigidbody>().AddForce(cam.transform.forward * 15, ForceMode.Impulse);
        }
    }

    private IEnumerator ShootDelay()
    {
        delayed = true;
        yield return new WaitForSeconds(0.5f);
        delayed = false;
    }
}
