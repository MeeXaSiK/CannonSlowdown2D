using UnityEngine;

namespace Scripts.Projectiles
{
    public class Projectile2D : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody2D;
        
        public Rigidbody2D Rigidbody2D => _rigidbody2D;
    }
}