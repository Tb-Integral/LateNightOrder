using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject NPC1;
    [SerializeField] private GameObject NPC2;
    [SerializeField] private Transform NPCspawnPoint;
    [SerializeField] private Transform[] paths;
    [HideInInspector] public int currentNPC = 0;

    void Start()
    {
        instance = this;
        StartNPC1();
    }

    public void StartTarining()
    {

    }

    public void StartNPC1()
    {
        GameObject npc = Instantiate(NPC1, NPCspawnPoint.position, Quaternion.identity);
        npc.transform.GetComponent<NPCcontroller>().pathPoints = paths;
    }

    public void StartNPC2()
    {
        GameObject npc = Instantiate(NPC2, NPCspawnPoint.position, Quaternion.identity);
        npc.transform.GetComponent<NPCcontroller>().pathPoints = paths;
    }
}
