using System.Collections;
using UnityEngine;

public class MakingCoffe : MonoBehaviour
{
    [SerializeField] private AudioClip pouringSound;
    [SerializeField] private AudioSource machineSound;
    private AudioSource audioSource;
    [SerializeField] private CupPoint cupPoint;
    [SerializeField] private Transform CoffeEffectPoint1;
    [SerializeField] private Transform CoffeEffectPoint2;
    [SerializeField] private float waitCoffePouringTime = 3f;
    [SerializeField] private GameObject CoffeEffect;
    private GameObject coffeEffect1;
    private GameObject coffeEffect2;
    private GameObject coffePng;
    private bool isMaking = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartMakingCoffe()
    {
        if (isMaking) return;    
        if (!cupPoint.IsThereCup) return;
        if (cupPoint.IsCoffePoured) return;

        isMaking = true;
        machineSound.Play();
        coffePng = cupPoint.currentCup.GetComponent<Cup>().coffe;

        StartCoroutine(Wait(waitCoffePouringTime));
    }


    IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);

        audioSource.PlayOneShot(pouringSound);
        coffeEffect1 = Instantiate(CoffeEffect, CoffeEffectPoint1.position, Quaternion.Euler(90, 0, 0));
        coffeEffect2 = Instantiate(CoffeEffect, CoffeEffectPoint2.position, Quaternion.Euler(90, 0, 0));

        StartCoroutine(CleanupAfterDelay(6.6f));
    }

    IEnumerator CleanupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        coffePng.SetActive(true);

        if (machineSound.isPlaying)
        {
            machineSound.Stop();
        }

        if (coffeEffect1 != null)
        {
            Destroy(coffeEffect1);
        }
        if (coffeEffect2 != null)
        {
            Destroy(coffeEffect2);
        }

        cupPoint.IsCoffePoured = true;
        isMaking = false;
    }
}
