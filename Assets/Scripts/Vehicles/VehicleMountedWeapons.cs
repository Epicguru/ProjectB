
using JNetworking;
using UnityEngine;

public class VehicleMountedWeapons : MonoBehaviour
{
    public MountedWeapon Weapon;
    public MountedWeaponSpot[] Spots;

    private void Awake()
    {
        Spots = GetComponentsInChildren<MountedWeaponSpot>();
        for (int i = 0; i < Spots.Length; i++)
        {
            var spot = Spots[i];
            spot.SetNodeID((byte)i);
        }

        if (JNet.IsServer)
        {
            var spawned = Instantiate(Weapon);
            JNet.Spawn(spawned.gameObject);

            Spots[0].SetMountedWeapon(spawned);
        }
    }
}
