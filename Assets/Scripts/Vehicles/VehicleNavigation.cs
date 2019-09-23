
using System.Collections.Generic;
using ThreadedPathfinding;
using ThreadedPathfinding.Internal;
using UnityEngine;

namespace ProjectB.Vehicles
{
    public class VehicleNavigation : MonoBehaviour
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

        [System.NonSerialized]
        public List<PNode> Targets = new List<PNode>();

        public float PointReachedDistance = 2f;

        [Header("Thrust")]
        public float ThrustForce = 1000f;
        public float ThrustDecreaseDistance = 10f;

        [Header("Torque")]
        public float TorqueForce = 100f;
        public float TorqueAngleMax = 90f;

        private bool reachedEnd = true;
        private Vector2 finalTarget;
        private float finalAngle;

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) && Vehicle.Unit.IsSelected && !InputManager.IsMouseInUI)
            {
                finalTarget = InputManager.MousePos;
                var start = Navigation.GetClosestTile(transform.position);
                var end = Navigation.GetClosestTile(finalTarget);
                var result = new Pathfinding().Run(start.x, start.y, end.x, end.y, Navigation.Instance, Targets);
                if (result != PathfindingResult.SUCCESSFUL)
                {
                    Debug.LogWarning($"Pathfinding result: {result}");
                    reachedEnd = true;
                }
                else
                {
                    reachedEnd = false;
                }
            }

            if (Targets == null || (Targets.Count == 0 && reachedEnd))
            {
                float deltaAngle = Mathf.DeltaAngle(transform.localEulerAngles.z, finalAngle);
                float scale = Mathf.Abs(deltaAngle) / TorqueAngleMax;
                float torque = TorqueForce * scale * (deltaAngle > 0f ? 1f : -1f) * 1f;

                Movement.ForwardsThrust = 0f;
                Movement.Torque = torque;
            }
            else
            {
                Vector2 point = Targets.Count != 0 ? Navigation.GetWorldPos(Targets[0]) : finalTarget;
                if (point.DistanceCheck(transform.position, Targets.Count != 0 ? PointReachedDistance : 0.2f))
                {
                    if (Targets.Count > 1)
                    {
                        point = Navigation.GetWorldPos(Targets[1]);
                        Targets.RemoveAt(0);
                    }
                    else
                    {
                        if (Targets.Count == 0)
                        {
                            reachedEnd = true;
                        }
                        else
                        {
                            Targets.Clear();
                        }
                    }
                }
                Vector2 currentPos = transform.position;
                float currentAngle = transform.localEulerAngles.z;
                float dst = currentPos.DistanceTo(point);

                float angle = currentPos.AngleTowards(point);
                float deltaAngle = Mathf.DeltaAngle(currentAngle, angle);
                float scale = Mathf.Abs(deltaAngle) / TorqueAngleMax;

                float torque = TorqueForce * scale * (deltaAngle > 0f ? 1f : -1f);
                float thrust = ThrustForce * Mathf.Clamp01(dst / (Targets.Count != 0 ? ThrustDecreaseDistance : 7f));

                Movement.ForwardsThrust = thrust;
                Movement.Torque = torque;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (Targets == null)
                return;

            bool first = true;
            foreach (var point in Targets)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(Navigation.GetWorldPos(point), Vector3.one * 0.5f);
                if (first)
                {
                    first = false;
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(Navigation.GetWorldPos(point), ThrustDecreaseDistance);
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(Navigation.GetWorldPos(point), PointReachedDistance);
                }
            }
            if (!reachedEnd)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(finalTarget, Vector3.one * 0.5f);
            }
        }
    }
}

