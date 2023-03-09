using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSee : MonoBehaviour
{
    private GameObject player;
    private float distance;

    void Start()
    {
        player = MoveController.instance.cam;
        distance = MoveController.instance.distanceToPickUp;
    }

    void OnMouseEnter()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= distance && tag != "Untagged") gameObject.GetComponent<Outline>().OutlineWidth = 3;
    }

    void OnMouseOver()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= distance && tag != "Untagged") gameObject.GetComponent<Outline>().OutlineWidth = 3;
        else gameObject.GetComponent<Outline>().OutlineWidth = 0;
    }

    void OnMouseExit()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= distance) gameObject.GetComponent<Outline>().OutlineWidth = 0;
    }
}
