using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class Dragabble : MonoBehaviour
{
    [SerializeField] private Transform additionalMesh;
    private Rigidbody rb;
    private Outline outline;
    private bool IsFalling;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // у крышки плохо отображается аутлайн, поэтому берем его дочерний цилиндр для этого
        if (additionalMesh != null)
        {
            outline = additionalMesh.GetComponent<Outline>();
        }
        else
        {
            outline = GetComponent<Outline>();
        }
        outline.enabled = false;
    }

    public void PrepareObject()
    {
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        outline.enabled = false;
    }

    public void DropPrepare()
    {
        rb.useGravity = true;
        IsFalling = true;
    }

    public void DropWithForcePrepare(Vector3 direction, float force)
    {
        rb.useGravity = true;
        IsFalling = true;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    public void SwitchOutlineOn()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void SwitchOutlineOff()
    {
        if (outline != null)
            outline.enabled = false;
    }

    // если сразу делать простую физику столкновений после броска, то предметы могут иногда падать сквозь пол,
    // поэтому нужно сохранять ее сложной до тех пор, пока предмет впервые после броска не столкнется с преградой
    private void OnCollisionEnter(Collision collision)
    {
        if (IsFalling)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            IsFalling = false;
        }
    }
}
