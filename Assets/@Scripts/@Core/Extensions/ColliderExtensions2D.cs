using UnityEngine;

namespace Scripts.Core.Extensions
{
    public static class ColliderExtensions2D
    {
        public static bool TryGetRigidbody(this Collider2D collider2D, out Rigidbody2D rigidbody2D)
        {
            rigidbody2D = collider2D.attachedRigidbody;
            
            return rigidbody2D != null;
        }

        public static void LogWarningIfTriggerDisabled(this Collider2D collider2D)
        {
            if (collider2D.isTrigger == false)
            {
                Debug.LogWarning(
                    $"The '{nameof(collider2D.isTrigger)}' option of this collider is disabled!", collider2D);
            }
        }
    }
}