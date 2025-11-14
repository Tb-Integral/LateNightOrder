using UnityEngine;

public class CupPoint : MonoBehaviour
{
    public bool IsThereCup;
    public Transform currentCup;
    public GameObject lastCap;
    [HideInInspector]
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
                    other.GetComponent<Rigidbody>().isKinematic = true;
                    IsThereCup = true;
                    currentCup = other.transform;
                    cupInZone = true;
                }
            }
        }
        else if (other.transform.GetComponent<Cap>() != null && IsCoffePoured && !HasCap)
        {
            other.gameObject.layer = 0;
            other.tag = "Untagged";
            Destroy(other.transform.GetComponent<Dragabble>());
            Destroy(other.transform.GetComponent<Rigidbody>());
            other.transform.position = currentCup.GetChild(1).transform.position;
            other.transform.rotation = Quaternion.Euler(-90, 0, 0);
            other.transform.SetParent(currentCup);
            HasCap = true;

            currentCup.GetComponent<Rigidbody>().isKinematic = false;
            Debug.Log($"Cup kinematic set to: {currentCup.GetComponent<Rigidbody>().isKinematic}");
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
            IsCoffePoured = false;
            HasCap = false;
            cupInZone = false;
        }
    }
}
