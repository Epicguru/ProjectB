using ProjectB.Commands;
using UnityEngine;

namespace ProjectB.Vehicles
{
    [RequireComponent(typeof(Vehicle))]
    public class Ship : MonoBehaviour
    {
        public Vehicle Vehicle
        {
            get
            {
                if (_vehicle == null)
                    _vehicle = GetComponent<Vehicle>();
                return _vehicle;
            }
        }
        private Vehicle _vehicle;
        public string Name { get { return Vehicle.Name; } }

        [Header("Info")]
        public ShipSize Size = ShipSize.Small;

        [Header("Dock")]
        public bool IsInDock = false;
        public int DockSlot = -1;
        public Dock CurrentDock = null;

        private void Update()
        {
            if(IsInDock == Vehicle.Movement.enabled)
            {
                Vehicle.Movement.enabled = !IsInDock;
            }

            if (IsInDock)
            {
                Vehicle.Body.velocity = Vector2.zero;
                Vehicle.Body.angularVelocity = 0f;
                transform.position = CurrentDock.GetSlotPos(DockSlot);
                transform.eulerAngles = new Vector3(0f, 0f, CurrentDock.GetSlotDirection(DockSlot).ToAngle());
            }
        }

        public bool CanEnterDock(Dock dock)
        {
            int spot = dock.GetOpenSlot(this.Size);
            return spot != -1;
        }

        public void GoToDock(Dock dock)
        {
            if (!CanEnterDock(dock))
            {
                Debug.LogWarning($"Ship {this.Name} cannot enter dock {dock.Name}! Will not go there.");
                return;
            }

            Vehicle.Nav.MakePathToPos(dock.GetEntrancePos(), dock.GetEntranceDirection().ToAngle(), (veh, worked) =>
            {
                Debug.Log("Path finished. Arrived at destination: " + worked);
                if (worked)
                {
                    TryEnterDock(dock);
                }
            });
        }

        /// <summary>
        /// Should only be called when positioned at the <see cref="Dock.Entrance"/> world position.
        /// Causes this ship to enter that dock, if possible. See <see cref="CanEnterDock(Dock)"/> to check if
        /// a dock can accept this ship.
        /// </summary>
        private void TryEnterDock(Dock dock)
        {
            if(dock == null)
            {
                Debug.LogError("Dock is null!");
                return;
            }

            if (IsInDock)
            {
                Debug.LogError($"Already in dock! Ship {this.Name} tried to enter {dock.Name}, but is already in {this.CurrentDock.Name} in slot {DockSlot}.");
                return;
            }

            if (!CanEnterDock(dock))
            {
                Debug.LogError($"Cannot enter dock {dock.Name}, no slot available for this [{Size}] {Name}. Remeber to check CanEnterDock()");
                return;
            }

            int slotIndex = dock.GetOpenSlot(this.Size);
            bool worked = dock.AssignShip(slotIndex, this);

            if (!worked)
            {
                Debug.LogError("Unexpected error docking ship. Exiting method.");
                return;
            }

            // Set all the of 'current dock' variables.
            this.IsInDock = true;
            this.DockSlot = slotIndex;
            this.CurrentDock = dock;

            // Move to the dock entrance (should already be close or exactly here).
            Vehicle.Body.velocity = Vector2.zero;
            Vehicle.Body.angularVelocity = 0f;
            transform.position = dock.GetEntrancePos();
            transform.right = dock.GetEntranceDirection();

            // URGTODO left off here
            // place the ship in the correct slot and keep it there by disabling Movement class.
        }

        /// <summary>
        /// Leaves the current dock.
        /// </summary>
        public void LeaveDock()
        {
            if (!IsInDock)
            {
                Debug.LogError("Not in dock, cannot leave it!");
                return;
            }
            if(CurrentDock == null)
            {
                Debug.LogError("Big error :D. Ship is flagged as in dock, but dock is null. Was it destroyed?");
                return;
            }

            // Remove current ship from slot.
            CurrentDock.AssignShip(DockSlot, null);

            IsInDock = false;
            DockSlot = -1;

            // Move to dock 'entrance' but facing in the 'leaving' direction.
            Vehicle.Body.velocity = Vector2.zero;
            Vehicle.Body.angularVelocity = 0f;
            transform.position = CurrentDock.GetEntrancePos();
            transform.right = -CurrentDock.GetEntranceDirection();

            CurrentDock = null;
        }

        [Command("Tells all selected units to move to a single named port.")]
        private static string Ship_To_Dock(string dockName)
        {
            Dock dock = Dock.Get(dockName);
            if(dock == null)
            {
                return "Couldn't find a dock for that name";
            }

            foreach (var veh in Vehicle.AllVehicles)
            {
                if (!veh.Unit.IsSelected)
                    continue;

                if(veh.IsShip)
                {
                    veh.Ship.GoToDock(dock);
                }
            }

            return "Done.";
        }

        [Command("Tells all selected units to leave their current port.")]
        private static string Leave_Dock()
        {
            int left = 0;
            foreach (var veh in Vehicle.AllVehicles)
            {
                if (!veh.Unit.IsSelected)
                    continue;

                if (veh.IsShip && veh.Ship.IsInDock)
                {
                    veh.Ship.LeaveDock();
                    left++;
                }
            }

            return $"Done. {left} ships left their dock.";
        }
    }
}

