
using JNetworking;
using ProjectB.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectB
{
    public class Dock : NetBehaviour
    {
        public static readonly List<Dock> AllDocks = new List<Dock>();

        public string Name = "Rocky Port";
        public Vector2 Entrance;
        public Vector2 EntranceDirection = new Vector2(-1f, 0f);
        public int SlotCount { get { return Slots?.Length ?? 0; } }
        public Slot[] Slots;

        private void Awake()
        {
            AllDocks.Add(this);
        }

        private void OnDestroy()
        {
            AllDocks.Remove(this);
        }

        public Vector2 GetEntrancePos()
        {
            return transform.TransformPoint(Entrance);
        }

        public Vector2 GetEntranceDirection()
        {
            return transform.TransformVector(EntranceDirection).normalized;
        }

        public Vector2 GetSlotPos(int index)
        {
            if(index >= 0 && index < Slots.Length)
            {
                return this.GetSlotPos(Slots[index]);
            }
            else
            {
                Debug.LogError($"Index {index} is out of bounds for dock slot, there are {Slots.Length} slots.");
                return Vector2.zero;
            }
        }

        public Vector2 GetSlotPos(Slot spot)
        {
            return transform.TransformPoint(spot.Offset);
        }

        public Vector2 GetSlotDirection(int index)
        {
            if (index >= 0 && index < Slots.Length)
            {
                return this.GetSlotDirection(Slots[index]);
            }
            else
            {
                Debug.LogError($"Index {index} is out of bounds for dock slot, there are {Slots.Length} slots.");
                return Vector2.zero;
            }
        }

        public Vector2 GetSlotDirection(Slot spot)
        {
            return transform.TransformDirection(spot.InwardsDirection).normalized;
        }

        /// <summary>
        /// Gets the first available slot the can fit the ship of that size.
        /// Priotitizes placing it in the smallest possible slot, although small ships can dock in
        /// slots intended for larger ships.
        /// </summary>
        /// <param name="size">The size of the ship to fit in.</param>
        /// <returns>The index of the slot, or -1 if none were available.</returns>
        public int GetOpenSlot(ShipSize size)
        {
            int bestFit = -1;
            ShipSize bestFitSize = ShipSize.Large;
            for (int i = 0; i < Slots.Length; i++)
            {
                var spot = Slots[i];

                if (spot.CurrentShip != null)
                    continue;

                if (spot.Size.IsLargerOrEqual(size))
                {
                    if (spot.Size.IsSmallerOrEqual(bestFitSize))
                    {
                        bestFit = i;
                        bestFitSize = spot.Size;
                    }
                }
            }

            return bestFit;
        }

        /// <summary>
        /// Assigns a ship to a spot. The ship can be null to indicate that the ship has left that spot.
        /// The ship (if not null) must fit into the spot (see <see cref="Slot.Size"/>).
        /// Note that this is not the correct method to call to dock a ship. For that, see <see cref="Ship.TryEnterDock"/>.
        /// </summary>
        /// <param name="index">The index of the spot. Should be 0 < index < <see cref="SlotCount"/>.</param>
        /// <param name="ship">The ship to place in that spot. Can be null to remove any existing ship.</param>
        /// <returns>True if the operation succeeded, false if it didn't.</returns>
        public bool AssignShip(int index, Ship ship)
        {
            if(index < 0 || index >= Slots.Length)
            {
                Debug.LogError($"Index {index} is out of bounds for this ports spots. Max index: {Slots.Length - 1}.");
                return false;
            }

            var spot = Slots[index];

            if(ship != null)
            {
                if (!spot.Size.IsLargerOrEqual(ship.Size))
                {
                    Debug.LogError($"Cannot assign ship {ship.Vehicle.Name} to dock spot {index}: expected size {spot.Size} or smaller, got {ship.Size}!");
                    return false;
                }
            }

            spot.CurrentShip = ship;
            Slots[index] = spot;
            return true;
        }

        private void OnDrawGizmosSelected()
        {
            Vector2 entrance = GetEntrancePos();
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(entrance, 0.5f);
            Gizmos.DrawLine(entrance, entrance + GetEntranceDirection() * 0.2f);

            foreach (var spot in Slots)
            {
                float radius = 0.1f;
                Color c = Color.white;
                switch (spot.Size)
                {
                    case ShipSize.Tiny:
                        radius = 1f;
                        c = Color.green;
                        break;
                    case ShipSize.Small:
                        radius = 2.5f;
                        c = Color.cyan;
                        break;
                    case ShipSize.Medium:
                        c = Color.yellow;
                        radius = 5f;
                        break;
                    case ShipSize.Large:
                        radius = 6f;
                        c = Color.red;
                        break;
                }
                radius *= 0.5f;

                Vector2 pos = GetSlotPos(spot);
                Vector2 dir = GetSlotDirection(spot);
                Gizmos.color = c;
                Gizmos.DrawWireSphere(pos, radius);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(pos, pos + dir * radius * 1.1f);
            }
        }

        [System.Serializable]
        public struct Slot
        {
            public Vector2 Offset;
            public Vector2 InwardsDirection;
            public ShipSize Size;
            public Ship CurrentShip;

            public Slot(Vector2 offset, Vector2 inwards, ShipSize size)
            {
                this.Offset = offset;
                this.InwardsDirection = inwards;
                this.Size = size;
                this.CurrentShip = null;
            }
        }

        /// <summary>
        /// Gets a dock given it's name. Not case sensitive. In the case of multiple docks with the
        /// same name, only the first dock will be returned.
        /// </summary>
        /// <param name="name">The full name of the dock. Capitalization is ignored.</param>
        /// <returns>The found Dock instance, or null if it was not found.</returns>
        public static Dock Get(string name)
        {
            name = name.Trim().ToLower();

            foreach (var dock in AllDocks)
            {
                if(dock.Name.Trim().ToLower() == name)
                {
                    return dock;
                }
            }

            return null;
        }
    }
}
