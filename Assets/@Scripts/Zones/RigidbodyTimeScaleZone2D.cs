using Scripts.Core.Extensions;
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

        protected override void OnRigidbodyExit(in RigidbodyData2D initialRigidbodyData2D)
        {
            RevertTimeScale(initialRigidbodyData2D.Rigidbody2D);
        }

        private void ApplyTimeScale(Rigidbody2D rigidbodyToModify)
        {
            rigidbodyToModify.mass /= _timeScale;
            rigidbodyToModify.gravityScale *= _timeScale.GetSquaredNumber();
            rigidbodyToModify.velocity *= _timeScale;
            rigidbodyToModify.angularVelocity *= _timeScale;
            rigidbodyToModify.drag *= _timeScale;
            rigidbodyToModify.angularDrag *= _timeScale;
        }

        private void RevertTimeScale(Rigidbody2D rigidbodyToRevert)
        {
            rigidbodyToRevert.gravityScale /= _timeScale.GetSquaredNumber();
            rigidbodyToRevert.mass *= _timeScale;
            rigidbodyToRevert.velocity /= _timeScale;
            rigidbodyToRevert.angularVelocity /= _timeScale;
            rigidbodyToRevert.drag /= _timeScale;
            rigidbodyToRevert.angularDrag /= _timeScale;
        }
    }
}