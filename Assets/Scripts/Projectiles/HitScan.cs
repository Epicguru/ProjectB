
using UnityEngine;

/// <summary>
/// Class that allows you to fire a 'hitscan' projectile that has very simple collision detection (cannot penetrate or reflect).
/// </summary>
public static class HitScan
{
    private static LayerMask DefaultMask = LayerMask.GetMask("Default");
    private static RaycastHit2D[] hits = new RaycastHit2D[20];

    public static (ICollisionSurface surface, Vector2 hitPoint) Shoot(Vector2 origin, Vector2 direction, float range, float damage, bool display = true)
    {
        return Shoot(origin, direction, range, damage, DefaultMask, display);
    }

    public static (ICollisionSurface surface, Vector2 hitPoint) Shoot(Vector2 origin, Vector2 direction, float range, float damage, LayerMask mask, bool display = true)
    {
        int count = Physics2D.RaycastNonAlloc(origin, direction, hits, range, mask);
        if(count != 0)
        {
            for (int i = 0; i < count; i++)
            {
                var hit = hits[i];
                var surface = Projectile.GetCollisionDataOrDefault(hit.collider);

                if (!surface.IsEnabled())
                    continue;

                var h = surface.GetHealth();
                if(h != null)
                {
                    h.ChangeHealth(hit.collider, -damage);
                }

                if (display)
                {
                    DisplayShot(origin, hit.point);
                }

                return (surface, hit.point);
            }
        }

        Vector2 end = origin + direction.normalized * range;
        if (display)
        {
            DisplayShot(origin, end);
        }
        return (null, end);
    }

    private static HitscanEffect effect;
    public static void DisplayShot(Vector2 start, Vector2 end)
    {
        if (effect == null)
            effect = Spawnables.Get<HitscanEffect>("HitscanEffect");

        var spawned = PoolObject.Spawn(effect);
        spawned.Set(start, end);
    }
}
