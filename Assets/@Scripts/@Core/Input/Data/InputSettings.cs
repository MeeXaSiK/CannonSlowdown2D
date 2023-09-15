using System;
using UnityEngine;

namespace Scripts.Core.Input.Data
{
    [Serializable]
    public class InputSettings
    {
        public KeyCode LeftCannonKey = KeyCode.Q;
        public KeyCode RightCannonKey = KeyCode.E;
        public KeyCode DisableModifierZoneKey = KeyCode.D;
        public KeyCode EnableModifierZoneKey = KeyCode.A;
        public KeyCode ReloadSceneKey = KeyCode.R;
        public KeyCode QuitApplicationKey = KeyCode.Escape;
    }
}