using System;
using System.Collections.Generic;
using Scripts.Core.Input.Data;
using Scripts.Core.Interfaces;
using UnityEngine;

namespace Scripts.Core.Input
{
    public class KeyboardInputListener : IUpdatable
    {
        private readonly List<KeyboardInputAction> _keyboardInputActions = new(Constants.DefaultCollectionCapacity);

        public void BindAction(KeyCode keyCode, Action action)
        {
#if DEBUG
            if (action == null)
                throw new ArgumentNullException(nameof(action));
#endif
            _keyboardInputActions.Add(new KeyboardInputAction(keyCode, action));
        }
        
        void IUpdatable.OnUpdate()
        {
            for (var i = 0; i < _keyboardInputActions.Count; i++)
            {
                if (UnityEngine.Input.GetKeyDown(_keyboardInputActions[i].KeyCode))
                {
                    _keyboardInputActions[i].Action.Invoke();
                }
            }
        }
    }
}