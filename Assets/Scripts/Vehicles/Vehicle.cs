using JNetworking;
using ProjectB.Commands;
using ProjectB.Units;
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

        public Collider2D SelectionCollider;
        public string Name { get { return Unit.Name; } }

        private void Awake()
        {
            NetPosSync.SyncRotation = true;
            AllVehicles.Add(this);

            if (SelectionCollider == null)
            {

                SelectionCollider = GetComponentInChildren<Collider2D>();
                if (SelectionCollider == null)
                {
                    Debug.LogError($"Failed to get selection collider for vehicle {Name}. Add one!");
                }
            }
        }

        /// <summary>
        /// Returns the closest point on the selection collider's perimeter of this vehicle to the supplied point.
        /// </summary>
        /// <param name="exteriorPos">The exterior point to get the cloest point to.</param>
        /// <returns>The closest point on the collider perimeter.</returns>
        public Vector2 GetClosestSelectionPoint(Vector2 exteriorPos)
        {
            return SelectionCollider?.ClosestPoint(exteriorPos) ?? exteriorPos;
        }

        /// <summary>
        /// Returns the square of the distance from the supplied exterior point to the closest point on the surface of this vehicle's selection collider.
        /// </summary>
        /// <param name="exteriorPos">The exterior point to get the cloest point to.</param>
        /// <returns>The square of the distance, in Unity units.</returns>
        public float GetClosestSquareDistance(Vector2 exteriorPos)
        {
            Vector2 closestPoint = GetClosestSelectionPoint(exteriorPos);

            return (closestPoint - exteriorPos).sqrMagnitude;
        }

        /// <summary>
        /// Returns the the distance from the supplied exterior point to the closest point on the surface of this vehicle's selection collider.
        /// </summary>
        /// <param name="exteriorPos">The exterior point to get the cloest point to.</param>
        /// <returns>The distance, in Unity units. Remember, 1 unit is roughly 10 meters.</returns>
        public float GetClosestDistance(Vector2 exteriorPos)
        {
            return Mathf.Sqrt(GetClosestSquareDistance(exteriorPos));
        }

        private void UpdateUnitBounds(Unit unit)
        {
            Vector2 min = SelectionCollider.bounds.min;
            Vector2 max = SelectionCollider.bounds.max;

            Vector2 size = max - min;

            unit.Bounds = new Rect(min, size);
        }

        private void GL_DrawUnitSelected(Unit unit)
        {
            // URGTODO
            // BOOKMARK
            // TOOD left off here.

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
}

