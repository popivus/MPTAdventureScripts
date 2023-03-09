using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basket : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Bottle") GameManagerBasket.instance.NewHit();
        Destroy(this);
    }
}
