using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Boss1Controller : MonoBehaviour
{
    [SerializeField] private Animator bossAnimator;
    [SerializeField] private GameObject damageEffect;
    [SerializeField] private AudioSource damageSound;
    [SerializeField] private Animation mouseAnimation;
    [SerializeField] private EnemyCannon enemyCannon;
    private int hp = 3;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Lifted")
        {
            Damage();
            Destroy(collider.gameObject);
            damageSound.Play();
            var newEffect = Instantiate(damageEffect, collider.transform.position, collider.transform.rotation);
            Destroy(newEffect, 3f);
            bossAnimator.SetTrigger("Is Damaged");
            UIController.instance.objective.GetComponent<TextMeshProUGUI>().text = $"Принесите ядро и выстрелите из пушки в компьютер ({3 - hp}/3)";
        }
    }

    public void StartShooting()
    {
        StartCoroutine(enemyCannon.shooting);
    }

    private void Damage()
    {
        hp--;
        if (hp == 2) mouseAnimation.Play();
        if (hp <= 0)
        {
            StopCoroutine(enemyCannon.shooting);
            StartCoroutine(GameManagerBoss.instance.EnterSecondLevel(1f));
        }
    }
}
