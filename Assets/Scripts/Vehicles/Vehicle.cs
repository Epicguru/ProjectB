using JNetworking;
using UnityEngine;

[RequireComponent(typeof(VehicleMovement))]
[RequireComponent(typeof(VehicleNavigation))]
[RequireComponent(typeof(VehicleMountedWeapons))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NetPosSync))]
public class Vehicle : MonoBehaviour
{
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

    public string Name = "My Vehicle";

    private void Awake()
    {
        NetPosSync.SyncRotation = true;
    }
}
