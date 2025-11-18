using System;
using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    [SerializeField] private float speedForDraggableObject = 5f;
    private KeyCode key0 = KeyCode.Mouse0;
    private string Tag = "Draggable";
    [SerializeField] private float rayLength = 3f;
    [SerializeField] private Transform playerCamera;
    private Transform draggableObject;
    private Rigidbody rbDraggableObject;
    [SerializeField] private Transform dragPoint;
    [SerializeField] private LayerMask defaultLayerMask;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private CupPoint cupPoint;
    private bool IsDragging;
    private KeyCode key1 = KeyCode.Mouse1;
    [SerializeField] private float dropForce = 5f;
    private Dragabble lastOutlineObject;
    private Outline lastOutlineButton;
    private string TagButton = "Button";
    [HideInInspector] public bool mouseInputEnabled = true;
    [HideInInspector] public bool canLearning;

    // Update is called once per frame
    void Update()
    {
        if (!mouseInputEnabled) return;
        PerformRaycast();
    }

    private void PerformRaycast()
    {

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, rayLength, defaultLayerMask))
        {
            if (hit.transform.CompareTag(Tag))
            {

                if (lastOutlineObject != null)
                {
                    lastOutlineObject.SwitchOutlineOff();
                }

                if (!IsDragging)
                {
                    lastOutlineObject = hit.transform.GetComponent<Dragabble>();
                    lastOutlineObject.SwitchOutlineOn();
                }

                if (Input.GetKeyDown(key0))
                {
                    CheckTutorialStepOnPickup(hit.transform);
                    PrepareObgect(hit);
                    IsDragging = true;
                }
            }
            else if (hit.transform.CompareTag(TagButton))
            {
                if (cupPoint != null && cupPoint.IsThereCup && !cupPoint.IsCoffeStartPouring)
                {
                    lastOutlineButton = hit.transform.GetComponent<Outline>();
                    lastOutlineButton.enabled = true;

                    if (Input.GetKeyDown(key0))
                    {
                        hit.transform.GetComponent<MakingCoffe>().StartMakingCoffe();

                        // Сообщаем о нажатии кнопки, если это текущий шаг обучения
                        if (canLearning && TutorialManager.Instance.CurrentStep == TutorialManager.TutorialStep.PressButton)
                        {
                            TutorialManager.Instance.CompleteStep(TutorialManager.TutorialStep.PressButton);
                        }
                    }
                }
            }

        }
        else if (lastOutlineObject != null || lastOutlineButton != null)
        {
            if (lastOutlineObject != null)
            {
                lastOutlineObject.SwitchOutlineOff();
                lastOutlineObject = null;
            }
            if (lastOutlineButton != null)
            {
                lastOutlineButton.enabled = false;
                lastOutlineButton = null;
            }
        }

        if (Input.GetKeyUp(key0) && draggableObject != null)
        {
            IsDragging = false;
            Drop();
        }

        if (Input.GetKeyDown(key1) && draggableObject != null)
        {
            IsDragging = false;
            DropWithForce();
        }
    }

    private void DropWithForce()
    {
        if (rbDraggableObject != null)
        {
            draggableObject.GetComponent<Dragabble>().DropWithForcePrepare(playerCamera.forward, dropForce);
            draggableObject = null;
            rbDraggableObject = null;
        }
    }

    private void FixedUpdate()
    {
        if (IsDragging && draggableObject != null)
        {
            Drag();
        }
    }

    private void Drag()
    {
        if (rbDraggableObject != null)
        {
            //перемещение
            Vector3 direction = dragPoint.position - draggableObject.transform.position;
            rbDraggableObject.linearVelocity = direction * speedForDraggableObject;

            //вращение
            Quaternion targetRotation = Quaternion.Euler(-90f, 0f, 0f);
            rbDraggableObject.rotation = Quaternion.Slerp(rbDraggableObject.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void Drop()
    {
        if (rbDraggableObject != null)
        {
            draggableObject.GetComponent<Dragabble>().DropPrepare();
            draggableObject = null;
            rbDraggableObject = null;
        }
    }

    private void PrepareObgect(RaycastHit hit)
    {
        draggableObject = hit.transform;
        draggableObject.GetComponent<Dragabble>().PrepareObject();
        rbDraggableObject = hit.transform.GetComponent<Rigidbody>();
    }

    private void CheckTutorialStepOnPickup(Transform pickedObject)
    {
        Debug.Log("can learning: "+canLearning);
        if (!canLearning) return;

        var currentStep = TutorialManager.Instance.CurrentStep;
        Cup cup = pickedObject.GetComponent<Cup>();

        // Если взяли стаканчик
        if (cup != null)
        {
            // Проверяем, есть ли в стаканчике кофе
            if (cup.HasCoffee)
            {
                // Если в стаканчике есть кофе - проверяем задание на подбор готового кофе
                if (currentStep == TutorialManager.TutorialStep.TakeCoffee && cup.IsCoffeDone)
                {
                    TutorialManager.Instance.CompleteStep(TutorialManager.TutorialStep.TakeCoffee);
                }
            }
            else
            {
                Debug.Log("TakeCup? "+ (currentStep == TutorialManager.TutorialStep.TakeCup));
                // Если стаканчик пустой - проверяем задание на подбор стаканчика
                if (currentStep == TutorialManager.TutorialStep.TakeCup)
                {
                    TutorialManager.Instance.CompleteStep(TutorialManager.TutorialStep.TakeCup);
                }
            }
        }
        // Проверяем, взяли ли крышку (отдельный объект)
        else if (currentStep == TutorialManager.TutorialStep.TakeLid && pickedObject.GetComponent<Cap>() != null)
        {
            TutorialManager.Instance.CompleteStep(TutorialManager.TutorialStep.TakeLid);
        }
    }
}
