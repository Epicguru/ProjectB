
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
            spot.Vehicle = this;
            spot.SetNodeID((byte)i);
        }

        if (JNet.IsServer)
        {
            var spawned = Instantiate(Weapon);
            JNet.Spawn(spawned.gameObject);

            Spots[0].SetMountedWeapon(spawned);
        }
    }

    public MountedWeaponSpot GetMountSpot(int index)
    {
        if(index >= 0 && index < Spots.Length)
        {
            return Spots[index];
        }
        return null;
    }

    public void SetWeapon(int spotIndex, string mountedWeaponName)
    {
        if (spotIndex >= 0 && spotIndex < Spots.Length)
        {
            SetWeapon(Spots[spotIndex], mountedWeaponName);
        }
        else
        {
            Debug.LogError($"Spot index {spotIndex} is out of bounds! Min {0}, max {Spots.Length}.");
            return;
        }
    }

    /// <summary>
    /// Sets the mounted weapon to the mounted weapon with the specified prefab name. Only works when called from the server.
    /// If the weapon is null, the current weapon is removed instantly.
    /// </summary>
    /// <param name="spot">The spot. Should be on this vehicle, and not null!</param>
    /// <param name="mountedWeaponName">The name of the prefab.</param>
    public void SetWeapon(MountedWeaponSpot spot, string mountedWeaponName)
    {
        if (string.IsNullOrWhiteSpace(mountedWeaponName))
        {
            Debug.LogError("Null or whitespace name. Cannot set mounted weapon!");
            return;
        }

        var spawnable = Spawnables.Get<MountedWeapon>(mountedWeaponName);
        if(spawnable != null)
        {
            var spawned = Instantiate(spawnable);
            SetWeapon(spot, spawned);
            JNet.Spawn(spawned.gameObject);
        }
        else
        {
            Debug.LogError($"Failed to spawn mounted weapoon from name {mountedWeaponName}.");
        }
    }

    /// <summary>
    /// Sets the mounted weapon to the mounted weapon with the specified prefab name. Only works when called from the server.
    /// If the weapon is null, the current weapon is removed instantly.
    /// </summary>
    /// <param name="spotIndex">The mount index.</param>
    /// <param name="weapon">The weapon instance to place on the mount. May be null to remove the current weapon..</param>
    public void SetWeapon(int spotIndex, MountedWeapon weapon)
    {
        if(spotIndex >= 0 && spotIndex < Spots.Length)
        {
            SetWeapon(Spots[spotIndex], weapon);
        }
        else
        {
            Debug.LogError($"Spot index {spotIndex} is out of bounds! Min {0}, max {Spots.Length}.");
            return;
        }
    }

    /// <summary>
    /// Sets the mounted weapon to the specified instance. Only works when called from the server.
    /// If the weapon is null, the current weapon is removed instantly.
    /// </summary>
    /// <param name="spot">The spot. Should be on this vehicle, and not null!</param>
    /// <param name="weapon">The weapon instance, not prefab!</param>
    public void SetWeapon(MountedWeaponSpot spot, MountedWeapon weapon)
    {
        if (!JNet.IsServer)
        {
            Debug.LogError("Cannot set mounted weapon when not on server.");
            return;
        }
        if(spot == null)
        {
            Debug.LogError("Spot is null.");
            return;
        }  
        if(spot.Vehicle != this)
        {
            Debug.LogError($"This spot is not on the current vehicle ({this.gameObject.name}), cannot set mounted weapon.");
            return;
        }
        if(spot.Size != weapon.Size)
        {
            Debug.LogError($"Size is not correct. Expected {spot.Size}, got {weapon.Size}... Weapon will not be placed.");
            return;
        }

        spot.SetMountedWeapon(weapon);
    }
}
