using UnityEngine;

public class NPCcontroller : MonoBehaviour
{
    [Header("Path")]
    public Transform[] pathPoints;
    public float speedWalk = 2f;

    [Header("NPC")]
    [SerializeField] private GameObject gun;
    public bool IsSecondNPC => NPCindex == 1;

    private int NPCindex;
    private int nextPointIndex = 1;
    private bool isReturning;
    private bool needCoffee;
    private bool isWalking = true;

    private Transform nextPoint;
    private Vector3 direction;

    // Systems
    private Animator animator;
    private AudioSource footsteps;
    private DragNDrop dragNDrop;

    // Dialogue
    private string[] initialDialogue;
    private string[][] afterCoffeeDialogues;
    private int dialogueIndex = 0;

    private Color textColor;


    private void Start()
    {
        dragNDrop = GameManager.instance.dragNDrop;
        animator = GetComponent<Animator>();
        footsteps = transform.GetChild(0).GetComponent<AudioSource>();

        nextPoint = pathPoints[nextPointIndex];

        // ------------------------------------
        // NPC CONFIG
        // ------------------------------------
        NPCindex = GameManager.instance.currentNPC;
        bool isFirstNPC = NPCindex == 0;

        textColor = isFirstNPC ?
            GameManager.instance.textColorNPC1 :
            GameManager.instance.textColorNPC2;

        if (isFirstNPC)
        {
            initialDialogue = new[] { "Кофе. Черный." };
            afterCoffeeDialogues = new[]
            {
                new[] { "У вас тут тихо. Одни работаете?" },
                new[] { "Да, в эту смену один." },
                new[] { "...", "Как удобно.", "Благодарю." }
            };
        }
        else
        {
            initialDialogue = new[] { "Эспрессо, пожалуйста. Двойной. Не ночь, а кошмар какой-то." };
            afterCoffeeDialogues = new[]
            {
                new[] { "Машину на обочине бросил...", "Слушай, а ты в машинах шаришь?" },
                new[] { "Нет, извините, не моё." },
                new[] { "Ясно...", "А может, тот парень, второй, разбирается?" },
                new[] { "Какой второй? Я здесь один." },
                new[] { "Ну, мужик в куртке...", "Зашёл с той двери возле баков." },
                new[] { "...", "Секунду." }
            };
        }
    }


    private void Update()
    {
        if (!isWalking)
        {
            animator.SetBool("IsWalking", false);
            return;
        }

        // Движение
        MoveTowardsPoint();

        // Анимация
        animator.SetBool("IsWalking", true);
    }


    // ---------------------------------------------------------
    //  ДВИЖЕНИЕ
    // ---------------------------------------------------------
    private void MoveTowardsPoint()
    {
        float distance = Vector3.Distance(transform.position, nextPoint.position);

        if (distance < 0.1f)
        {
            HandlePointReached();
            return;
        }

        direction = (nextPoint.position - transform.position).normalized;

        // Поворот
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            5f * Time.deltaTime
        );

        // Шаги
        if (!footsteps.isPlaying) footsteps.Play();

        // Перемещение
        transform.Translate(direction * speedWalk * Time.deltaTime, Space.World);
    }


    private void HandlePointReached()
    {
        nextPointIndex += isReturning ? -1 : +1;

        // NPC УХОДИТ
        if (isReturning && nextPointIndex < 0)
        {
            GameManager.instance.currentNPC++;
            GameManager.instance.StartNPC2();
            Destroy(gameObject);
            return;
        }

        // NPC ДОШЁЛ ДО ТОЧКИ КОФЕ
        if (!isReturning && nextPointIndex >= pathPoints.Length)
        {
            nextPointIndex = pathPoints.Length - 1;
            isWalking = false;

            animator.SetBool("IsWalking", false);
            footsteps.Pause();

            needCoffee = true;

            if (TutorialManager.Instance.CurrentStep == TutorialManager.TutorialStep.None)
            {
                dragNDrop.canLearning = true;
                TutorialManager.Instance.StartTutorial();
            }

            ShowDialogue(initialDialogue, textColor);
            return;
        }

        nextPoint = pathPoints[nextPointIndex];
    }


    // ---------------------------------------------------------
    //  ДИАЛОГИ
    // ---------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (!needCoffee) return;
        if (!other.TryGetComponent(out Cup cup)) return;
        if (!cup.IsCoffeDone) return;

        needCoffee = false;
        Destroy(other.gameObject);

        if (TutorialManager.Instance.CurrentStep == TutorialManager.TutorialStep.GiveCoffeeToClient)
            TutorialManager.Instance.CompleteStep(TutorialManager.TutorialStep.GiveCoffeeToClient);

        dialogueIndex = 0;
        ShowNextAfterCoffeeDialogue();
    }


    private void ShowNextAfterCoffeeDialogue()
    {
        if (dialogueIndex < afterCoffeeDialogues.Length)
        {
            bool isPlayerSpeaking = (dialogueIndex % 2 == 1);
            Color color = isPlayerSpeaking ? Color.white : textColor;

            ShowDialogue(afterCoffeeDialogues[dialogueIndex], color, true);
        }
        else
        {
            EndAfterCoffeeDialogue();
        }
    }


    private void EndAfterCoffeeDialogue()
    {
        GameManager.instance.ActivatePlayer();

        if (NPCindex == 0)
        {
            StartReturnJourney();
        }
        else
        {
            if (TutorialManager.Instance.CurrentStep == TutorialManager.TutorialStep.Wait)
                TutorialManager.Instance.CompleteStep(TutorialManager.TutorialStep.Wait);

            GameManager.instance.ScreamerTime();
        }
    }


    private void ShowDialogue(string[] lines, Color color, bool isAfterCoffee = false)
    {
        UIHandler.Instance.SetDialog(transform.position, lines, color, isAfterCoffee ? this : null);
    }


    public void OnDialogueComplete()
    {
        dialogueIndex++;
        ShowNextAfterCoffeeDialogue();
    }


    // ---------------------------------------------------------
    //  ДРУГОЕ
    // ---------------------------------------------------------
    private void StartReturnJourney()
    {
        isReturning = true;
        isWalking = true;
    }

    public void Shot()
    {
        animator.SetTrigger("Shot");
        gun.SetActive(true);
    }
}
