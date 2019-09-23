using JNetworking;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VehicleMovement))]
[RequireComponent(typeof(VehicleNavigation))]
[RequireComponent(typeof(VehicleMountedWeapons))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NetPosSync))]
public class Vehicle : MonoBehaviour
{
    public static List<Vehicle> AllVehicles = new List<Vehicle>();

    public VehicleMovement Movement
    {
        get
        {
            if (_movement == null)
                _movement = GetComponent<VehicleMovement>();
            return _movement;
        }
    }
    private VehicleMovement _movement;
    public Health Health
    {
        get
        {
            if (_health == null)
                _health = GetComponent<Health>();
            return _health;
        }
    }
    private Health _health;
    public VehicleNavigation Nav
    {
        get
        {
            if (_nav == null)
                _nav = GetComponent<VehicleNavigation>();
            return _nav;
        }
    }
    private VehicleNavigation _nav;
    public VehicleMountedWeapons MountedWeapons
    {
        get
        {
            if (_weapons == null)
                _weapons = GetComponent<VehicleMountedWeapons>();
            return _weapons;
        }
    }
    private VehicleMountedWeapons _weapons;
    public NetPosSync NetPosSync
    {
        get
        {
            if (_posSync == null)
                _posSync = GetComponent<NetPosSync>();
            return _posSync;
        }
    }
    private NetPosSync _posSync;
    public Rigidbody2D Body
    {
        get
        {
            return Movement.Body;
        }
    }

    public string Name = "My Vehicle";

    private void Awake()
    {
        NetPosSync.SyncRotation = true;
        AllVehicles.Add(this);
    }

    private void OnDestroy()
    {
        if (AllVehicles.Contains(this))
        {
            AllVehicles.Remove(this);
        }
    }

    [Command("Mounts a weapon on to a named ship.")]
    public static string MountWeapon(string vehicleName, int slot, string weaponName, bool forAll)
    {
        if (string.IsNullOrWhiteSpace(weaponName))
            weaponName = null;

        int count = 0;
        foreach (var veh in AllVehicles)
        {
            if (veh.gameObject.name.ToLower().Contains(vehicleName.ToLower()))
            {
                veh.MountedWeapons.SetWeapon(slot, weaponName);

                if (!forAll)
                {
                    return $"Placed {weaponName} in spot {veh.MountedWeapons.GetMountSpot(slot).Name} on vehicle {veh.Name} ({veh.gameObject.name}).";
                }
                else
                {
                    count++;
                }
            }
        }

        return $"Placed weapon on {count} vehicles.";
    }
}
