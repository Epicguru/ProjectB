
using UnityEngine;

namespace ProjectB.Vehicles
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class VehicleMovement : MonoBehaviour
    {
        public Rigidbody2D Body
        {
            get
            {
                if (_body == null)
                    _body = GetComponent<Rigidbody2D>();
                return _body;
            }
        }
        private Rigidbody2D _body;

        public float ForwardsThrust = 0f;
        public float Torque = 0f;

        private void Awake()
        {
            Body.gravityScale = 0f;
        }

        private void FixedUpdate()
        {
            Body.AddForce(transform.right * ForwardsThrust);
            Body.AddTorque(Torque);
        }
    }
}

