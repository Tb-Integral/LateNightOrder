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

    private CharacterController controller;
    private float xRotation = 0f;
    private AudioSource footsteps;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        footsteps = transform.GetChild(0).GetComponent<AudioSource>();
    }

    void Update()
    {
        Look();
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Столкнулись: "+collision.gameObject);
    }

    void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveZ != 0)
        {
            Vector3 cameraForward = playerCamera.forward;
            Vector3 cameraRight = playerCamera.right;

            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            if (!footsteps.isPlaying)
            {
                footsteps.Play();
            }

            Vector3 move = (cameraForward * moveZ + cameraRight * moveX).normalized;
            controller.Move(move * moveSpeed * Time.deltaTime);
        }
        else
        {
            if (footsteps.isPlaying)
            {
                footsteps.Stop();
            }
        }
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void SyncCameraRotation()
    {
        xRotation = playerCamera.localEulerAngles.x;

        // Нормализуем угол к диапазону [-180, 180] для корректной работы Clamp
        if (xRotation > 180f)
            xRotation -= 360f;

        // Обеспечиваем что угол в пределах допустимого диапазона
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
    }
}