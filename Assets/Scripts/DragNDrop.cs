using UnityEngine;

public class DragNDrop : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float dragSpeed = 5f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float rayLength = 3f;
    [SerializeField] private float dropForce = 5f;

    [Header("Refs")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform dragPoint;
    [SerializeField] private LayerMask rayMask;
    [SerializeField] private CupPoint cupPoint;

    // States
    private bool isDragging;
    public bool mouseInputEnabled = true;
    public bool canLearning;

    // Cached draggable
    private Transform dragObject;
    private Rigidbody dragRb;
    private Dragabble dragScript;

    // Outlines
    private Dragabble lastOutlineObject;
    private Outline lastOutlineButton;

    private const string TagDraggable = "Draggable";
    private const string TagButton = "Button";

    void Update()
    {
        if (!mouseInputEnabled) return;
        ProcessRaycast();

        if (isDragging && Input.GetMouseButtonUp(0)) Drop();
        if (isDragging && Input.GetMouseButtonDown(1)) DropWithForce();
    }

    private void FixedUpdate()
    {
        if (isDragging && dragRb)
        {
            Vector3 dir = dragPoint.position - dragObject.position;
            dragRb.linearVelocity = dir * dragSpeed;

            // Rotation
            Quaternion rot = Quaternion.Euler(-90, 0, 0);
            dragRb.rotation = Quaternion.Slerp(dragRb.rotation, rot, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    // --------------------------
    //   RAYCAST LOGIC
    // --------------------------
    private void ProcessRaycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, rayLength, rayMask))
        {
            // DRAGGABLE
            if (hit.transform.CompareTag(TagDraggable))
            {
                HandleOutlineOnObject(hit);

                if (Input.GetMouseButtonDown(0) && !isDragging)
                {
                    CheckTutorialOnPickup(hit.transform);
                    StartDragging(hit.transform);
                }

                return;
            }

            // BUTTON
            if (hit.transform.CompareTag(TagButton))
            {
                HandleOutlineOnButton(hit);

                if (Input.GetMouseButtonDown(0))
                {
                    hit.transform.GetComponent<MakingCoffe>().StartMakingCoffe();
                    TryCompleteTutorialButton();
                }

                return;
            }
        }

        // Если ни во что не попали — выключаем подсветки
        ClearOutlines();
    }

    // --------------------------
    //   OUTLINES
    // --------------------------
    private void HandleOutlineOnObject(RaycastHit hit)
    {
        // reset button outline
        if (lastOutlineButton)
        {
            lastOutlineButton.enabled = false;
            lastOutlineButton = null;
        }

        if (lastOutlineObject && lastOutlineObject.transform != hit.transform)
            lastOutlineObject.SwitchOutlineOff();

        if (!isDragging)
        {
            lastOutlineObject = hit.transform.GetComponent<Dragabble>();
            lastOutlineObject.SwitchOutlineOn();
        }
    }

    private void HandleOutlineOnButton(RaycastHit hit)
    {
        if (cupPoint != null && cupPoint.IsThereCup && !cupPoint.IsCoffeStartPouring)
        {
            if (lastOutlineObject)
            {
                lastOutlineObject.SwitchOutlineOff();
                lastOutlineObject = null;
            }

            lastOutlineButton = hit.transform.GetComponent<Outline>();
            lastOutlineButton.enabled = true;
        }
    }

    private void ClearOutlines()
    {
        if (lastOutlineObject)
        {
            lastOutlineObject.SwitchOutlineOff();
            lastOutlineObject = null;
        }

        if (lastOutlineButton)
        {
            lastOutlineButton.enabled = false;
            lastOutlineButton = null;
        }
    }

    // --------------------------
    //   DRAGGING
    // --------------------------
    private void StartDragging(Transform obj)
    {
        dragObject = obj;
        dragScript = obj.GetComponent<Dragabble>();
        dragRb = obj.GetComponent<Rigidbody>();

        dragScript.PrepareObject();

        isDragging = true;
    }

    public void Drop()
    {
        if (dragScript != null)
        {
            dragScript.DropPrepare();
        }
        ClearDragState();
    }

    private void DropWithForce()
    {
        if (dragScript != null)
        {
            dragScript.DropWithForcePrepare(playerCamera.forward, dropForce);
        }
        ClearDragState();
    }

    private void ClearDragState()
    {
        dragObject = null;
        dragRb = null;
        dragScript = null;
        isDragging = false;
    }

    // --------------------------
    //   TUTORIAL
    // --------------------------
    private void TryCompleteTutorialButton()
    {
        if (canLearning &&
            TutorialManager.Instance.CurrentStep == TutorialManager.TutorialStep.PressButton)
        {
            TutorialManager.Instance.CompleteStep(TutorialManager.TutorialStep.PressButton);
        }
    }

    private void CheckTutorialOnPickup(Transform obj)
    {
        if (!canLearning) return;

        var step = TutorialManager.Instance.CurrentStep;

        // Cup
        Cup cup = obj.GetComponent<Cup>();
        if (cup != null)
        {
            if (cup.HasCoffee)
            {
                if (step == TutorialManager.TutorialStep.TakeCoffee && cup.IsCoffeDone)
                    TutorialManager.Instance.CompleteStep(step);
            }
            else
            {
                if (step == TutorialManager.TutorialStep.TakeCup)
                    TutorialManager.Instance.CompleteStep(step);
            }
            return;
        }

        // Lid
        if (obj.GetComponent<Cap>() && step == TutorialManager.TutorialStep.TakeLid)
        {
            TutorialManager.Instance.CompleteStep(step);
        }
    }
}
