
using UnityEngine;

namespace ProjectB.Units.Actions
{
    public static class ActionKeys
    {
        // TODO move this to InputManager.
        private static KeyCode[] keys = new KeyCode[]
        {
            KeyCode.E,
            KeyCode.Q,
            KeyCode.F,
            KeyCode.G
        };

        public static KeyCode Get(int index)
        {
            if (index < 0 || index >= keys.Length)
                return KeyCode.None;
            else
                return keys[index];
        }

        public static bool Set(int index, KeyCode code, bool allowDupes = false, KeyCode replacedDefault = KeyCode.None)
        {
            if (index < 0 || index >= keys.Length)
                return false;

            if (!allowDupes)
            {
                // Replace any current.
                for (int i = 0; i < keys.Length; i++)
                {
                    if (i != index)
                    {
                        if (keys[i] == code)
                        {
                            keys[i] = replacedDefault;
                        }
                    }
                }
            }

            keys[index] = code;
            return true;
        }
    }
}
