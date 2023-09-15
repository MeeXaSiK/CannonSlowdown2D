using Scripts.Core;
using Scripts.Zones.Base;
using Scripts.Zones.Data;
using UnityEngine;

namespace Scripts.Zones
{
    public class SlowdownZone2D : RigidbodyAffectionZone2D
    {
        [SerializeField, Min(Constants.Zero)] private float _timeScale = 0.1f;

        private const float RigidbodyGravityScaleOnAffect = Constants.Zero;

        protected override void OnRigidbodyEntered(in RigidbodyData2D initialRigidbodyData2D)
        {
            ApplySlowdown(initialRigidbodyData2D.Rigidbody2D);
        }

        protected override void OnRigidbodyStay(in RigidbodyData2D initialRigidbodyData2D)
        {
            ApplyGravity(initialRigidbodyData2D);
        }

        protected override void OnRigidbodyExit(in RigidbodyData2D initialRigidbodyData2D)
        {
            RevertSlowdown(initialRigidbodyData2D);
        }

        private void ApplySlowdown(Rigidbody2D rigidbodyToAffect)
        {
            rigidbodyToAffect.gravityScale = RigidbodyGravityScaleOnAffect;
            rigidbodyToAffect.mass /= _timeScale;
            rigidbodyToAffect.velocity *= _timeScale;
            rigidbodyToAffect.angularVelocity *= _timeScale;
            rigidbodyToAffect.drag *= _timeScale;
            rigidbodyToAffect.angularDrag *= _timeScale;
        }

        private void ApplyGravity(in RigidbodyData2D data)
        {
            var deltaTime = Time.fixedDeltaTime * _timeScale * data.GravityScale;
            
            data.Rigidbody2D.velocity += Physics2D.gravity / data.Rigidbody2D.mass * deltaTime;	
        }

        private void RevertSlowdown(in RigidbodyData2D data)
        {
            var rigidbodyToRevert = data.Rigidbody2D;
            
            rigidbodyToRevert.gravityScale = data.GravityScale;
            rigidbodyToRevert.mass *= _timeScale;
            rigidbodyToRevert.velocity /= _timeScale;
            rigidbodyToRevert.angularVelocity /= _timeScale;
            rigidbodyToRevert.drag /= _timeScale;
            rigidbodyToRevert.angularDrag /= _timeScale;
        }
    }
}