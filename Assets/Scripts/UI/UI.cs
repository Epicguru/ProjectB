
using UnityEngine;
using System.Collections.Generic;

namespace ProjectB.Interface
{
    [DefaultExecutionOrder(-100000)]
    public class UI : MonoBehaviour
    {
        [GameVar(Name = "UI_Scale")]
        public static float GlobalScale = 1f;

        [GameVar(Name = "UI_DrawerCount")]
        public static int UIDrawerCount { get { return drawers.Count; } }

        [GameVar(Name = "UI_UseCustomSkin")]
        public static bool UseCustomSkin { get; set; } = false;

        public GUISkin CustomSkin;

        public static float ScreenWidth { get { return Screen.width / GlobalScale; } }
        public static float ScreenHeight { get { return Screen.height / GlobalScale; } }

        private static List<DrawUI> drawers = new List<DrawUI>();

        public static void AddDrawer(DrawUI method)
        {
            if(method != null)
            {
                if (drawers.Contains(method))
                    return;

                drawers.Add(method);
            }
        }

        private static void CleanDrawers()
        {
            for (int i = 0; i < drawers.Count; i++)
            {
                if(drawers[i] == null)
                {
                    drawers.RemoveAt(i);
                    i--;
                }
            }
        }

        private static void RemoveDrawer(DrawUI method)
        {
            if (method == null)
                return;

            if (drawers.Contains(method))
            {
                drawers.Remove(method);
            }
        }

        private static void DrawDrawers()
        {
            foreach (var method in drawers)
            {
                method?.Invoke();
            }
        }

        private static void ApplyScaling()
        {
            GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * GlobalScale);
        }

        private void OnGUI()
        {
            if (UseCustomSkin)
                GUI.skin = CustomSkin;

            // Reset anchour.
            Alignment = TextAnchor.UpperLeft;

            ApplyScaling();
            CleanDrawers();
            DrawDrawers();
        }

        #region GUI wrapper

        public static TextAnchor Alignment
        {
            get
            {
                return anchour;
            }
            set
            {
                if (anchour != value)
                    anchour = value;
            }
        }
        private static TextAnchor anchour = TextAnchor.UpperLeft;

        public static bool Button(Rect pos, GUIContent content)
        {
            return GUI.Button(pos, content);
        }

        public static bool Button(string content, bool repeat = false, params GUILayoutOption[] options)
        {
            if (!repeat)
                return GUILayout.Button(content, options);
            else
                return GUILayout.RepeatButton(content, options);
        }

        public static bool Button(GUIContent content, bool repeat = false, params GUILayoutOption[] options) 
        {
            if (!repeat)
                return GUILayout.Button(content, options);
            else
                return GUILayout.RepeatButton(content, options);
        }

        public static void Label(Rect pos, GUIContent content)
        {
            var style = GUI.skin.label;
            var old = style.alignment;
            style.alignment = Alignment;
            GUI.Label(pos, content);
            style.alignment = old;
        }

        public static void Label(string content, params GUILayoutOption[] options)
        {
            var style = GUI.skin.label;
            var old = style.alignment;
            style.alignment = Alignment;
            GUILayout.Label(content, options);
            style.alignment = old;
        }

        public static void Label(GUIContent content, params GUILayoutOption[] options)
        {
            var style = GUI.skin.label;
            var old = style.alignment;
            style.alignment = Alignment;
            GUILayout.Label(content, options);
            style.alignment = old;
        }

        #endregion
    }

    public delegate void DrawUI();
}
