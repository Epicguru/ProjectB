
namespace ThreadedPathfinding
{
    public enum PathfindingResult : byte
    {
        /// <summary>
        /// Path was found successfuly.
        /// </summary>
        SUCCESSFUL,
        /// <summary>
        /// Pathfinding request was cancelled before starting processing. [Not implemented]
        /// </summary>
        CANCELLED,
        /// <summary>
        /// Error: The starting position is the target position.
        /// </summary>
        ERROR_START_IS_END,
        /// <summary>
        /// Error: The path could not be found, the number of open nodes exceeded the capacity.
        /// </summary>
        ERROR_PATH_TOO_LONG,
        /// <summary>
        /// Error: The start position is not a walkable tile.
        /// </summary>
        ERROR_START_NOT_WALKABLE,
        /// <summary>
        /// Error: The target position is not a walkable tile.
        /// </summary>
        ERROR_END_NOT_WALKABLE,
        /// <summary>
        /// Error: All open nodes were explored, but a path could not be found between the start tile and the target tile.
        /// </summary>
        ERROR_PATH_NOT_FOUND,
        /// <summary>
        /// Error: The list passed into the pathfinding method is null.
        /// </summary>
        ERROR_PATH_ARRAY_NULL,
        /// <summary>
        /// Error: The tile provider object passed into the pathfinding method is null.
        /// </summary>
        ERROR_NO_TILE_PROVIDER,
        /// <summary>
        /// Error: An unspecified internal error in the pathfinding algorithm.
        /// </summary>
        ERROR_INTERNAL
    }
}

