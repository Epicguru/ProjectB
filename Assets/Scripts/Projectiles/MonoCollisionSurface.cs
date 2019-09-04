
using UnityEngine;

public class MonoCollisionSurface : MonoBehaviour, ICollisionSurface
{
    public static ICollisionSurface DefaultTrigger
    {
        get
        {
            if (_defaultDataT == null)
                _defaultDataT = new DefaultTriggerC();
            return _defaultDataT;
        }
    }
    private static ICollisionSurface _defaultDataT;

    public static ICollisionSurface DefaultSolid
    {
        get
        {
            if (_defaultDataS == null)
                _defaultDataS = new DefaultSolidC();
            return _defaultDataS;
        }
    }
    private static ICollisionSurface _defaultDataS;

    private class DefaultTriggerC : ICollisionSurface
    {
        public bool IsEnabled() { return false; }
        public bool IsPenetrable() { return true; }
        public float GetToughness() { return 1f; }
        public PoolObject GetHitEffect() { return null; }
        public Health GetHealth() { return null; }
        public float GetReflectionChance() { return 1f; }
    }
    private class DefaultSolidC : ICollisionSurface
    {
        public bool IsEnabled() { return true; }
        public bool IsPenetrable() { return false; }
        public float GetToughness() { return 1f; }
        public PoolObject GetHitEffect() { return null; }
        public Health GetHealth() { return null; }
        public float GetReflectionChance() { return 1f; }
    }

    public static Health GetParentHealth(Transform t)
    {
        if (t == null)
            return null;

        return t.GetComponentInParent<Health>();
    }

    public bool Enabled = true;
    public bool IsEnabled()
    {
        return Enabled;
    }

    public bool Penetrable = true;
    public bool IsPenetrable()
    {
        return Penetrable;
    }

    [Min(0f)]
    public float Toughness = 1f;
    public float GetToughness()
    {
        return this.Toughness;
    }

    public PoolObject HitEffect;
    public PoolObject GetHitEffect()
    {
        return HitEffect;
    }

    [Range(0f, 5f)]
    [Tooltip("The base chance multiplier that this surface has to reflect a projectile, if the projectile does not penetrate it. The chance will also be affected by the penetration capability, reflection chance and angle of impact of a projectile.")]
    public float ReflectionChance = 1f;
    public float GetReflectionChance()
    {
        return ReflectionChance;
    }

    [Header("Health")]
    [Tooltip("If true, when Health is null then a Health component is searched for in this object or a parent object.")]
    public bool AutoGetHealth = true;
    public Health Health;
    public Health GetHealth()
    {
        if (Health == null && AutoGetHealth)
        {
            Health = GetParentHealth(this.transform);
        }
        return Health;
    }
}
