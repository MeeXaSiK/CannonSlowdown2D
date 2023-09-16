using System;
using Scripts.Core;
using Scripts.Projectiles;
using UnityEngine;

namespace Scripts.Cannons
{
    public class Cannon2D : MonoBehaviour
    {
        [Header("Force")]
        [SerializeField] private ForceMode2D _forceMode2D = ForceMode2D.Impulse;
        [SerializeField, Min(Constants.Zero)] private float _shotForce = 10f;
        
        [Header(Constants.Headers.ExternalDependencies)]
        [SerializeField] private Projectile2D _projectilePrefab;
        [SerializeField] private Transform _projectileLaunchPoint;

        public void PerformShot()
        {
#if DEBUG
            if (_projectilePrefab == null)
                throw new NullReferenceException("You didn't set the projectile prefab!");
#endif
            Vector3 projectileSpawnPosition = _projectileLaunchPoint.position;
            Vector3 direction = transform.up;
            Vector2 shotForce = (Vector2) direction * _shotForce;
            
            Projectile2D projectile = Instantiate(_projectilePrefab, projectileSpawnPosition, Quaternion.identity);
            projectile.Rigidbody2D.AddForce(shotForce, _forceMode2D);
        }
    }
}