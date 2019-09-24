
using ProjectB.Vehicles;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectB.Units
{
    public partial class Unit : MonoBehaviour
    {
        public static readonly List<Unit> AllActiveUnits = new List<Unit>();
        
        /// <summary>
        /// Gets all active units that are currently selected.
        /// Quite expensive since it itterates through all units, so use sparingly.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Unit> GetAllSelected()
        {
            foreach (var unit in AllActiveUnits)
            {
                if(unit != null && unit.IsSelected)
                    yield return unit;
            }
        }

        public static void DeselectAll()
        {
            foreach (var unit in AllActiveUnits)
            {
                unit.IsSelected = false;
            }
        }

        public static void GL_DrawAllSelected()
        {
            foreach (var unit in AllActiveUnits)
            {
                if (unit.IsSelected)
                {
                    unit.GL_DrawSelected();
                }
            }
        }

        [Header("Basic")]
        public string Name = "Unit Name";

        [Header("Selection")]
        public BoxCollider2D SelectionCollider;
        public bool IsSelected;

        public bool IsVehicle { get { return Vehicle != null; } }

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

        private void Awake()
        {
            AllActiveUnits.Add(this);

            if (SelectionCollider == null)
            {

                SelectionCollider = GetComponentInChildren<BoxCollider2D>();
                if (SelectionCollider == null)
                {
                    Debug.LogError($"Failed to get selection collider for vehicle {Name}. Add one!");
                }
            }
            if(SelectionCollider != null)
                SelectionCollider.gameObject.layer = 10;
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

        private void OnDestroy()
        {
            AllActiveUnits.Remove(this);
        }

        public virtual void GL_DrawSelected()
        {
            SendMessage("GL_DrawUnitSelected", Color.red, SendMessageOptions.DontRequireReceiver);
        }
    }
}

