
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
        public static bool UseCustomSkin { get; set; } = true;

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

        private bool thing = false;
        private void OnGUI()
        {
            if (UseCustomSkin)
                GUI.skin = CustomSkin;

            ApplyScaling();
            CleanDrawers();
            DrawDrawers();

            GUI.Box(new Rect(200, 200, 100, 100), "Hey");
            GUI.Box(new Rect(300, 300, 50, 80), "Hey 2");
            GUILayout.Button("I am a button");
            GUILayout.RepeatButton("Repeat button...");
            GUILayout.Space(10);
            thing = GUILayout.Toggle(thing, "Some toggle");
        }
    }

    public delegate void DrawUI();
}
