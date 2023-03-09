using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTubesController : MonoBehaviour
{
    public List<GameObject> tubeObjects;
    private bool won = true;

    void Update()
    {
        bool isWin = true;
        foreach(GameObject tubeObject in tubeObjects)
        {
            var tube = tubeObject.GetComponent<TubeController>();
            if (tube.isStraight)
            {
                if (tube.rightState != tube.state && tube.rightState + 2 != tube.state) isWin = false;
            }
            else 
            {
                if (tube.rightState != tube.state) isWin = false;
            }
        }
        if (isWin && won) 
        {
            won = false;
            StartCoroutine(Win());
        }
    }

    private IEnumerator Win()
    {
        AudioManager.instance.PlaySound(1);
        yield return new WaitForSeconds(1.5f);
        GameManagerTubes.instance.GridTubesWon();
        while (gameObject.transform.position.y != -4.5f)
        {
            yield return null;
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, new Vector3(gameObject.transform.position.x, -4.5f, gameObject.transform.position.z), Time.deltaTime);
        }
    }
}
