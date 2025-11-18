using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject NPC1;
    [SerializeField] private GameObject NPC2;
    [SerializeField] private Transform NPCspawnPoint;
    [SerializeField] private Transform[] paths;
    [SerializeField] private PlayerController playerController;
    public DragNDrop dragNDrop;
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;
    public Color textColorNPC1;// = "#1A5524";
    public Color textColorNPC2;// = "#005FBC";
    [HideInInspector] public int currentNPC = 0;
    [SerializeField] private AudioSource noiseSource;
    [SerializeField] private AudioClip noiseAudio;
    [SerializeField] private AudioSource radio;
    [SerializeField] private ScreamerPoint screamerPoint;
    [SerializeField] private AudioSource heartbeat;
    private NPCcontroller policeman;

    void Start()
    {
        instance = this;
        StartNPC1();
        UIHandler.Instance.FadeIn();
    }

    public void StartNPC1()
    {
        GameObject npc = Instantiate(NPC1, NPCspawnPoint.position, Quaternion.identity);
        npc.transform.GetComponent<NPCcontroller>().pathPoints = paths;
    }

    public void StartNPC2()
    {
        GameObject npc = Instantiate(NPC2, NPCspawnPoint.position, Quaternion.identity);
        policeman = npc.GetComponent<NPCcontroller>();
        npc.transform.GetComponent<NPCcontroller>().pathPoints = paths;
    }

    public void LockPlayer(Vector3 NPCPosition)
    {
        dragNDrop.Drop();
        playerController.enabled = false;
        Vector3 npcFacePosition = NPCPosition + Vector3.up * 0.9f; //смотрим выше
        StartCoroutine(RotateTowardsNPCFace(npcFacePosition));
    }

    private IEnumerator RotateTowardsNPCFace(Vector3 npcFacePosition)
    {
        float rotationSpeed = 5f;

        // —охран€ем начальное локальное вращение камеры
        Quaternion initialCameraLocalRotation = mainCamera.transform.localRotation;

        // ¬ычисл€ем направление к лицу NPC
        Vector3 directionToFace = (npcFacePosition - player.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToFace);

        // –аздел€ем поворот: игрок - по Y, камера - по X
        Vector3 targetEuler = targetRotation.eulerAngles;

        // ÷елевой поворот игрока (только горизонталь)
        Quaternion playerTargetRotation = Quaternion.Euler(0, targetEuler.y, 0);

        // ÷елевой поворот камеры (только вертикаль относительно игрока)
        Quaternion cameraTargetLocalRotation = Quaternion.Euler(targetEuler.x, initialCameraLocalRotation.eulerAngles.y, initialCameraLocalRotation.eulerAngles.z);

        bool playerRotationComplete = false;
        bool cameraRotationComplete = false;

        while (!playerRotationComplete || !cameraRotationComplete)
        {
            // ѕоворачиваем игрока по Y
            if (!playerRotationComplete)
            {
                player.rotation = Quaternion.Slerp(player.rotation, playerTargetRotation, rotationSpeed * Time.deltaTime);
                if (Quaternion.Angle(player.rotation, playerTargetRotation) <= 0.1f)
                {
                    player.rotation = playerTargetRotation;
                    playerRotationComplete = true;
                }
            }

            // ѕоворачиваем камеру по X (локально относительно игрока)
            if (!cameraRotationComplete)
            {
                mainCamera.transform.localRotation = Quaternion.Slerp(
                    mainCamera.transform.localRotation,
                    cameraTargetLocalRotation,
                    rotationSpeed * Time.deltaTime
                );
                if (Quaternion.Angle(mainCamera.transform.localRotation, cameraTargetLocalRotation) <= 0.1f)
                {
                    mainCamera.transform.localRotation = cameraTargetLocalRotation;
                    cameraRotationComplete = true;
                }
            }

            yield return null;
        }
    }

    public void ScreamerTime()
    {
        screamerPoint.IsScreamerTime = true;
        heartbeat.Play();
    }

    public void NoiceAudio()
    {
        noiseSource.PlayOneShot(noiseAudio);
    }

    public void ActivatePlayer()
    {
        StopAllCoroutines();
        playerController.SyncCameraRotation();
        playerController.enabled = true;
    }

    public void Shot()
    {
        policeman.Shot();
        StartCoroutine(TheEnd());
    }

    public IEnumerator TheEnd(bool killedByEnemy = false)
    {
        yield return new WaitForSeconds(1f);
        if (!killedByEnemy) LockPlayer(policeman.transform.position);
        yield return new WaitForSeconds(2f);
        UIHandler.Instance.FadeOut();
    }
}
