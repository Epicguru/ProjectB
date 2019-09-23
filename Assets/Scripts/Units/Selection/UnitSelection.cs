
using System.Collections.Generic;
using UnityEngine;

namespace ProjectB.Units.Selection
{
    public class UnitSelection : MonoBehaviour
    {
        public static bool IsSelecting { get; private set; } = false;
        public KeyCode SelectKey = KeyCode.Mouse0;
        public LayerMask Mask;

        private static LayerMask staticMask;
        private static List<Unit> tempUnits = new List<Unit>();
        private static KeyCode staticSelectKey = KeyCode.Mouse0;

        private static RaycastHit2D[] hits = new RaycastHit2D[32];
        private static Collider2D[] colliders = new Collider2D[64];
        private static Vector2 mouseStartPixels;
        private static Vector2 mouseStart;

        private static int singleSelectIndex = 0;

        private void Awake()
        {
            staticMask = Mask;
            staticSelectKey = SelectKey;
        }

        private void Update()
        {
            bool keyPressed = Input.GetKey(SelectKey);
            bool keyUp = Input.GetKeyUp(SelectKey);
            bool keyDown = Input.GetKeyDown(SelectKey);

            // On key down.
            if (keyDown)
            {
                mouseStartPixels = Input.mousePosition;
                mouseStart = InputManager.MousePos;
            }

            // Upon key up...
            if (keyUp)
            {
                // If mouse is in UI, stop.
                if (InputManager.IsMouseInUI)
                {
                    return;
                }

                // Check if mouse has moved less than 10 pixels from start.
                Vector2 mouseDelta = (Vector2)Input.mousePosition - mouseStartPixels;
                if(mouseDelta.sqrMagnitude <= 100f)
                {
                    // Un-select all units, if not holding shift.
                    if (!Input.GetKey(KeyCode.LeftShift))
                        Unit.DeselectAll();

                    // Select it, while 'scrolling' through all units under mouse upon multiple clicks.
                    var underMouse = GetUnitsUnderMouse();
                    int count = underMouse.Count;
                    if (singleSelectIndex >= count)
                        singleSelectIndex = 0;

                    if (count > 0)
                    {
                        underMouse[singleSelectIndex].IsSelected = true;
                    }

                    singleSelectIndex++;
                }
                else
                {
                    // Un-select all units, if not holding shift.
                    if (!Input.GetKey(KeyCode.LeftShift))
                        Unit.DeselectAll();

                    // Select them all.
                    var underMouse = GetUnitsInBounds(mouseStart, InputManager.MousePos);
                    foreach (var unit in underMouse)
                    {
                        unit.IsSelected = true;
                    }
                }                
            }
        }

        public static void GL_Draw()
        {
            if (Input.GetKey(staticSelectKey))
            {
                Color color = Color.grey;
                Vector2 start = mouseStart;
                Vector2 end = InputManager.MousePos;

                GameCamera.DrawLine(start, new Vector2(start.x, end.y), color);
                GameCamera.DrawLine(new Vector2(start.x, end.y), end, color);
                GameCamera.DrawLine(end, new Vector2(end.x, start.y), color);
                GameCamera.DrawLine(new Vector2(end.x, start.y), start, color);
            }
        }

        public static List<Unit> GetUnitsUnderMouse()
        {
            tempUnits.Clear();
            Vector2 mousePos = InputManager.MousePos;

            int count = Physics2D.GetRayIntersectionNonAlloc(new Ray(new Vector3(mousePos.x, mousePos.y, -100f), Vector3.forward), hits, 200f, staticMask);
            if(count > hits.Length)
            {
                Debug.LogError($"Number of intersection hits ({count}) exceeded the maximum of {hits.Length}.");
                count = hits.Length;
            }

            for (int i = 0; i < count; i++)
            {
                var hit = hits[i];
                var unit = hit.transform.GetComponentInParent<Unit>();
                if (unit != null && !tempUnits.Contains(unit))
                    tempUnits.Add(unit);
            }

            return tempUnits;
        }

        public static List<Unit> GetUnitsInBounds(Vector2 min, Vector2 max)
        {
            tempUnits.Clear();

            int count = Physics2D.OverlapAreaNonAlloc(min, max, colliders, staticMask);
            if (count > hits.Length)
            {
                Debug.LogError($"Number of intersection hits ({count}) exceeded the maximum of {hits.Length}.");
                count = hits.Length;
            }

            for (int i = 0; i < count; i++)
            {
                Collider2D collider = colliders[i];
                var unit = collider.transform.GetComponentInParent<Unit>();
                if (unit != null && !tempUnits.Contains(unit))
                    tempUnits.Add(unit);
            }

            return tempUnits;
        }
    }
}
