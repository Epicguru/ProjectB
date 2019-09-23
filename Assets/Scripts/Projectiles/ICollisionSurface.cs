
namespace ProjectB.Ballistics
{
    public interface ICollisionSurface
    {
        bool IsEnabled();
        bool IsPenetrable();
        float GetToughness();
        float GetReflectionChance();
        PoolObject GetHitEffect();
        Health GetHealth();
    }
}
