using System;
using UnityEngine;

namespace Scripts.Core.Input.Data
{
    [Serializable]
    public class InputSettings
    {
        public KeyCode leftCannonKey = KeyCode.Q;
        public KeyCode rightCannonKey = KeyCode.E;
        public KeyCode enableAffectionZoneKey = KeyCode.A;
        public KeyCode disableAffectionZoneKey = KeyCode.D;
        public KeyCode reloadSceneKey = KeyCode.R;
        public KeyCode quitApplicationKey = KeyCode.Escape;
    }
}