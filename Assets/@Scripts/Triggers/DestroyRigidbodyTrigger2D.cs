using Scripts.Core.Extensions;
using UnityEngine;

namespace Scripts.Triggers
{
    [RequireComponent(typeof(Collider2D))]
    public class DestroyRigidbodyTrigger2D : MonoBehaviour
    {
#if DEBUG
        private void Start()
        {
            GetComponent<Collider2D>().LogWarningIfTriggerDisabled();
        }
#endif
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.attachedRigidbody != null)
            {
                Destroy(other.gameObject);
            }
        }
    }
}