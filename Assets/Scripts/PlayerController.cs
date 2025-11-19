using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Camera Look")]
    public Transform playerCamera;
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Optimization")]
    [SerializeField] private float inputUpdateInterval = 0.016f; // 60 FPS для ввода
    [SerializeField] private float audioCheckInterval = 0.1f; // 10 раз в секунду для аудио

    private CharacterController controller;
    private float xRotation = 0f;
    private AudioSource footsteps;

    // ОПТИМИЗАЦИЯ: Кэширование ввода
    private float inputTimer = 0f;
    private float audioTimer = 0f;
    private Vector2 cachedMoveInput;
    private Vector2 cachedLookInput;
    private bool isMoving = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        footsteps = transform.GetChild(0).GetComponent<AudioSource>();
    }

    void Update()
    {
        // ОПТИМИЗАЦИЯ: Разделение частоты обновления
        UpdateInput();
        UpdateMovement();
        UpdateAudio();
    }

    private void UpdateInput()
    {
        // ОБНОВЛЯЕМ ВВОД РЕЖЕ (60 FPS вместо каждого кадра)
        inputTimer += Time.deltaTime;
        if (inputTimer >= inputUpdateInterval)
        {
            // Кэшируем значения ввода
            cachedMoveInput.x = Input.GetAxisRaw("Horizontal");
            cachedMoveInput.y = Input.GetAxisRaw("Vertical");

            cachedLookInput.x = Input.GetAxis("Mouse X");
            cachedLookInput.y = Input.GetAxis("Mouse Y");

            inputTimer = 0f;
        }
    }

    private void UpdateMovement()
    {
        // ДВИЖЕНИЕ на основе кэшированного ввода (каждый кадр)
        if (cachedMoveInput.x != 0 || cachedMoveInput.y != 0)
        {
            Vector3 cameraForward = playerCamera.forward;
            Vector3 cameraRight = playerCamera.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 move = (cameraForward * cachedMoveInput.y + cameraRight * cachedMoveInput.x).normalized;
            controller.Move(move * moveSpeed * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        // ВРАЩЕНИЕ на основе кэшированного ввода (каждый кадр)
        if (cachedLookInput != Vector2.zero)
        {
            xRotation -= cachedLookInput.y * mouseSensitivity;
            xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * cachedLookInput.x * mouseSensitivity);
        }
    }

    private void UpdateAudio()
    {
        // ОПТИМИЗАЦИЯ: Проверка аудио реже
        audioTimer += Time.deltaTime;
        if (audioTimer >= audioCheckInterval)
        {
            if (isMoving)
            {
                if (!footsteps.isPlaying)
                {
                    footsteps.Play();
                }
            }
            else
            {
                if (footsteps.isPlaying)
                {
                    footsteps.Stop();
                }
            }
            audioTimer = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ОПТИМИЗАЦИЯ: Уберите в билде или используйте conditional compilation
#if UNITY_EDITOR
        Debug.Log("Столкнулись: " + collision.gameObject);
#endif
    }

    public void SyncCameraRotation()
    {
        xRotation = playerCamera.localEulerAngles.x;

        if (xRotation > 180f)
            xRotation -= 360f;

        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
    }
}