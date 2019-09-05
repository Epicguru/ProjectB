
using System;

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
