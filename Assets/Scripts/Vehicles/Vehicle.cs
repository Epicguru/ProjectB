using JNetworking;
using ProjectB.Commands;
using ProjectB.Units;
using ProjectB.Units.Actions;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectB.Vehicles
{
    [RequireComponent(typeof(VehicleMovement))]
    [RequireComponent(typeof(VehicleNavigation))]
    [RequireComponent(typeof(VehicleMountedWeapons))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(NetPosSync))]
    [RequireComponent(typeof(Unit))]
    public class Vehicle : MonoBehaviour
    {
        public static List<Vehicle> AllVehicles = new List<Vehicle>();

        public Unit Unit
        {
            get
            {
                if (_unit == null)
                    _unit = GetComponent<Unit>();

                return _unit;
            }
        }
        private Unit _unit;
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

        public string Name { get { return Unit.Name; } }

        private void Awake()
        {
            NetPosSync.SyncRotation = true;
            AllVehicles.Add(this);

            Unit.AddAction<ExplodeAction>();
        }       

        private void GL_DrawUnitSelected(Color color)
        {
            Vector2[] points = Unit.SelectionCollider.GetWorldCorners();

            GameCamera.DrawLine(points[0], points[1], color);
            GameCamera.DrawLine(points[1], points[2], color);
            GameCamera.DrawLine(points[2], points[3], color);
            GameCamera.DrawLine(points[3], points[0], color);
        }

        private void OnDestroy()
        {
            if (AllVehicles.Contains(this))
            {
                AllVehicles.Remove(this);
            }
        }

        [Command("Mounts a weapon on the selected ship(s).")]
        public static string MountWeapon(int slot, string weaponName, bool forAll)
        {
            if (string.IsNullOrWhiteSpace(weaponName))
                weaponName = null;

            int count = 0;

            var selected = Unit.GetAllSelected();
            foreach (var unit in selected)
            {
                if (unit.IsVehicle)
                {
                    var veh = unit.Vehicle;
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
}

