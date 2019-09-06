
using JNetworking;
using Lidgren.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    [GameVar(Name = "ProjectileSpeed")]
    public static float SpeedMultiplier = 1f;

    public PoolObject PoolObject
    {
        get
        {
            if (_po == null)
                _po = GetComponent<PoolObject>();
            return _po;
        }
    }
    private PoolObject _po;

    [Header("Basic")]
    [Tooltip("The speed, in units per second, that the projectile initially travels at.")]
    public float Speed = 20f;
    [Tooltip("The direction vector that the projectile initially travels in.")]
    public Vector2 Direction;
    [Tooltip("Defines what layers the projectile will interact with.")]
    public LayerMask LayerMask;
    [Tooltip("The base damage that the projectile deals to Health components.")]
    public float Damage = 10f;

    [Header("Optimization")]
    public float TimeToDespawn = 5f;

    [Header("Penetration")]
    [Min(0f)]
    [Tooltip("The penetration potential of this projectile. It is compared to the surfaces 'toughness' value.")]
    public float PenetrationPower = 1f;
    [Tooltip("This number is subtracted from the penetration power upon every successful penetration, limiting how many surfaces the projectile can penetrate.")]
    [Range(0f, 10f)]
    public float PenetrationPenalty = 0.2f;
    [Tooltip("The minimum (x) and maximum (y) angle in degress that the projectile will rotate when it penetrates a surface.")]
    public Vector2 PenetrationSkew = new Vector2(0f, 20f);

    [Header("Reflection")]
    [Range(0f, 1f)]
    [Tooltip("The base chance for this projectile to reflect of a surface when it doesn't penetrate.")]
    public float BaseReflectionChance = 1f;
    public int MaxReflections = 1;
    [Range(0f, 1f)]
    [Tooltip("The reflection chance multiplier when the projectile hits a surface head-on (at 90 degrees)")]
    public float HeadOnCoefficient = 0.5f;

    [Header("Effects")]
    public PoolObject HitEffectPrefab;
    public LineRenderer Tracer;
    [Range(0f, 20f)]
    public float TracerLength = 5f;

    [Header("Sync")]
    public int Seed = 0;

    [Header("Debug")]
    public bool PathTraceEnabled = false;
    [Range(0, 30)]
    public int PathTraceIdleInterval = 5;
    public bool RemainAfterDeath = false;

    private static readonly RaycastHit2D[] hits = new RaycastHit2D[MAX_HITS];
    private const int MAX_TRACE_POINTS = 1000;
    private const int MAX_HITS = 50;
    private PredictableRandom random;
    private List<Vector2> pathTrace;
    private List<Collider2D> penetratedColliders = new List<Collider2D>();
    private int traceTimer;
    private int remainingReflections = 0;
    private bool destroyed = false;
    private float currentPenetrationPower;

    private void UponSpawn()
    {
        traceTimer = PathTraceIdleInterval;
        penetratedColliders.Clear();
        if (pathTrace != null)
            pathTrace.Clear();
        destroyed = false;
        remainingReflections = MaxReflections;
        currentPenetrationPower = PenetrationPower;

        if (Tracer != null)
            Tracer.positionCount = 0;

        Invoke("Kill", TimeToDespawn);
    }

    private void UponDespawn()
    {
        random = null;
        CancelInvoke("Kill");
    }

    private void Kill()
    {
        PoolObject.Despawn(this.PoolObject);
    }

    private void LateUpdate()
    {
        if (destroyed)
            return;

        if (random == null)
            random = new PredictableRandom(Seed);

        UpdateTracer();

        (Vector2 newPos, Vector2 newDir) = Step(transform.position, Direction, Time.deltaTime * Speed * SpeedMultiplier);
        transform.position = newPos;
        this.Direction = newDir;

        if (PathTraceEnabled)
            LogPath();

        UpdateTracer();

        transform.localEulerAngles = new Vector3(0f, 0f, this.Direction.ToAngle());
    }

    private (Vector2 newPos, Vector2 newDir) Step(Vector2 start, Vector2 dir, float distance)
    {
        bool oldQueries = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = false;
        float dst = distance;

        const int MAX_ITTERATIONS = 100;
        int itter = 0;
        while (true)
        {
            if (dst <= 0f)
                break;

            int hitCount = Physics2D.RaycastNonAlloc(start, dir, hits, dst, LayerMask);

            if (hitCount == 0f)
            {
                start = start + dir * dst;
                break;
            }

            if (hitCount > MAX_HITS)
            {
                Debug.LogWarning($"Projectile step has more raycast hits than it can handle ({hitCount}/{MAX_HITS}). Some objects may not be collided with correctly.");
                hitCount = MAX_HITS;
            }

            bool quit = false;
            bool anyProcessed = false;
            for (int i = 0; i < hitCount; i++)
            {
                var hit = hits[i];
                var surfaceData = GetCollisionDataOrDefault(hit.collider);

                // This object is in our path, and we will definitely collide with it this frame.
                // Now the only decision to make is how to interact with it.

                // Ignore if the surface is not enabled.
                if (!surfaceData.IsEnabled())
                    continue;

                bool canPenetrate = surfaceData.IsPenetrable() && surfaceData.GetToughness() <= this.currentPenetrationPower;

                if (!canPenetrate)
                {
                    float baseRefRoll = random.GetValue();

                    float chance = BaseReflectionChance * surfaceData.GetReflectionChance();

                    // The dot product of these normalized vectors will return 1 if it is a head on collision, or close to 0 if it is a glancing hit.
                    float dot = Mathf.Clamp01(Mathf.Abs(Vector2.Dot(hit.normal, dir)));
                        
                    chance *= Mathf.Lerp(1f, HeadOnCoefficient, dot);
                    //Debug.Log($"Chance: {chance*100f:F1}%, Rolled {baseRefRoll*100.0:F1} ({baseRefRoll <= chance})");
                    bool reflect = remainingReflections > 0 && baseRefRoll <= chance;
                    if (reflect)
                    {
                        // Reflect the direction of the projectile around the normal.
                        start = hit.point + hit.normal * 0.01f;
                        AddPathTracePoint(hit.point);
                        AddNewTracerVertex(hit.point);
                        anyProcessed = true;

                        UponReflected(hit, surfaceData, chance, dir);

                        dir = CalculateReflection(dir.normalized, hit.normal);
                        dst -= Vector2.Distance(start, hit.point);

                        remainingReflections--;
                    }
                    else
                    {
                        // Cannot penetrate, did not reflect.
                        // This projectile will definitely be stopped by this surface. End simulation.
                        start = hit.point;
                        AddPathTracePoint(hit.point);
                        quit = true;
                        anyProcessed = true;
                        destroyed = true;

                        AddNewTracerVertex(hit.point);
                        AddPathTracePoint(hit.point);
                        UponStopped(hit, surfaceData, dir);

                        if (!RemainAfterDeath)
                        {
                            StartCoroutine(Despawn());
                        }
                    }

                    break;
                }
                else
                {
                    if (penetratedColliders.Contains(hit.collider))
                        continue;

                    penetratedColliders.Add(hit.collider);

                    // The surface can be penetrated by this projectile.
                    // When the projectile travels through the surface, it's direction may change.
                    AddPathTracePoint(hit.point);
                    AddNewTracerVertex(hit.point);

                    dst -= Vector2.Distance(start, hit.point);
                    start = hit.point - hit.normal * 0.001f;

                    UponPenetrate(hit, surfaceData, dir);

                    currentPenetrationPower -= PenetrationPenalty;

                    // Skew the direction vector by an angle.
                    if (PenetrationSkew.y != 0f)
                    {
                        float current = dir.ToAngle();
                        float randomValue = random.GetValue();
                        bool positive = random.GetValue() <= 0.5f;
                        float min = PenetrationSkew.x / 2f;
                        float max = PenetrationSkew.y / 2f;
                        float rand = Mathf.Lerp(min, max, randomValue) * (positive ? 1f : -1f);
                        dir = (current + rand).ToDirection();
                    }
                    anyProcessed = true;

                    // Since direction may have changed, break out of this current ray's loop.
                    break;
                }
            }

            // Break outter loop.
            if (quit)
                break;

            if (!anyProcessed)
            {
                start = start + dir * dst;
                break;
            }

            itter++;
            if (itter == MAX_ITTERATIONS)
            {
                Debug.LogWarning($"Projectile step hit max number of itterations, {MAX_ITTERATIONS}, this may be a bug and may cause unintended behaviour.");
                break;
            }
        }

        Physics2D.queriesStartInColliders = oldQueries;

        return (start, dir);
    }

    private Vector2 CalculateReflection(Vector2 inDirection, Vector2 normal)
    {
        // Assumes that vectors are normalized. The normal MUST be normalized, and the input direction's magnitude will always equal that of the output (reflected) vector's magnitude, so
        // technically it doesn't have to be normalized.

        return inDirection - 2 * normal * (Vector2.Dot(inDirection, normal));
    }

    private void UpdateTracer()
    {
        if (Tracer == null)
            return;

        if (Tracer.positionCount == 0)
        {
            Tracer.positionCount = 2;
            Tracer.SetPosition(0, transform.position);
            Tracer.SetPosition(1, transform.position);
        }

        // Update 'first' vertex.
        SetFirstTracerVertex(transform.position);
        EnsureLineSize();
    }

    private void SetFirstTracerVertex(Vector3 pos)
    {
        Tracer.SetPosition(0, pos);
    }

    private void EnsureLineSize()
    {
        if (Tracer.positionCount < 2)
            return;

        // Makes sure that the entirety of the line is no more than MAX units long.
        float dst = 0f;
        Vector2 previousPoint = Tracer.GetPosition(0);
        for (int i = 1; i < Tracer.positionCount; i++)
        {
            // Grab the position, get square distance to the next vertex.
            Vector2 pos = Tracer.GetPosition(i);

            dst += Vector2.Distance(pos, previousPoint);
            previousPoint = pos;
        }

        // Is it more than the max length?
        if (dst >= TracerLength)
        {
            // Now we need to remove this excess from the line, starting from the last vertex moving towards the first vertex.
            float excess = dst - TracerLength;

            // Trim all the excess from the line.
            // Normally the excess is no more than one frame's worth of movement, but this is the only way to ensure very fast, bouncing projectiles can maintain a continuous line.
            RemoveFromLine(excess, -1);
        }

        dst = 0f;
        previousPoint = Tracer.GetPosition(0);
        for (int i = 1; i < Tracer.positionCount; i++)
        {
            // Grab the position, get square distance to the next vertex.
            Vector2 pos = Tracer.GetPosition(i);

            dst += Vector2.Distance(pos, previousPoint);
            previousPoint = pos;
        }
    }

    /// <summary>
    /// Removes the toRemove distance, in units, from the edge (index, index - 1) and returns the amount that could not be removed, if any.
    /// This method uses recursion to remove the inital toRemove value from the whole line, assuming that the inital index supplied had a value of (vertexCount - 1) or simply -1.
    /// </summary>
    /// <param name="toRemove">The target length to remove from the edge. The distance is removed from index towards (index - 1).</param>
    /// <param name="index">The index of the vertex to start removing from.</param>
    /// <returns>The remiaining length that could not be removed, if any.</returns>
    private float RemoveFromLine(float toRemove, int index)
    {
        if (index == 0)
            return 0f;
        if (index < 0)
            index = Tracer.positionCount - 1;

        Vector2 current = Tracer.GetPosition(index);
        Vector2 next = Tracer.GetPosition(index - 1);

        float dst = Vector2.Distance(current, next);
        if (toRemove > dst)
        {
            // Remove the last point (this one).
            Tracer.positionCount -= 1;
            return RemoveFromLine(toRemove - dst, index - 1);
        }
        else
        {
            // The distance between the current point and the next one is less that the target length to remove.
            // Therefore, the resulting new vertex position will line alone the line (current <-> next).
            // The return value should also logically at this point be zero.

            Vector2 diff = current - next;
            diff.Normalize();

            // Now that we have the direction, set it's magnitude to:
            // Distance - toRemove
            // to achieve the final position of the last vertex.

            diff *= dst - toRemove;
            Vector2 finalPos = next + diff;
            //Debug.Log("Removed {0}/{1} from point ({2} <-> {3}) on frame {4}".Form(Vector2.Distance(current, finalPos), toRemove, index, index - 1, Time.frameCount));
            Tracer.SetPosition(index, finalPos);
            return 0f;
        }
    }

    /// <summary>
    /// Adds the current position as a new vertex, placed immediately after the first vertex.
    /// </summary>
    private void AddNewTracerVertex(Vector2 pos)
    {
        if (Tracer == null)
            return;

        // Say we have two points:
        // The start and the current point, where 0 is current and 1 is start.
        // Now 0 must become 1 and 1 become 2.
        Tracer.positionCount++;
        for (int i = Tracer.positionCount - 1; i >= 1; i--)
        {
            Tracer.SetPosition(i, Tracer.GetPosition(i - 1));
        }
        Tracer.SetPosition(1, pos);
    }

    private void LogPath()
    {
        bool traceNow = false;
        if (traceTimer == PathTraceIdleInterval)
        {
            traceTimer = 0;
            traceNow = true;
        }
        else
        {
            traceTimer++;
        }

        if (traceNow)
        {
            AddPathTracePoint(transform.position);
        }
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForEndOfFrame();

        PoolObject.Despawn(this.PoolObject);
        yield return null;
    }

    private void AddPathTracePoint(Vector2 pos)
    {
        if (!PathTraceEnabled)
            return;

        if (pathTrace == null)
            pathTrace = new List<Vector2>();

        int c = pathTrace.Count;
        if (c != 0 && pathTrace[c - 1] == pos)
            return;

        pathTrace.Add(pos);
        while (pathTrace.Count > MAX_TRACE_POINTS)
        {
            pathTrace.RemoveAt(0);
        }
    }

    private void OnDrawGizmos()
    {
        Color a = Color.red;
        Color b = Color.yellow;

        if (PathTraceEnabled && pathTrace != null && pathTrace.Count >= 2)
        {
            for (int i = 0; i < pathTrace.Count - 1; i++)
            {
                var current = pathTrace[i];
                var next = pathTrace[i + 1];

                Gizmos.color = i % 2 == 0 ? a : b;
                Gizmos.DrawLine(current, next);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Direction.normalized));
    }

    private void UponPenetrate(RaycastHit2D hit, ICollisionSurface surfaceData, Vector2 dir)
    {
        // Called whenever the projectile penetrates a surface.
        SpawnHitEffect(surfaceData.GetHitEffect() ?? HitEffectPrefab, hit.point, hit.normal);

        DealDamage(hit, surfaceData, true, false, dir);
    }

    private void UponStopped(RaycastHit2D hit, ICollisionSurface surfaceData, Vector2 dir)
    {
        // Called whenever the projectile is stopped in it's tracks by an object that it can't penetrate.
        SpawnHitEffect(surfaceData.GetHitEffect() ?? HitEffectPrefab, hit.point, hit.normal);

        DealDamage(hit, surfaceData, false, false, dir);
    }

    private void UponReflected(RaycastHit2D hit, ICollisionSurface surfaceData, float chance, Vector2 dir)
    {
        // Called whenever the projectile is deflected off a surface that it could not penetrate.
        SpawnHitEffect(surfaceData.GetHitEffect() ?? HitEffectPrefab, hit.point, hit.normal);

        DealDamage(hit, surfaceData, false, true, dir);
    }

    private void SpawnHitEffect(PoolObject po, Vector2 pos, Vector2 dir)
    {
        if (po == null)
            return;

        var p = PoolObject.Spawn(po);
        p.transform.position = pos;
        p.transform.forward = dir;
    }

    private void DealDamage(RaycastHit2D hit, ICollisionSurface surface, bool penetrated, bool reflected, Vector2 dir)
    {
        // Of course no damage is dealt on the client.
        if (!JNet.IsServer)
            return;

        // Currently reflections deal no damage...
        if (reflected)
            return;

        // TODO UNDONE - Reduce damage based on current number of penetrations, damage falloff etc.
        Health h = surface.GetHealth();
        if (h != null)
        {
            h.ChangeHealth(hit.collider, -Damage);
        }
    }

    // GENERIC STUFF BELOW

    /*
     * Basic collision plan:
     * An object with a collider can be a few things:
     * - A solid (non-trigger) collider on a solid object (such as a wall)
     * - A solid (non-trigger) collider on a non-solid object (such as a player or enemy)
     * - A trigger collider on a destructible/killable object (window/enemy)
     * 
     * In all cases, the 'collision surface' should define a 'thickness' or 'toughness' that defines
     * how hard it is to penetrate the object (with bullets or arrows for example).
     */

    public static ICollisionSurface GetCollisionData(Collider2D collider)
    {
        if (collider == null)
            return null;

        var component = collider.GetComponentInParent<ICollisionSurface>();
        return component;
    }

    public static ICollisionSurface GetCollisionDataOrDefault(Collider2D collider)
    {
        var data = GetCollisionData(collider);
        if (data == null)
            return collider.isTrigger ? MonoCollisionSurface.DefaultTrigger : MonoCollisionSurface.DefaultSolid;
        else
            return data;
    }

    // SPAWNING & NETWORKING

    public static Projectile Spawn(Vector2 position, Vector2 direction, float speed)
    {
        if (!JNet.IsServer)
        {
            Debug.LogError("Cannot spawn projectile when not on server.");
            return null;
        }

        int seed = Random.Range(0, int.MaxValue);
        var spawned = SpawnLocal(position, direction, speed, seed);

        var msg = JNet.CreateCustomMessage(true, CustomMsg.PROJECTILE_SPAWN, 32);
        msg.Write(position);
        msg.Write(direction);
        msg.Write(speed);
        msg.Write(spawned.Seed);
        JNet.SendCustomMessageToAll(JNet.GetServer().LocalClientConnection, msg, Lidgren.Network.NetDeliveryMethod.ReliableUnordered, 0);

        return spawned;
    }

    private static Projectile SpawnLocal(Vector2 position, Vector2 direction, float speed, int seed)
    {
        var spawned = PoolObject.Spawn(Spawnables.Get<Projectile>("Projectile"));

        spawned.transform.position = position;
        spawned.Direction = direction;
        spawned.Speed = speed;
        spawned.Seed = seed;

        return spawned;
    }

    public static void ProcessMessage(NetIncomingMessage msg)
    {
        Vector2 pos = msg.ReadVector2();
        Vector2 dir = msg.ReadVector2();
        float speed = msg.ReadFloat();
        int seed = msg.ReadInt32();

        SpawnLocal(pos, dir, speed, seed);
    }
}
