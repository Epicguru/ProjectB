
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

    [Header("Mounting")]
    public MountedWeaponSize Size = MountedWeaponSize.SMALL;

    [Header("Shooting")]
    public ProjectileType Type = ProjectileType.PROJECTILE;
    public bool AnimationDriven = false;
    public Transform[] MuzzleSpots;
    public float ProjectileSpeed = 70f;
    public float RPM = 120f;

    [Header("Turning")]
    public bool TurnToTarget = true;
    public float MaxTurnSpeed = 180f;
    public float InSightsMinAngle = 5f;
    public float IdleAngle = 0f;

    [Header("Spread")]
    public Vector2 AngleOffset = new Vector2(0f, 5f);

    [Header("Hitscan")]
    public float HitscanRange = 10f;

    [Header("Effects")]
    public MuzzleFlash MuzzleFlashPrefab;

    public bool TargetInSights { get; private set; }

    public NetAnimator Anim
    {
        get
        {
            if (_anim == null)
                _anim = GetComponentInChildren<NetAnimator>();
            return _anim;
        }
    }
    private NetAnimator _anim;

    private float timer;

    private void Awake()
    {
        var sync = GetComponent<NetPosSync>();
        sync.SendRate = 5f;
        sync.SyncRotation = true;

        if(MuzzleSpots.Length == 0)
        {
            Debug.LogError($"Missing muzzle for mounted weapon {name}.");
        }

        if(AnimationDriven && GetComponentInChildren<NetAnimator>() == null)
        {
            Debug.LogError($"This mounted weapon {name} is animation driven, but no net animator was found. Add it!");
        }
    }

    private void Update()
    {
        float currentAngle = transform.eulerAngles.z;
        float realTargetAngle = ((Vector2)transform.position).AngleTowards(TargetPos);
        float targetAngle = TurnToTarget ? realTargetAngle : IdleAngle;
        float delta = Mathf.DeltaAngle(currentAngle, targetAngle);

        float turnScale = delta == 0f ? 0f : delta > 0f ? 1f : -1f;
        float toTurn = MaxTurnSpeed * turnScale * Time.deltaTime;
        float finalAngle;

        if(Mathf.Abs(toTurn) > Mathf.Abs(delta))
        {
            finalAngle = targetAngle;
        }
        else
        {
            finalAngle = currentAngle + toTurn;
        }

        delta = Mathf.Abs(Mathf.DeltaAngle(finalAngle, realTargetAngle));
        TargetInSights = delta <= InSightsMinAngle;

        transform.eulerAngles = new Vector3(0f, 0f, finalAngle);

        if (AnimationDriven)
        {
            if (JNet.IsServer && Anim != null)
            {
                Anim.SetBool("Shoot", Fire);
            }
            return;
        }

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
                if (JNet.IsServer)
                {
                    Projectile.Spawn(pos.position, direction, ProjectileSpeed);
                }
                break;
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, TargetPos);
    }
}
