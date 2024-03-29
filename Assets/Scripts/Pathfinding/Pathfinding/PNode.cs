﻿
using Priority_Queue;
using UnityEngine;

namespace ThreadedPathfinding
{
    [System.Serializable]
    public class PNode : FastPriorityQueueNode
    {
        public int X;
        public int Y;

        public static PNode Create(int x, int y)
        {
            // Unfortunately, I can't find any reasonable way to implement pooling.
            // It would have to work accross multiple threads at one, and more imporantly, still inherit from FastPriorityQueueNode and not break the HSPQ system.
            return new PNode(x, y);
        }

        private PNode(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vector2(PNode pn)
        {
            return new Vector2(pn.X, pn.Y);
        }

        public static implicit operator Vector3(PNode pn)
        {
            return new Vector3(pn.X, pn.Y, 0f);
        }

        public static explicit operator Vector2Int(PNode pn)
        {
            return new Vector2Int(pn.X, pn.Y);
        }

        public override bool Equals(object obj)
        {
            var other = (PNode)obj;
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X + Y * 7;
        }

        public override string ToString()
        {
            return "(" + X.ToString() + ", " + Y.ToString() + ")";
        }
    }
}
