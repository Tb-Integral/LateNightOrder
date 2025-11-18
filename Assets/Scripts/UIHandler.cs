using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler Instance;
    public float textSpeed;
    [SerializeField] private AudioClip textBlip;
    private AudioSource textSource;
    private VisualElement dialogWindow;
    private VisualElement centerPoint;
    private Label label;
    private VisualElement blackScreen;
    private bool IsDialogActive;
    private string[] currentLines;
    private int currentLineIndex = 0;
    private Color currentTextColor;
    private NPCcontroller currentNPC;
    private bool secondNPC;

    // FPS
    private Label fpsLabel;
    private float deltaTime = 0.0f;

    // Learning
    private Label learningLabel;

    void Awake()
    {
        Instance = this;
        var root = GetComponent<UIDocument>().rootVisualElement;
        dialogWindow = root.Q<VisualElement>("Background");
        label = root.Q<Label>("DialogText");
        blackScreen = root.Q<VisualElement>("BlackScreen");
        dialogWindow.style.display = DisplayStyle.None;
        label.text = string.Empty;
        centerPoint = root.Q<VisualElement>("CenterPoint"); 
        textSource = GetComponent<AudioSource>();

        blackScreen.style.display = DisplayStyle.None;
        blackScreen.style.opacity = 0f;

        fpsLabel = root.Q<Label>("FPS");
        learningLabel = root.Q<Label>("Teaching");
        learningLabel.style.display = DisplayStyle.None;
    }

    void Update()
    {
        // Обновление FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        if (fpsLabel != null)
            fpsLabel.text = $"FPS: {Mathf.Ceil(fps)}";

        if (Input.GetKeyDown(KeyCode.Space) && IsDialogActive)
        {
            SkipText();
        }
    }

    public void SetDialog(Vector3 NPCPosition, string[] lines, Color textColor, NPCcontroller npc = null)
    {
        IsDialogActive = true;
        currentLines = lines;
        currentTextColor = textColor;
        currentTextColor.a = 1f;
        currentNPC = npc;

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
            textSource.PlayOneShot(textBlip);
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

    public void FadeOut()
    {
        StartCoroutine(FadeScreen(0f, 1f, 1f));
    }

    public void FadeIn()
    {
        StartCoroutine(FadeScreen(1f, 0f, 1f));
    }

    public void FadeTo(float targetAlpha, float duration = 1f)
    {
        float currentAlpha = blackScreen.resolvedStyle.opacity;
        StartCoroutine(FadeScreen(currentAlpha, targetAlpha, duration));
    }

    IEnumerator FadeScreen(float startAlpha, float targetAlpha, float duration)
    {
        blackScreen.style.display = DisplayStyle.Flex;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            blackScreen.style.opacity = currentAlpha;
            yield return null;
        }

        blackScreen.style.opacity = targetAlpha;

        if (targetAlpha == 0f)
        {
            blackScreen.style.display = DisplayStyle.None;
        }
        else
        {
            SceneManager.LoadScene("CoffeeHorror");
        }
    }

    public void SetLearningText(string text)
    {
        learningLabel.text = text;
        learningLabel.style.display = DisplayStyle.Flex;
    }

    public void CleanLearningText()
    {
        learningLabel.text = "";
        learningLabel.style.display = DisplayStyle.None;
    }
}
