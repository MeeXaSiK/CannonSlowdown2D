using UnityEngine;

namespace Scripts.Zones.Data
{
    public readonly struct RigidbodyData2D
    {
        public readonly Rigidbody2D Rigidbody2D;
        public readonly float GravityScale;

        public RigidbodyData2D(Rigidbody2D rigidbody2D)
        {
            Rigidbody2D = rigidbody2D;
            GravityScale = rigidbody2D.gravityScale;
        }
    }
}