using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Dragabble : MonoBehaviour
{
    private Rigidbody rb;
    private Outline outline;
    private int defaultLayerMask = 0;
    private int dragLayerMask = 7;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void PrepareObject()
    {
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        outline.enabled = false;
        gameObject.layer = defaultLayerMask;
        Debug.Log("объект приготовлен");
    }

    public void DropPrepare()
    {
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        gameObject.layer = dragLayerMask;
    }

    public void DropWithForcePrepare(Vector3 direction, float force)
    {
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        gameObject.layer = dragLayerMask;

        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}
