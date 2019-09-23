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
    }
}

