
using System;

namespace ProjectB.Commands
{
    public struct CommandDrawable
    {
        public Action DrawAction;

        public CommandDrawable(Action draw)
        {
            this.DrawAction = draw;
        }

        public void Draw()
        {
            if (DrawAction != null)
                DrawAction.Invoke();
        }
    }
}
