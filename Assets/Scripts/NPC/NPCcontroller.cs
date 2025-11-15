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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        nextPointIndex = 1;
        nextPoint = pathPoints[nextPointIndex];
        currentDirectional = (nextPoint.position - transform.position).normalized;
        IsWalking = true;
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
                    animator.SetBool("IsWalking", false);
                    if (GameManager.instance.currentNPC == 0)
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
                    if (GameManager.instance.currentNPC == 0)
                    {
                        UIHandler.Instance.SetDialog(transform.position, new string[] { "Кофе. Черный." });
                    }
                    else
                    {
                        UIHandler.Instance.SetDialog(transform.position, new string[] { "Эспрессо, пожалуйста. Двойной. Не ночь, а кошмар какой-то." });
                    }
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
            StartReturnJourney();
        }
    }

    private void StartReturnJourney()
    {
        isReturning = true;
        IsWalking = true;
    }
}
