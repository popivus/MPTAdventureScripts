using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeController : MonoBehaviour
{
    public int state = 0, rightState = 0;
    public bool isStraight = false;

    private float angle = 0;
    
    void Update()
    {
        switch (state)
        {
            case 0: angle = 0f; break;
            case 1: angle = 90f; break;
            case 2: angle = 180f; break;
            case 3: angle = 270f; break;
        }
        transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, new Vector3(0, 0, angle), Time.deltaTime / 0.2f);
    }

    public void Turn()
    {
        if (state != 3) state++;
        else state = 0;
    }
}
