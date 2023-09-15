using System;
using System.Collections.Generic;
using Scripts.Core;
using Scripts.Core.Extensions;
using Scripts.Zones.Data;
using UnityEngine;

namespace Scripts.Zones.Base
{
    [RequireComponent(typeof(Collider2D))]
    public abstract class RigidbodyModifierZone2D : MonoBehaviour
    {
        private readonly Dictionary<Rigidbody2D, RigidbodyData2D> _initialRigidbodiesDataMap = new(
            Constants.DefaultCollectionCapacity);
        
        private Collider2D _zoneCollider2D;
        private bool _isComponentEnabled;
        private bool _hasEnteredRigidbodies;

        public bool IsModifierEnabled { get; private set; } = true;

        private void Awake()
        {
            _isComponentEnabled = enabled;
            _zoneCollider2D = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
#if DEBUG
            ZoneErrorsDetection.CheckForCorrectActive(_isComponentEnabled, _zoneCollider2D,
                errorDetectedCallback: () => _zoneCollider2D.enabled = false);
#endif
            _isComponentEnabled = true;
            _zoneCollider2D.enabled = true;
        }
        
        private void OnDisable()
        {
#if DEBUG
            ZoneErrorsDetection.CheckForCorrectActive(_isComponentEnabled, _zoneCollider2D, 
                errorDetectedCallback: () => _zoneCollider2D.enabled = true);
#endif
            _zoneCollider2D.enabled = false;
            _isComponentEnabled = false;

            if (_hasEnteredRigidbodies)
            {
                RevertModifiedRigidbodies();
                ClearRigidbodiesInitialDataMap();
            }
        }
        
#if DEBUG
        private void Start()
        {
            _zoneCollider2D.LogWarningIfTriggerDisabled();
        }
#endif
        private void FixedUpdate()
        {
            if (IsModifierEnabled == false)
                return;
            
            if (_hasEnteredRigidbodies == false)
                return;
            
            foreach (RigidbodyData2D enteredRigidbody in _initialRigidbodiesDataMap.Values)
            {
#if DEBUG
                ZoneErrorsDetection.CheckRigidbodyForNull(enteredRigidbody, _isComponentEnabled, _zoneCollider2D);
#endif
                OnRigidbodyStay(enteredRigidbody);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_isComponentEnabled == false)
                return;
            
            if (other.TryGetRigidbody(out Rigidbody2D attachedRigidbody) == false)
                return;
            
            if (TryAddRigidbody(attachedRigidbody, out RigidbodyData2D initialData))
            {
                if (IsModifierEnabled)
                {
                    OnRigidbodyEntered(initialData);
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (_isComponentEnabled == false)
                return;
            
            if (other.TryGetRigidbody(out Rigidbody2D attachedRigidbody) == false)
                return;
            
            if (TryRemoveRigidbody(attachedRigidbody, out RigidbodyData2D initialData))
            {
                if (IsModifierEnabled)
                {
                    OnRigidbodyExit(initialData);
                }
            }
        }

        public void EnableModifier()
        {
            if (IsModifierEnabled)
                return;
            
            ModifyEnteredRigidbodies();
            
            IsModifierEnabled = true;
        }

        public void DisableModifier()
        {
            if (IsModifierEnabled == false)
                return;
            
            RevertModifiedRigidbodies();

            IsModifierEnabled = false;
        }
        
        private void ForEachRigidbodyInitialData(Action<RigidbodyData2D> action)
        {
#if DEBUG
            if (action == null)
                throw new ArgumentNullException(nameof(action));
#endif
            foreach (RigidbodyData2D initialData in _initialRigidbodiesDataMap.Values)
            {
#if DEBUG
                ZoneErrorsDetection.CheckRigidbodyForNull(initialData, _isComponentEnabled, _zoneCollider2D);
#endif
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

        private void ModifyEnteredRigidbodies()
        {
            ForEachRigidbodyInitialData(data => OnRigidbodyEntered(data));
        }
        
        private void RevertModifiedRigidbodies()
        {
            ForEachRigidbodyInitialData(data => OnRigidbodyExit(data));
        }

        private void ClearRigidbodiesInitialDataMap()
        {
            _initialRigidbodiesDataMap.Clear();
            _hasEnteredRigidbodies = false;
        }

        protected virtual void OnRigidbodyEntered(in RigidbodyData2D initialRigidbodyData2D) { }
        protected virtual void OnRigidbodyStay(in RigidbodyData2D initialRigidbodyData2D) { }
        protected virtual void OnRigidbodyExit(in RigidbodyData2D initialRigidbodyData2D) { }
    }
}