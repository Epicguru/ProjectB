using Priority_Queue;
using System.Collections.Generic;
using UnityEngine;

namespace ThreadedPathfinding.Internal
{
    public class Pathfinding
    {
        public const int MAX = 5500;
        public const float DIAGONAL_DST = 1.41421356237f;

        private FastPriorityQueue<PNode> open = new FastPriorityQueue<PNode>(MAX);
        private Dictionary<PNode, PNode> cameFrom = new Dictionary<PNode, PNode>();
        private Dictionary<PNode, float> costSoFar = new Dictionary<PNode, float>();
        private List<PNode> near = new List<PNode>();
        private bool left, right, below, above;

        public Pathfinding()
        {

        }

        /// <summary>
        /// Attempts to trace a path from the startin point to the end point, sychronously.
        /// May fail for a number of reasons (see <see cref="PathfindingResult"/>).
        /// </summary>
        /// <param name="startX">The starting X position.</param>
        /// <param name="startY">The starting Y position.</param>
        /// <param name="endX">The target (end) X position.</param>
        /// <param name="endY">The target (end) Y position.</param>
        /// <param name="provider">The tile provider.</param>
        /// <param name="inPath">An optional path. If null, a new list is created. If not null, this list is used as the output path.</param>
        /// <param name="path">The output path array</param>
        /// <returns></returns>
        public PathfindingResult Run(int startX, int startY, int endX, int endY, ITileProvider provider, List<PNode> path)
        {
            // Null checks.
            if (provider == null)
            {
                return PathfindingResult.ERROR_NO_TILE_PROVIDER;
            }
            if(path == null)
            {
                return PathfindingResult.ERROR_PATH_ARRAY_NULL;
            }

            // Validate start and end points.
            if (!provider.IsTileWalkable(startX, startY))
            {
                return PathfindingResult.ERROR_START_NOT_WALKABLE;
            }
            if (!provider.IsTileWalkable(endX, endY))
            {
                return PathfindingResult.ERROR_END_NOT_WALKABLE;
            }

            // Clear everything up.
            Clear();

            var start = PNode.Create(startX, startY);
            var end = PNode.Create(endX, endY);

            // Check the start/end relationship.
            if (start.Equals(end))
            {
                return PathfindingResult.ERROR_START_IS_END;
            }

            // Add the starting point to all relevant structures.
            open.Enqueue(start, 0f);
            cameFrom[start] = start;
            costSoFar[start] = 0f;

            int count;
            while ((count = open.Count) > 0)
            {
                // Detect if the current open amount exceeds the capacity.
                // This only happens in very large open areas. Corridors and hallways will never cause this, not matter how large the actual path length.
                if (count >= MAX - 8)
                {
                    return PathfindingResult.ERROR_PATH_TOO_LONG;
                }

                var current = open.Dequeue();

                if (current.Equals(end))
                {
                    // We found the end of the path!
                    TracePath(end, path);
                    return PathfindingResult.SUCCESSFUL;
                }

                // Get all neighbours (tiles that can be walked on to)
                var neighbours = GetNear(current, provider);
                foreach (PNode n in neighbours)
                {
                    float newCost = costSoFar[current] + GetCost(current, n); // Note that this could change depending on speed changes per-tile. Currently not implemented.

                    if (!costSoFar.ContainsKey(n) || newCost < costSoFar[n])
                    {
                        costSoFar[n] = newCost;
                        float priority = newCost + Heuristic(current, n);
                        open.Enqueue(n, priority);
                        cameFrom[n] = current;
                    }
                }
            }

            return PathfindingResult.ERROR_PATH_NOT_FOUND;
        }

        private void TracePath(PNode end, List<PNode> path)
        {
            path.Clear();
            PNode child = end;

            bool run = true;
            while (run)
            {
                PNode previous = cameFrom[child];
                path.Add(child);
                if (previous != null && child != previous)
                {
                    child = previous;
                }
                else
                {
                    run = false;
                }
            }

            path.Reverse();
        }

        public void Clear()
        {
            costSoFar.Clear();
            cameFrom.Clear();
            near.Clear();
            open.Clear();
        }

        private float Abs(float x)
        {
            if (x < 0)
                return -x;
            else
                return x;
        }

        private float Heuristic(PNode a, PNode b)
        {
            // Gives a rough distance.
            return Abs(a.X - b.X) + Abs(a.Y - b.Y);
        }

        private float GetCost(PNode a, PNode b)
        {
            // Only intended for neighbours.

            // Is directly horzontal
            if (Abs(a.X - b.X) == 1 && a.Y == b.Y)
            {
                return 1;
            }

            // Directly vertical.
            if (Abs(a.Y - b.Y) == 1 && a.X == b.X)
            {
                return 1;
            }

            // Assume that it is on one of the corners.
            return DIAGONAL_DST;
        }

        private List<PNode> GetNear(PNode node, ITileProvider provider)
        {
            // Want to add nodes connected to the center node, if they are walkable.
            // This code stops the pathfinder from cutting corners, and going through walls that are diagonal from each other.

            near.Clear();

            // Left
            left = false;
            if (provider.IsTileWalkable(node.X - 1, node.Y))
            {
                near.Add(PNode.Create(node.X - 1, node.Y));
                left = true;
            }

            // Right
            right = false;
            if (provider.IsTileWalkable(node.X + 1, node.Y))
            {
                near.Add(PNode.Create(node.X + 1, node.Y));
                right = true;
            }

            // Above
            above = false;
            if (provider.IsTileWalkable(node.X, node.Y + 1))
            {
                near.Add(PNode.Create(node.X, node.Y + 1));
                above = true;
            }

            // Below
            below = false;
            if (provider.IsTileWalkable(node.X, node.Y - 1))
            {
                near.Add(PNode.Create(node.X, node.Y - 1));
                below = true;
            }

            // Above-Left
            if (left && above)
            {
                if (provider.IsTileWalkable(node.X - 1, node.Y + 1))
                {
                    near.Add(PNode.Create(node.X - 1, node.Y + 1));
                }
            }

            // Above-Right
            if (right && above)
            {
                if (provider.IsTileWalkable(node.X + 1, node.Y + 1))
                {
                    near.Add(PNode.Create(node.X + 1, node.Y + 1));
                }
            }

            // Below-Left
            if (left && below)
            {
                if (provider.IsTileWalkable(node.X - 1, node.Y - 1))
                {
                    near.Add(PNode.Create(node.X - 1, node.Y - 1));
                }
            }

            // Below-Right
            if (right && below)
            {
                if (provider.IsTileWalkable(node.X + 1, node.Y - 1))
                {
                    near.Add(PNode.Create(node.X + 1, node.Y - 1));
                }
            }

            return near;
        }
    }
}
