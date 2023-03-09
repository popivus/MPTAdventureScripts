using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Bottle")
        {
            GameManagerBasket.instance.FailHit();
        }
    }
}
