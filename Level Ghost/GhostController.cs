using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostController : MonoBehaviour
{
    public GameObject deathEffect, attackZone;
    private GameObject player;
    private NavMeshAgent agent;
    private Animator anim;
    private bool isAttacking = false;

    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        player = MoveController.instance.player;
        agent.destination = player.transform.position;
    }
    
    void Update()
    {
        player = MoveController.instance.player;
        agent.destination = player.transform.position;
        if (agent.remainingDistance <= 1.5f)
        {
            if (!isAttacking) StartCoroutine(Attack());
        }
        else if (!isAttacking)
        {
            agent.isStopped = false;
            anim.SetBool("Is Attack", false);
        }
    }

    private IEnumerator Attack()
    {
        agent.isStopped = true;
        anim.SetBool("Is Attack", true);
        isAttacking = true;
        yield return new WaitForSeconds(0.5f);
        attackZone.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attackZone.SetActive(false);
        yield return new WaitForSeconds(0.7f);
        isAttacking = false;
    }

    public void Kill()
    {
        GameManagerLevelGhost.instance.GhostKilled();
        Destroy(gameObject);
        Destroy(Instantiate(deathEffect, transform.position + new Vector3(0, 1.3f, 0), transform.rotation), 1f);
    }
}
