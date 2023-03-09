using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCamera : MonoBehaviour
{
    [SerializeField] private Transform boss;
    void Update()
    {
        this.transform.LookAt(boss);
    }
}
