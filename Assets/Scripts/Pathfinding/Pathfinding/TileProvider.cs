using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThreadedPathfinding
{
    public interface ITileProvider
    {
        bool IsTileWalkable(int x, int y);
    }
}

