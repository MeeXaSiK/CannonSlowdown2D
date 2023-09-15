using System;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Core.Extensions;
using Scripts.Zones.Data;
using UnityEngine;

namespace Scripts.Zones.Base
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class RigidbodyAffectionZone2D : MonoBehaviour
    {
        private readonly Dictionary<Rigidbody2D, RigidbodyData2D> _initialRigidbodiesDataMap = new(
            Constants.DefaultCollectionCapacity);

        private Collider2D _zoneCollider2D;
        private bool _isAffectionEnabled = true;
        private bool _hasEnteredRigidbodies;
        private bool _shouldColliderBeDisabled;

        private void Awake()
        {
            _zoneCollider2D = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
#if DEBUG
            if (_shouldColliderBeDisabled && _zoneCollider2D.enabled)
            {
                Debug.LogWarning("The active of a collider belonging to this zone should be regulated " +
                                 "only by this zone to avoid incorrect behaviour!", _zoneCollider2D);
            }
#endif
            _zoneCollider2D.enabled = true;
            _shouldColliderBeDisabled = false;
        }
        
        private void OnDisable()
        {
            _zoneCollider2D.enabled = false;
            _shouldColliderBeDisabled = true;
        }
        
#if DEBUG
        private void Start()
        {
            _zoneCollider2D.LogWarningIfTriggerDisabled();
        }
#endif
        private void FixedUpdate()
        {
            if (_isAffectionEnabled == false)
                return;
            
            if (_hasEnteredRigidbodies == false)
                return;
            
            foreach (RigidbodyData2D enteredRigidbody in _initialRigidbodiesDataMap.Values)
            {
                OnRigidbodyStay(enteredRigidbody);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetRigidbody(out Rigidbody2D attachedRigidbody) == false)
                return;
            
            if (TryAddRigidbody(attachedRigidbody, out RigidbodyData2D initialData))
            {
                if (_isAffectionEnabled)
                {
                    OnRigidbodyEntered(initialData);
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetRigidbody(out Rigidbody2D attachedRigidbody) == false)
                return;
            
            if (TryRemoveRigidbody(attachedRigidbody, out RigidbodyData2D initialData))
            {
                if (_isAffectionEnabled)
                {
                    OnRigidbodyExit(initialData);
                }
            }
        }

        public void EnableAffection()
        {
            if (_isAffectionEnabled)
                return;
            
            AffectEnteredRigidbodies();
            
            _isAffectionEnabled = true;
        }

        public void DisableAffection()
        {
            if (_isAffectionEnabled == false)
                return;
            
            RevertEnteredRigidbodies();

            _isAffectionEnabled = false;
        }
        
        private void ForEachRigidbodyInitialData(Action<RigidbodyData2D> action)
        {
#if DEBUG
            if (action == null)
                throw new ArgumentNullException(nameof(action));
#endif
            foreach (RigidbodyData2D initialData in _initialRigidbodiesDataMap.Values)
            {
                action.Invoke(initialData);
            }
        }

        private bool TryAddRigidbody(Rigidbody2D newRigidbody, out RigidbodyData2D data)
        {
            if (_initialRigidbodiesDataMap.ContainsKey(newRigidbody) == false)
            {
                data = new RigidbodyData2D(newRigidbody);
                _initialRigidbodiesDataMap.Add(newRigidbody, data);
                _hasEnteredRigidbodies = true;
                return true;
            }
            
            data = default;
            return false;
        }

        private bool TryRemoveRigidbody(Rigidbody2D rigidbodyToRemove, out RigidbodyData2D data)
        {
            if (_initialRigidbodiesDataMap.TryGetValue(rigidbodyToRemove, out data))
            {
                _initialRigidbodiesDataMap.Remove(rigidbodyToRemove);
                _hasEnteredRigidbodies = _initialRigidbodiesDataMap.Count > 0;
                return true;
            }

            return false;
        }

        private void AffectEnteredRigidbodies()
        {
            ForEachRigidbodyInitialData(data => OnRigidbodyEntered(data));
        }
        
        private void RevertEnteredRigidbodies()
        {
            ForEachRigidbodyInitialData(data => OnRigidbodyExit(data));
        }

        protected virtual void OnRigidbodyEntered(in RigidbodyData2D initialRigidbodyData2D) { }
        protected virtual void OnRigidbodyStay(in RigidbodyData2D initialRigidbodyData2D) { }
        protected virtual void OnRigidbodyExit(in RigidbodyData2D initialRigidbodyData2D) { }
    }
}