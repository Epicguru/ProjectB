
using UnityEngine;

namespace ProjectB
{
    public static class GuiUtils
    {
        public static Texture2D Pixel = new Texture2D(1, 1);
        public static Camera Camera
        {
            get
            {
                if (_cam == null)
                {
                    _cam = Camera.main;
                }
                return _cam;
            }
        }
        private static Camera _cam;

        static GuiUtils()
        {
            Pixel.SetPixels32(new Color32[] { new Color32(255, 255, 255, 255) });
        }

        public static void DrawBox(Rect rect, Color color)
        {
            GUI.DrawTexture(rect, Pixel, ScaleMode.StretchToFill, true, 0f, color, 0f, 0f);
        }

        public static Rect ToScreenBounds(this Bounds bounds)
        {
            Rect r = new Rect();
            r.min = Camera.WorldToScreenPoint(bounds.min);
            r.max = Camera.WorldToScreenPoint(bounds.max);
            r.yMin = Screen.height - r.yMin;
            r.yMax = Screen.height - r.yMax;
            return r;
        }
    }
}

