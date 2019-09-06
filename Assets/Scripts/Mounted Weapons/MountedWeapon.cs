
using JNetworking;
using UnityEngine;

[RequireComponent(typeof(NetPosSync))]
public class MountedWeapon : NetBehaviour
{
    [SyncVar] public Vector2 TargetPos;
    [SyncVar] public bool Fire;

    public enum ProjectileType
    {
        HITSCAN,
        PROJECTILE
    }

    [Header("Shooting")]
    public ProjectileType Type = ProjectileType.PROJECTILE;
    public bool AnimationDriven = false;
    public Transform[] MuzzleSpots;
    public float ProjectileSpeed = 70f;
    public float RPM = 120f;

    [Header("Turning")]
    public bool TurnToTarget = false;
    [SyncVar] public bool AutoFire = false;
    public float MaxTurnSpeed = 180f;
    public float AutoFireMinAngle = 5f;

    [Header("Spread")]
    public Vector2 AngleOffset = new Vector2(0f, 5f);

    [Header("Hitscan")]
    public float HitscanRange = 10f;

    [Header("Effects")]
    public MuzzleFlash MuzzleFlashPrefab;

    private float timer;

    private void Awake()
    {
        var sync = GetComponent<NetPosSync>();
        sync.SendRate = 5f;
        sync.SyncRotation = true;
    }

    private void Update()
    {
        bool targetInSights = false;
        if (TurnToTarget)
        {
            float currentAngle = transform.eulerAngles.z;
            float targetAngle = ((Vector2)transform.position).AngleTowards(TargetPos);
            float delta = Mathf.DeltaAngle(currentAngle, targetAngle);

            float turnScale = delta == 0f ? 0f : delta > 0f ? 1f : -1f;
            float toTurn = MaxTurnSpeed * turnScale * Time.deltaTime;
            float finalAngle = currentAngle;

            float final = currentAngle + toTurn;
            if(turnScale == 1f && final > targetAngle)
            {
                finalAngle = targetAngle;
            }
            else if(turnScale == -1f && final < targetAngle)
            {
                finalAngle = targetAngle;
            }
            else
            {
                finalAngle = final;
            }

            transform.localEulerAngles = new Vector3(0f, 0f, finalAngle);

            delta = Mathf.Abs(Mathf.DeltaAngle(finalAngle, targetAngle));
            targetInSights = delta <= AutoFireMinAngle;
        }

        if (AutoFire && targetInSights)
            Fire = true;

        if (AnimationDriven)
            return;

        float interval = 1f / (RPM / 60f);
        timer += Time.deltaTime;
        if(timer >= interval)
        {
            timer -= interval;

            if (Fire)
            {
                Shoot(0);
            }
        }
    }

    private void UponAnimationEvent(AnimationEvent e)
    {
        string s = e.stringParameter.Trim().ToLowerInvariant();

        switch (s)
        {
            case "shoot":

                Shoot(e.intParameter);                

                break;
        }
    }

    public void Shoot(int muzzle)
    {
        Transform pos = MuzzleSpots[muzzle];

        if(MuzzleFlashPrefab != null)
        {
            var spawned = PoolObject.Spawn(MuzzleFlashPrefab);
            spawned.transform.position = pos.position;
            spawned.transform.rotation = pos.rotation;
        }

        if (JNet.IsServer)
        {
            Vector2 direction = pos.right;
            float angle = direction.ToAngle();
            bool right = Random.value <= 0.5f;
            angle += Random.Range(AngleOffset.x, AngleOffset.y) * 0.5f * (right ? 1f : -1f);
            direction = angle.ToDirection();

            switch (Type)
            {
                case ProjectileType.HITSCAN:
                    HitScan.Shoot(pos.position, direction, HitscanRange, 10f); // TODO allow custom damage here and in spawned projectiles.
                    break;
                case ProjectileType.PROJECTILE:
                    Projectile.Spawn(pos.position, direction, ProjectileSpeed);
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, TargetPos);
    }
}
