
using UnityEngine;
using UnityEngine.EventSystems;

namespace ProjectB
{
    [DefaultExecutionOrder(-100)]
    public class InputManager : MonoBehaviour
    {
        public static Vector2 MousePos { get; private set; }
        public static Bounds CameraBounds { get; private set; }
        public static bool IsMouseInUI { get; private set; }


        public Camera Camera;
        public EventSystem Events;

        private void Update()
        {
            MousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 min = Camera.ScreenToWorldPoint(Vector2.zero);
            min.z = Camera.transform.position.z;
            Vector3 max = Camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            max.z = min.z + Camera.farClipPlane;
            var b = CameraBounds;
            b.min = min;
            b.max = max;
            CameraBounds = b;

            IsMouseInUI = Events.IsPointerOverGameObject();
        }
    }
}

