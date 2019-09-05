
using JNetworking;
using UnityEngine;

[RequireComponent(typeof(AutoCannon))]
public class AutoCannon : MonoBehaviour
{
    public MountedWeapon MountedWeapon
    {
        get
        {
            if (_weapon == null)
                _weapon = GetComponent<MountedWeapon>();
            return _weapon;
        }
    }
    private MountedWeapon _weapon;

    public NetAnimator Anim;
    public Transform TopFire, BottomFire;
    public float ProjectileSpeed = 300f;

    private void Update()
    {
        if (JNet.IsServer)
        {
            Anim.SetBool("Shoot", MountedWeapon.Fire);
        }
    }

    private void UponAnimationEvent(AnimationEvent e)
    {
        string s = e.stringParameter.Trim().ToLowerInvariant();

        switch (s)
        {
            case "shoot":

                if (JNet.IsServer)
                {
                    Debug.Log(e.intParameter);
                    Transform pos = e.intParameter == 0 ? BottomFire : TopFire;
                    Projectile.Spawn(pos.position, pos.right, ProjectileSpeed);
                }

                break;
        }
    }
}
