
namespace ThreadedPathfinding
{
    public enum PathfindingResult : byte
    {
        SUCCESSFUL,
        CANCELLED,
        ERROR_START_IS_END,
        ERROR_PATH_TOO_LONG,
        ERROR_START_NOT_WALKABLE,
        ERROR_END_NOT_WALKABLE,
        ERROR_PATH_NOT_FOUND,
        ERROR_PATH_ARRAY_NULL,
        ERROR_NO_TILE_PROVIDER,
        ERROR_INTERNAL
    }
}

