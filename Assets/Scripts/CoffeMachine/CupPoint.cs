using Unity.VisualScripting;
using UnityEngine;

public class CupPoint : MonoBehaviour
{
    public bool IsThereCup;
    public Transform currentCup;
    public GameObject lastCap;
    [HideInInspector]
    public bool IsCoffeStartPouring = false;
    public bool IsCoffePoured = false;
    public bool HasCap = false;
    public bool cupInZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsThereCup)
        {
            if (other.transform.GetComponent<Cup>() != null)
            {
                if (other.gameObject != lastCap)
                {
                    other.transform.position = transform.position;
                    other.transform.rotation = Quaternion.Euler(-90, 0, 0);
                    other.gameObject.layer = default;
                    other.GetComponent<Dragabble>().SwitchOutlineOff();
                    Destroy(other.GetComponent<Dragabble>());
                    Destroy(other.GetComponent<Rigidbody>());
                    IsThereCup = true;
                    currentCup = other.transform;
                    cupInZone = true;
                }
            }
        }
        else if (other.transform.GetComponent<Cap>() != null && IsCoffePoured && !HasCap)
        {
            other.tag = "Untagged";
            other.GetComponent<Dragabble>().SwitchOutlineOff();
            Destroy(other.transform.GetComponent<Dragabble>());
            Destroy(other.transform.GetComponent<Rigidbody>());
            other.transform.position = currentCup.GetChild(1).transform.position;
            other.transform.rotation = Quaternion.Euler(-90, 0, 0);
            other.transform.SetParent(currentCup);
            HasCap = true;

            currentCup.transform.GetComponent<Cup>().IsCoffeDone = true;
            currentCup.AddComponent<Dragabble>();
            currentCup.AddComponent<Rigidbody>();
            currentCup.tag = "Draggable";
            currentCup.gameObject.layer = LayerMask.NameToLayer("Draggable");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == currentCup)
        {
            IsThereCup = false;
            lastCap = currentCup.gameObject;
            currentCup = null;
            IsCoffeStartPouring = false;
            IsCoffePoured = false;
            HasCap = false;
            cupInZone = false;
        }
    }
}
