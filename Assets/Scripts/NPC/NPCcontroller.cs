using System.Linq;
using UnityEngine;

public class NPCcontroller : MonoBehaviour
{
    public Transform[] pathPoints;
    private Transform nextPoint;
    private int nextPointIndex;
    public float speedWalk;
    private Animator animator;
    private bool IsWalking;
    private Vector3 currentDirectional;
    private bool needCoffe;
    private bool isReturning;
    //private string hexColor;
    private Color textColor;
    private int NPCindex;
    [SerializeField] private GameObject gun;
    public bool IsSecondNPC => NPCindex == 1;

    // Диалоговые массивы
    private string[] initialDialogue; // До получения кофе
    private string[][] afterCoffeeDialogues; // Чередующиеся диалоги после кофе
    private int currentDialogueIndex = 0;
    private bool isPlayerSpeaking = false; // Чей сейчас ход в диалоге
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        nextPointIndex = 1;
        nextPoint = pathPoints[nextPointIndex];
        currentDirectional = (nextPoint.position - transform.position).normalized;
        IsWalking = true;
        if (GameManager.instance.currentNPC == 0)
        {
            NPCindex = 0;
            textColor = GameManager.instance.textColorNPC1;

            initialDialogue = new string[] { "Кофе. Черный." };

            afterCoffeeDialogues = new string[][] {
            new string[] { "У вас тут тихо. Одни работаете?" },                    // NPC
            new string[] { "Да, в эту смену один." },                             // Игрок
            new string[] { "...", "Как удобно.", "Благодарю." }                   // NPC
            };
        }
        else
        {
            NPCindex = 1;
            textColor = GameManager.instance.textColorNPC2;

            initialDialogue = new string[] { "Эспрессо, пожалуйста. Двойной. Не ночь, а кошмар какой-то." };

            afterCoffeeDialogues = new string[][] {
            new string[] { "Машину на обочине бросил. То ли стук, то ли скрежет...", "Слушай, а ты в машинах шаришь?" }, // NPC
            new string[] { "Нет, извините, не моё." },                                                                 // Игрок
            new string[] { "Ясно...", "А может, тот парень, второй, разбирается? Тот, что с тобой в смене?" },         // NPC
            new string[] { "Какой второй? Я здесь один." },                                                           // Игрок
            new string[] { "Ну, мужик в куртке.", "Я его видел, когда подъезжал.", "Он с задней стороны здания заходил, в ту вашу дверь, что возле мусорных баков. Я подумал, ваш сотрудник." }, // NPC
            new string[] { "...", "Секунду." }                                                                        // Игрок
            };
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsWalking) return;

        if (Vector3.Distance(transform.position, nextPoint.position) < 0.1f)
        {
            if (isReturning)
            {
                nextPointIndex--;
                if (nextPointIndex < 0)
                {
                    IsWalking = false;
                    if (NPCindex == 0)
                    {
                        GameManager.instance.currentNPC++;
                        GameManager.instance.StartNPC2();
                    }
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                nextPointIndex++;
                if (nextPointIndex == pathPoints.Length)
                {
                    nextPointIndex--;
                    IsWalking = false;
                    animator.SetBool("IsWalking", false);
                    needCoffe = true;
                    ShowDialogue(initialDialogue, textColor); //показываем первый диалог
                    return;
                }
            }
            nextPoint = pathPoints[nextPointIndex];
        }
        currentDirectional = (nextPoint.position - transform.position).normalized;

        if (currentDirectional != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirectional);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        transform.Translate(currentDirectional * speedWalk * Time.deltaTime, Space.World);
        animator.SetBool("IsWalking", true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!needCoffe) return;
        Cup cup = other.GetComponent<Cup>();
        if (cup == null) return;

        if (cup.IsCoffeDone)
        {
            needCoffe = false;
            Destroy(other.gameObject);
            StartAfterCoffeeDialogue();
        }
    }

    private void StartAfterCoffeeDialogue()
    {
        currentDialogueIndex = 0;
        ShowNextAfterCoffeeDialogue();
    }

    private void ShowNextAfterCoffeeDialogue()
    {
        if (currentDialogueIndex < afterCoffeeDialogues.Length)
        {
            // Определяем, кто говорит и какой цвет использовать
            isPlayerSpeaking = (currentDialogueIndex % 2 == 1); // Нечетные индексы - игрок
            Color dialogueColor = isPlayerSpeaking ? Color.white : textColor;

            string[] currentDialogue = afterCoffeeDialogues[currentDialogueIndex];
            ShowDialogue(currentDialogue, dialogueColor, true);
        }
        else
        {
            // Все диалоги завершены, начинаем возвращение
            GameManager.instance.ActivatePlayer();

            if (NPCindex == 0)
            {
                StartReturnJourney();
            }
            else
            {
                GameManager.instance.ScreamerTime();
            }
        }
    }

    private void ShowDialogue(string[] lines, Color color, bool isAfterCoffee = false)
    {
        UIHandler.Instance.SetDialog(transform.position, lines, color, isAfterCoffee ? this : null);
    }

    // Вызывается из UIHandler когда заканчивается текущий диалог
    public void OnDialogueComplete()
    {
        if (currentDialogueIndex < afterCoffeeDialogues.Length)
        {
            currentDialogueIndex++;
            ShowNextAfterCoffeeDialogue();
        }
    }

    private void StartReturnJourney()
    {
        isReturning = true;
        IsWalking = true;
    }

    public void Shot()
    {
        animator.SetTrigger("Shot");
        gun.SetActive(true);
    }
}
