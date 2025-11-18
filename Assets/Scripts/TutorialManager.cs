using System;
using UnityEngine;
using static TutorialManager;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;
    public CupPoint cupPoint;

    // Определяем все возможные шаги обучения
    public enum TutorialStep
    {
        None,               // Обучение неактивно
        TakeCup,            // Возьмите стаканчик
        PutCupInMachine,    // Поставьте стаканчик в кофемашину
        PressButton,        // Нажмите кнопку
        TakeLid,            // Возьмите крышку
        PutLidOnCup,        // Наденьте крышку
        TakeCoffee,         // Возьмите кофе
        GiveCoffeeToClient, // Отдайте кофе клиенту
        Wait,               // Ждать
        GoToWarehouse,      // Посмореть склад
        Complete            // Обучение завершено
    }

    [Header("Текущее состояние")]
    [SerializeField] private TutorialStep _currentStep = TutorialStep.None;

    public event Action<TutorialStep> OnStepChanged; // Событие для уведомления о смене шага

    public TutorialStep CurrentStep
    {
        get => _currentStep;
        private set
        {
            if (_currentStep != value)
            {
                _currentStep = value;
                OnStepChanged?.Invoke(_currentStep); // Уведомляем всех подписчиков
                UpdateUI(); // Обновляем интерфейс
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Начинаем обучение с первого шага после диалога
        // Вызовем это из UIHandler, когда диалог с NPC закончится
        // StartTutorial();
    }

    public void StartTutorial()
    {
        CurrentStep = TutorialStep.TakeCup;
        cupPoint.CanWork = true;
    }

    // Этот метод будет вызываться из других скриптов для перехода к следующему шагу
    public void CompleteStep(TutorialStep completedStep)
    {
        // Проверяем, что завершили именно текущий шаг
        if (CurrentStep != completedStep) return;

        // Логика перехода к следующему шагу
        CurrentStep = completedStep switch
        {
            TutorialStep.TakeCup => TutorialStep.PutCupInMachine,
            TutorialStep.PutCupInMachine => TutorialStep.PressButton,
            TutorialStep.PressButton => TutorialStep.TakeLid,
            TutorialStep.TakeLid => TutorialStep.PutLidOnCup,
            TutorialStep.PutLidOnCup => TutorialStep.TakeCoffee,
            TutorialStep.TakeCoffee => TutorialStep.GiveCoffeeToClient,
            TutorialStep.GiveCoffeeToClient => TutorialStep.Wait,
            TutorialStep.Wait => TutorialStep.GoToWarehouse,
            TutorialStep.GoToWarehouse => TutorialStep.Complete,
            _ => TutorialStep.Complete
        };

        Debug.Log($"Tutorial step completed: {completedStep} -> Now at: {CurrentStep}");
    }

    // Для принудительной установки шага (может пригодиться)
    public void SetStep(TutorialStep newStep)
    {
        CurrentStep = newStep;
    }

    private void UpdateUI()
    {
        if (UIHandler.Instance != null)
        {
            string tutorialText = GetTutorialTextForStep(CurrentStep);
            if (string.IsNullOrEmpty(tutorialText))
            {
                UIHandler.Instance.CleanLearningText();
            }
            else
            {
                UIHandler.Instance.SetLearningText(tutorialText);
            }
        }
    }

    private string GetTutorialTextForStep(TutorialStep step)
    {
        return step switch
        {
            TutorialStep.TakeCup => "Возьмите стаканчик (ЛКМ)",
            TutorialStep.PutCupInMachine => "Поставьте стаканчик в кофемашинку",
            TutorialStep.PressButton => "Нажмите на кнопку на кофемашинке",
            TutorialStep.TakeLid => "Возьмите крышку",
            TutorialStep.PutLidOnCup => "Наденьте крышку на стаканчик с кофе",
            TutorialStep.TakeCoffee => "Возьмите стаканчик с кофе",
            TutorialStep.GiveCoffeeToClient => "Отдайте кофе клиенту (ПКМ)",
            TutorialStep.Wait => "",
            TutorialStep.GoToWarehouse => "Проверьте склад",
            TutorialStep.Complete => "",
            _ => ""
        };
    }
}
