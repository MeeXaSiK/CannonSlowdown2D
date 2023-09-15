using System;
using Scripts.Zones.Data;
using UnityEngine;

namespace Scripts.Zones
{
    public static class ZoneErrorsDetection
    {
        public static void CheckRigidbodyForNull(in RigidbodyData2D data, bool zoneActive, Collider2D zoneCollider2D)
        {
            if (data.Rigidbody2D == null)
            {
                CheckForCorrectActive(zoneActive, zoneCollider2D);
                
                throw new Exception("One of the modified rigidbodies is null! " +
                                    "The modifier collider was probably disabled during the game " +
                                    "by an external component or manually!");
            }
        }

        public static void CheckForCorrectActive(bool zoneActive, Collider2D collider2D, 
            Action errorDetectedCallback = null)
        {
            if (zoneActive != collider2D.enabled)
            {
                Debug.LogError("The active of the zone modifier collider must be equal to the active of the zone!",
                    collider2D);
                
                errorDetectedCallback?.Invoke();
            }
        }
    }
}