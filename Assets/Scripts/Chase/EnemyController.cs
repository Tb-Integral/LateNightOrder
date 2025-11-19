using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public float speedWalk;
    private Animator animator;
    private NavMeshAgent agent;

    private AudioSource scaryAudio;
    private AudioSource footsteps;

    private bool canWalk;
    private bool isWalkingAnim = false;

    private Vector3 lastPlayerPos;
    private float updateTimer;

    private readonly WaitForSeconds stabDelay = new WaitForSeconds(1f);
    private readonly WaitForSeconds startDelay = new WaitForSeconds(2.1f);

    [HideInInspector] public Transform player;
    public AudioClip stabAudio;

    private BoxCollider col;

    private const float UPDATE_INTERVAL = 0.3f;

    void Start()
    {
        StartCoroutine(StartWalking());

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        col = GetComponent<BoxCollider>();

        // Берём все AudioSource разом
        scaryAudio = GetComponent<AudioSource>();
        footsteps = transform.GetChild(0).GetComponent<AudioSource>();

        scaryAudio.Play();

        agent.speed = speedWalk;
        agent.angularSpeed = 360f;
        agent.acceleration = 8f;

        TutorialManager.Instance.CompleteStep(TutorialManager.TutorialStep.GoToWarehouse);
    }

    void Update()
    {
        if (!canWalk || player == null)
        {
            if (footsteps.isPlaying) footsteps.Pause();
            SetWalkAnim(false);
            return;
        }

        if (!footsteps.isPlaying)
            footsteps.Play();

        updateTimer -= Time.deltaTime;

        if (updateTimer <= 0f)
        {
            // если игрок не сдвинулся — не обновляем путь
            if ((player.position - lastPlayerPos).sqrMagnitude > 0.2f)
            {
                agent.SetDestination(player.position);
                lastPlayerPos = player.position;
            }

            updateTimer = UPDATE_INTERVAL;
        }

        bool isMoving = agent.velocity.sqrMagnitude > 0.1f;
        SetWalkAnim(isMoving);
    }

    private void SetWalkAnim(bool state)
    {
        if (state != isWalkingAnim)
        {
            isWalkingAnim = state;
            animator.SetBool("IsWalking", state);
        }
    }

    IEnumerator StartWalking()
    {
        yield return startDelay;
        canWalk = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canWalk) return;

        if (other.CompareTag("Player"))
        {
            StopEnemy();
            StartCoroutine(StabRoutine());
            animator.SetTrigger("Stabbing");

            GameManager.instance.LockPlayer(transform.position);
        }
    }

    private void StopEnemy()
    {
        canWalk = false;
        agent.isStopped = true;
        agent.ResetPath();
        SetWalkAnim(false);
    }

    IEnumerator StabRoutine()
    {
        yield return stabDelay;
        scaryAudio.PlayOneShot(stabAudio);
        StartCoroutine(GameManager.instance.TheEnd(true));
    }

    public void Dead()
    {
        StopEnemy();
        scaryAudio.Stop();

        animator.SetTrigger("Shooted");
        col.enabled = false;

        GameManager.instance.LockPlayer(transform.position);
        GameManager.instance.Shot();
    }
}
