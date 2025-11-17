using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;
    public float textSpeed;
    private VisualElement dialogWindow;
    private VisualElement centerPoint;
    private Label label;
    private bool IsDialogActive;
    private string[] currentLines;
    private int currentLineIndex = 0;
    private Color currentTextColor;
    private NPCcontroller currentNPC; // Для отслеживания чередующихся диалогов
    private bool secondNPC;

    void Awake()
    {
        Instance = this;
        dialogWindow = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("Background");
        label = GetComponent<UIDocument>().rootVisualElement.Q<Label>();
        dialogWindow.style.display = DisplayStyle.None;
        label.text = string.Empty;
        centerPoint = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("CenterPoint");
    }

    public void SetDialog(Vector3 NPCPosition, string[] lines, Color textColor, NPCcontroller npc = null)
    {
        IsDialogActive = true;
        currentLines = lines;
        currentTextColor = textColor;
        currentTextColor.a = 1f;
        currentNPC = npc; // Сохраняем ссылку на NPC для чередующихся диалогов

        label.style.color = new StyleColor(currentTextColor);
        currentLineIndex = 0;
        StartCoroutine(TypeLine());
        dialogWindow.style.display = DisplayStyle.Flex;
        centerPoint.style.display = DisplayStyle.None;
        GameManager.instance.LockPlayer(NPCPosition);
    }

    IEnumerator TypeLine()
    {
        foreach (char c in currentLines[currentLineIndex].ToCharArray())
        {
            label.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void SkipText()
    {
        if (label.text == currentLines[currentLineIndex])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            label.text = currentLines[currentLineIndex];
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsDialogActive)
        {
            SkipText();
        }
    }

    private void NextLine()
    {
        if (currentLineIndex < currentLines.Length - 1)
        {
            currentLineIndex++;
            label.text = string.Empty;
            label.style.color = currentTextColor;
            StartCoroutine(TypeLine());
        }
        else
        {
            label.text = string.Empty;
            IsDialogActive = false;
            dialogWindow.style.display = DisplayStyle.None;
            centerPoint.style.display = DisplayStyle.Flex;

            if (currentNPC != null)
            {
                currentNPC.OnDialogueComplete();
            }
            else
            {
                if (secondNPC)
                {
                    GameManager.instance.NoiceAudio();
                }
                else
                {
                    secondNPC = true;
                }

                GameManager.instance.ActivatePlayer();
            }
        }
    }
}
