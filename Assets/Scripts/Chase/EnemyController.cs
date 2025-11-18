using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float speedWalk;
    private Animator animator;
    private bool CanWalk;
    private NavMeshAgent navMeshAgent;
    [HideInInspector] public Transform player;
    public AudioSource stabAudioSource;
    public AudioClip stabAudio;
    private BoxCollider collider;
    private AudioSource scaryAudio;

    void Start()
    {
        StartCoroutine(Wait());
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        stabAudioSource = GetComponent<AudioSource>();
        scaryAudio = GetComponent<AudioSource>();
        scaryAudio.Play();

        // Настраиваем NavMeshAgent
        navMeshAgent.speed = speedWalk;
        navMeshAgent.angularSpeed = 360f; // Скорость поворота
        navMeshAgent.acceleration = 8f;   // Ускорение
        collider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (CanWalk && player != null)
        {
            // Устанавливаем цель для NavMeshAgent
            navMeshAgent.SetDestination(player.position);

            // Включаем анимацию ходьбы
            animator.SetBool("IsWalking", navMeshAgent.velocity.magnitude > 0.1f);

            // Автоматический поворот уже обрабатывается NavMeshAgent
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2.1f);
        CanWalk = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!CanWalk) return;
        if (other.transform.CompareTag("Player"))
        {
            StopEnemy();
            StartCoroutine(StabSound());
            animator.SetTrigger("Stabbing");
            animator.SetBool("IsWalking", false);
            GameManager.instance.LockPlayer(transform.position);
        }
    }

    private void StopEnemy()
    {
        CanWalk = false;

        if (navMeshAgent != null && navMeshAgent.isActiveAndEnabled)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.ResetPath();
        }

        animator.SetBool("IsWalking", false);
    }

    IEnumerator StabSound()
    {
        yield return new WaitForSeconds(1f);
        stabAudioSource.PlayOneShot(stabAudio);
        StartCoroutine(GameManager.instance.TheEnd(true));
    }

    public void Dead()
    {
        StopEnemy();
        scaryAudio.Stop();
        animator.SetTrigger("Shooted");
        collider.enabled = false;
        GameManager.instance.LockPlayer(transform.position);
        GameManager.instance.Shot();
    }

}
