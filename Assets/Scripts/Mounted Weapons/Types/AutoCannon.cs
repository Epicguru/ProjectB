
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

    private void Update()
    {
        if (JNet.IsServer)
        {
            Anim.SetBool("Shoot", MountedWeapon.Fire);
        }
    }    
}
