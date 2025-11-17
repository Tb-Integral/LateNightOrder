using UnityEngine;

public class ShootPoint : MonoBehaviour
{
    [SerializeField] private AudioSource shotAudioSource;
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != null && other.transform.parent.CompareTag("Enemy"))
        {
            shotAudioSource.Play();
            other.GetComponentInParent<EnemyController>().Dead();
        }
    }
}
