
using System;
using UnityEngine;

namespace ProjectB.Interface
{
    public class IMGUIWindow
    {
        public int ID { get; private set; }
        public string Name = "Window";
        public Rect Rect;
        public bool CanDrag = true;
        public object[] Data = new object[256];

        public Action DrawAction;

        public IMGUIWindow(int id, Rect rect, string name, Action drawAction)
        {
            this.ID = id;
            this.DrawAction = drawAction;
            this.Rect = rect;
            this.Name = name;
        }

        public void OnGUI()
        {
            Rect = GUI.Window(ID, Rect, Draw, Name);
            Rect.ClampTo(new Rect(0, 0, UI.ScreenWidth, UI.ScreenHeight));
        }

        private void Draw(int id)
        {
            if (id == this.ID)
            {
                DrawAction.Invoke();
                if (CanDrag)
                    GUI.DragWindow();
            }
        }
    }
}

