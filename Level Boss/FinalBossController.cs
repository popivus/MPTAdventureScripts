using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FinalBossController : MonoBehaviour
{
    [Header("Оружия")]
    [SerializeField] private EnemyCannon cannon;
    [SerializeField] private LaserController laser;
    [SerializeField] private GameObject wave;
    [Space]
    [SerializeField] private Transform waveStartPosition;
    [SerializeField] private GameObject upper;
    private NavMeshAgent agent;
    [SerializeField] private Transform[] points;
    private System.Random random;
    private System.Random randomAttack;
    private Transform newPoint;
    private bool isAttacking = false;
    [SerializeField] private AudioSource bossDamageSound, waveAttackSound;
    [SerializeField] private Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        random = new System.Random();
        randomAttack = new System.Random();
        newPoint = points[random.Next(0, points.Length - 1)];
        agent.isStopped = true;
    }

    public void Damage()
    {
        bossDamageSound.Play();
        animator.SetTrigger("Damaged");
    }

    private void Update()
    {
        agent.destination = newPoint.position;
        if (agent.remainingDistance <= 0.5f) newPoint = points[random.Next(0, points.Length - 1)];
        
        if (!agent.isStopped) upper.transform.LookAt(MoveController.instance.player.transform);

        if (!isAttacking && !agent.isStopped)
        {
            Attack();
        }
    }

    public IEnumerator StartAttackingDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        agent.isStopped = false;
    }

    public void Kill()
    {
        agent.isStopped = true;
        animator.SetBool("Is Dead", true);
        cannon.isWorking = false;
        laser.isWorking = false;
    }

    private void Attack()
    {
        isAttacking = true;
        switch (randomAttack.Next(3))
        {
            case 0: StartCoroutine(CannonAttack()); break;
            case 1: StartCoroutine(LaserAttack()); break;
            case 2: StartCoroutine(WaveAttack()); break;
        }
    }

    private IEnumerator CannonAttack()
    {
        cannon.Shoot();
        yield return new WaitForSeconds(0.6f);
        cannon.Shoot();
        yield return new WaitForSeconds(0.6f);
        cannon.Shoot();
        yield return new WaitForSeconds(1.5f);
        isAttacking = false;
    }

    private IEnumerator LaserAttack()
    {
        StartCoroutine(laser.Shoot());
        yield return new WaitForSeconds(1.1f);
        StartCoroutine(laser.Shoot());
        yield return new WaitForSeconds(1.1f);
        StartCoroutine(laser.Shoot());
        yield return new WaitForSeconds(1.1f);
        isAttacking = false;
    }

    private IEnumerator WaveAttack()
    {
        Debug.Log("Wave");
        waveAttackSound.Play();
        animator.SetTrigger("Wave");
        yield return new WaitForSeconds(1f);
        var newWave = Instantiate(wave, waveStartPosition.position, waveStartPosition.rotation);
        newWave.GetComponent<Rigidbody>().AddForce(waveStartPosition.forward * 5, ForceMode.Impulse);
        yield return new WaitForSeconds(1f);
        isAttacking = false;
    }
}
