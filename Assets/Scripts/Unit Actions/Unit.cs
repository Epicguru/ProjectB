
using UnityEngine;

namespace ProjectB.Units.Actions
{
    public partial class Unit : MonoBehaviour
    {
        public bool IsVehicle { get { return Vehicle != null; } }
        public Vehicle Vehicle;

        public Vector2 ForwardVector { get { return transform.right; } }
    }
}
