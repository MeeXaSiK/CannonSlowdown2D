using Scripts.Core;
using Scripts.Zones.Base;
using Scripts.Zones.Data;
using UnityEngine;

namespace Scripts.Zones
{
    public class RigidbodyTimeScaleZone2D : RigidbodyModifierZone2D
    {
        [SerializeField, Min(MinTimeScale)] private float _timeScale = 0.1f;

        private const float MinTimeScale = 0.01f;
        
        protected override void OnRigidbodyEnter(in RigidbodyData2D initialRigidbodyData2D)
        {
            ApplyTimeScale(initialRigidbodyData2D.Rigidbody2D);
        }

        protected override void OnRigidbodyStay(in RigidbodyData2D initialRigidbodyData2D)
        {
            ApplyGravity(initialRigidbodyData2D);
        }

        protected override void OnRigidbodyExit(in RigidbodyData2D initialRigidbodyData2D)
        {
            RevertTimeScale(initialRigidbodyData2D);
        }

        private void ApplyTimeScale(Rigidbody2D rigidbodyToModify)
        {
            rigidbodyToModify.gravityScale = Constants.Zero;
            rigidbodyToModify.mass /= _timeScale;
            rigidbodyToModify.velocity *= _timeScale;
            rigidbodyToModify.angularVelocity *= _timeScale;
            rigidbodyToModify.drag *= _timeScale;
            rigidbodyToModify.angularDrag *= _timeScale;
        }

        private void ApplyGravity(in RigidbodyData2D data)
        {
            var deltaTime = Time.fixedDeltaTime * _timeScale * data.GravityScale;
            
            data.Rigidbody2D.velocity += Physics2D.gravity / data.Rigidbody2D.mass * deltaTime;	
        }

        private void RevertTimeScale(in RigidbodyData2D data)
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