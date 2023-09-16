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
        
        private RigidbodyHandler2D _addRigidbodyInMap;
        private RigidbodyHandler2D _removeRigidbodyFromMap;
        
        private RigidbodyDataCallback2D _raiseOnRigidbodyEnter;
        private RigidbodyDataCallback2D _raiseOnRigidbodyExit;

        private delegate void RigidbodyHandler2D(Rigidbody2D rigidbody2D, RigidbodyDataCallback2D callback);
        private delegate void RigidbodyDataCallback2D(in RigidbodyData2D data);
        
        public bool IsModifierEnabled { get; private set; } = true;

        private void Awake()
        {
            Initialize();
        }

        private void OnEnable()
        {
            EnableZoneWithCollider();
        }

        private void OnDisable()
        {
            DisableZoneWithCollider();

            if (_hasEnteredRigidbodies)
            {
                RevertModifiedRigidbodies();
                ClearRigidbodiesInitialDataMap();
            }
        }
        
        private void FixedUpdate()
        {
            HandleRigidbodiesOnStay();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleTriggerAction(other, _addRigidbodyInMap, _raiseOnRigidbodyEnter);
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            HandleTriggerAction(other, _removeRigidbodyFromMap, _raiseOnRigidbodyExit);
        }

        public void EnableModifier()
        {
            if (IsModifierEnabled == false)
            {
                ModifyEnteredRigidbodies();
                IsModifierEnabled = true;
            }
        }

        public void DisableModifier()
        {
            if (IsModifierEnabled)
            {
                RevertModifiedRigidbodies();
                IsModifierEnabled = false;
            }
        }

        private void Initialize()
        {
            _isComponentEnabled = enabled;
            _zoneCollider2D = GetComponent<Collider2D>();
            InitializeHandlers();
            InitializeCallbacks();
        }

        private void InitializeHandlers()
        {
            _addRigidbodyInMap = AddRigidbodyInMap;
            _removeRigidbodyFromMap = RemoveRigidbodyFromMap;
        }

        private void InitializeCallbacks()
        {
            _raiseOnRigidbodyEnter = OnRigidbodyEnter;
            _raiseOnRigidbodyExit = OnRigidbodyExit;
        }
        
        private void EnableZoneWithCollider()
        {
#if DEBUG
            ZoneErrorsDetection.CheckForCorrectActive(_isComponentEnabled, _zoneCollider2D,
                errorDetectedCallback: () => _zoneCollider2D.enabled = false);
            
            _zoneCollider2D.LogWarningIfTriggerDisabled();
#endif
            _isComponentEnabled = true;
            _zoneCollider2D.enabled = true;
        }

        private void DisableZoneWithCollider()
        {
#if DEBUG
            ZoneErrorsDetection.CheckForCorrectActive(_isComponentEnabled, _zoneCollider2D, 
                errorDetectedCallback: () => _zoneCollider2D.enabled = true);
#endif
            _zoneCollider2D.enabled = false;
            _isComponentEnabled = false;
        }

        private void HandleRigidbodiesOnStay()
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

        private void HandleTriggerAction(Collider2D other, RigidbodyHandler2D rigidbodyHandler2D,
            RigidbodyDataCallback2D callback)
        {
#if DEBUG
            if (rigidbodyHandler2D == null)
                throw new ArgumentNullException(nameof(rigidbodyHandler2D));
#endif
            if (_isComponentEnabled == false)
                return;
            
            if (other.TryGetRigidbody(out Rigidbody2D attachedRigidbody) == false)
                return;

            rigidbodyHandler2D.Invoke(attachedRigidbody, callback);
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

        private void AddRigidbodyInMap(Rigidbody2D newRigidbody, RigidbodyDataCallback2D callback)
        {
            if (_initialRigidbodiesDataMap.ContainsKey(newRigidbody))
                return;
            
            RigidbodyData2D initialData = new(newRigidbody);
            
            _initialRigidbodiesDataMap.Add(newRigidbody, initialData);
            _hasEnteredRigidbodies = true;

            TryInvokeCallback(initialData, callback);
        }

        private void RemoveRigidbodyFromMap(Rigidbody2D rigidbodyToRemove, RigidbodyDataCallback2D callback2D)
        {
            if (_initialRigidbodiesDataMap.TryGetValue(rigidbodyToRemove, out RigidbodyData2D initialData) == false)
                return;
            
            _initialRigidbodiesDataMap.Remove(rigidbodyToRemove);
            _hasEnteredRigidbodies = _initialRigidbodiesDataMap.Count > 0;

            TryInvokeCallback(initialData, callback2D);
        }

        private bool TryInvokeCallback(in RigidbodyData2D data, 
            RigidbodyDataCallback2D callback)
        {
#if DEBUG
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
#endif
            if (IsModifierEnabled == false) 
                return false;
            
            callback.Invoke(data);
            return true;
        }

        private void ModifyEnteredRigidbodies()
        {
            ForEachRigidbodyInitialData(data => OnRigidbodyEnter(data));
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

        protected virtual void OnRigidbodyEnter(in RigidbodyData2D initialRigidbodyData2D) { }
        protected virtual void OnRigidbodyStay(in RigidbodyData2D initialRigidbodyData2D) { }
        protected virtual void OnRigidbodyExit(in RigidbodyData2D initialRigidbodyData2D) { }
    }
}