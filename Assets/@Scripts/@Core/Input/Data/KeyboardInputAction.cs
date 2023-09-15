using System;
using UnityEngine;

namespace Scripts.Core.Input.Data
{
    public readonly struct KeyboardInputAction
    {
        public readonly KeyCode KeyCode;
        public readonly Action Action;

        public KeyboardInputAction(KeyCode keyCode, Action action)
        {
            KeyCode = keyCode;
            Action = action;
        }
    }
}