using System.Collections;
using UnityEngine;

public class Fired : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Delete());
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
}
