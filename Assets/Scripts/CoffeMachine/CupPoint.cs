using UnityEngine;

public class CupPoint : MonoBehaviour
{
    public bool IsThereCup;
    public Transform currentCup;
    public GameObject lastCap;

    [HideInInspector] public bool IsCoffeStartPouring = false;
    public bool IsCoffePoured = false;
    public bool HasCap = false;
    public bool cupInZone = false;
    public bool CanWork = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!CanWork) return;

        //  ешируем компоненты
        var cup = other.GetComponent<Cup>();
        var cap = other.GetComponent<Cap>();

        // --- 1. ¬ход чашки -----------------------------------------
        if (!IsThereCup && cup != null && other.gameObject != lastCap)
        {
            PlaceCup(other, cup);
            return;
        }

        // --- 2. ¬ход крышки -----------------------------------------
        if (IsThereCup && IsCoffePoured && !HasCap && cap != null)
        {
            PlaceCap(other);
        }
    }

    private void PlaceCup(Collider other, Cup cup)
    {
        other.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(-90, 0, 0));
        other.gameObject.layer = default;

        // ќчень дорого вызывать Destroy(GetComponent) -> поэтому берем один раз
        var dr = other.GetComponent<Dragabble>();
        if (dr != null) dr.SwitchOutlineOff();
        Destroy(dr);
        Destroy(other.GetComponent<Rigidbody>());

        IsThereCup = true;
        currentCup = other.transform;
        cupInZone = true;

        // “ут тоже кешируем, чтобы не обращатьс€ 10 раз
        var tutorial = TutorialManager.Instance;
        if (tutorial.CurrentStep == TutorialManager.TutorialStep.PutCupInMachine)
        {
            tutorial.CompleteStep(TutorialManager.TutorialStep.PutCupInMachine);
        }
    }

    private void PlaceCap(Collider other)
    {
        other.tag = "Untagged";

        var dr = other.GetComponent<Dragabble>();
        if (dr != null) dr.SwitchOutlineOff();

        Destroy(dr);
        Destroy(other.GetComponent<Rigidbody>());

        other.transform.position = currentCup.GetChild(1).position;
        other.transform.rotation = Quaternion.Euler(-90, 0, 0);
        other.transform.SetParent(currentCup);

        HasCap = true;

        var cup = currentCup.GetComponent<Cup>();
        cup.IsCoffeDone = true;

        // возвращаем возможность поднимать
        currentCup.gameObject.AddComponent<Dragabble>();
        currentCup.gameObject.AddComponent<Rigidbody>();
        currentCup.tag = "Draggable";
        currentCup.gameObject.layer = LayerMask.NameToLayer("Draggable");

        var tutorial = TutorialManager.Instance;
        if (tutorial.CurrentStep == TutorialManager.TutorialStep.PutLidOnCup)
        {
            tutorial.CompleteStep(TutorialManager.TutorialStep.PutLidOnCup);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == currentCup)
        {
            lastCap = currentCup.gameObject;
            currentCup = null;

            IsThereCup = false;
            IsCoffeStartPouring = false;
            IsCoffePoured = false;
            HasCap = false;
            cupInZone = false;
        }
    }
}
