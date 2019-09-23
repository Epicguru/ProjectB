
using UnityEngine;

namespace ProjectB.DevTools
{
    public abstract class EditorTool : MonoBehaviour
    {
        public string Name = "Tool Name";
        public virtual string WindowTitle
        {
            get
            {
                return Name;
            }
        }
        public bool IsEnabled = false;
        public Rect WindowRect;
        public EditorTools Tools { get; set; }

        public void DrawWindowInternal(int id)
        {
            DrawWindow();
            GUI.DragWindow();
        }

        public abstract void DrawWindow();
    }
}
