
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

        public bool HasPath { get { return Path.Count > 0; } }
        public Vector2 CurrentTargetPos { get { return HasPath ? Path[Path.Count - 1].Position : Vector2.zero; } }
        public float CurrentTargetAngle { get { return HasPath ? Path[Path.Count - 1].TargetAngle : 0f; } }
        protected readonly List<TargetPoint> Path = new List<TargetPoint>();
        private static List<PNode> Nodes = new List<PNode>();

        public float PointReachedDistance = 2f;

        [Header("Thrust")]
        public float ThrustForce = 1000f;
        public float ThrustDecreaseDistance = 10f;

        [Header("Torque")]
        public float TorqueForce = 100f;
        public float TorqueAngleMax = 90f;

        private UponPathCompleted uponComplete;
        public delegate void UponPathCompleted(Vehicle v, bool completed);

        private void Update()
        {
            if (Input.GetMouseButtonDown(1) && Vehicle.Unit.IsSelected && !InputManager.IsMouseInUI)
            {
                MakePathToPos(InputManager.MousePos, 0f, null, true);                
            }

            if (!HasPath)
            {
                Movement.ForwardsThrust = 0f;
                Movement.Torque = 0f;
            }
            else
            {
                TargetPoint point = Path[0];
                if (point.Position.DistanceCheck(transform.position, point.MinDistance))
                {
                    if(Path.Count != 1)
                    {
                        Path.RemoveAt(0);
                    }
                    else
                    {
                        float delta = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, point.TargetAngle));
                        if (delta <= 10f)
                        {
                            // Path complete!
                            uponComplete?.Invoke(this.Vehicle, true);
                            uponComplete = null;
                            Path.RemoveAt(0);
                            return;
                        }
                    }
                }
                Vector2 currentPos = transform.position;
                float currentAngle = transform.eulerAngles.z;
                float dst = currentPos.DistanceTo(point.Position);

                float finalCurveP = Mathf.Clamp01(dst / (ThrustDecreaseDistance * 0.5f));
                float finalAngle = Mathf.LerpAngle(point.TargetAngle, currentPos.AngleTowards(point.Position), finalCurveP); 

                float angle = Path.Count == 1 ? finalAngle : currentPos.AngleTowards(point.Position);
                float deltaAngle = Mathf.DeltaAngle(currentAngle, angle);
                float scale = Mathf.Abs(deltaAngle) / TorqueAngleMax;

                float torque = TorqueForce * scale * (deltaAngle > 0f ? 1f : -1f);
                float thrust = ThrustForce * Mathf.Clamp01(dst / ThrustDecreaseDistance);

                if(Path.Count == 1 && Movement.Body.velocity.sqrMagnitude > 1f)
                {
                    thrust *= -1f;
                }

                Movement.ForwardsThrust = thrust;
                Movement.Torque = torque;
            }
        }

        public PathfindingResult MakePathToPos(Vector2 worldPos, float angle, UponPathCompleted uponComplete, bool autoAngle = false)
        {
            Path.Clear();

            var start = Navigation.GetClosestTile(transform.position);
            var end = Navigation.GetClosestTile(worldPos);
            var result = new Pathfinding().Run(start.x, start.y, end.x, end.y, Navigation.Instance, Nodes);

            if(result == PathfindingResult.SUCCESSFUL)
            {
                foreach (var node in Nodes)
                {
                    Vector2 pos = Navigation.GetWorldPos(node);

                    Path.Add(new TargetPoint(pos, PointReachedDistance));
                }
                this.uponComplete = uponComplete;

                // If auto angle, make the angle be the same angle as when travelling between the second-
                // to-last point and the last point.
                if (autoAngle)
                {
                    angle = (Path[Path.Count - 1].Position - Path[Path.Count - 2].Position).ToAngle();
                }

                Path.Add(new TargetPoint(worldPos, 1f, angle));
            }
            else if(result == PathfindingResult.ERROR_START_IS_END)
            {
                // When the end position is in the same navigation tile, just move straight to the target.
                Path.Add(new TargetPoint(worldPos, 1f, autoAngle ? 90f : angle));
                return PathfindingResult.SUCCESSFUL;
            }
            else
            {
                uponComplete?.Invoke(Vehicle, false);
            }

            return result;
        }

        public void ClearPath()
        {
            Path.Clear();
            uponComplete?.Invoke(Vehicle, false);
            uponComplete = null;
        }

        private void OnDrawGizmosSelected()
        {
            if (Path == null)
                return;

            bool first = true;
            foreach (var point in Path)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(point.Position, Vector3.one * 0.5f);
                if (first)
                {
                    first = false;
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(point.Position, ThrustDecreaseDistance);
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(point.Position, PointReachedDistance);
                }
            }
            if (HasPath)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(CurrentTargetPos, Vector3.one * 0.5f);
                Gizmos.DrawLine(CurrentTargetPos, CurrentTargetPos + CurrentTargetAngle.ToDirection() * 2f);
            }
        }

        [System.Serializable]
        public struct TargetPoint
        {
            public Vector2 Position;
            public float MinDistance;
            public float TargetAngle;

            public TargetPoint(Vector2 pos, float minDistance, float targetAngle = 0f)
            {
                this.Position = pos;
                this.MinDistance = minDistance;
                this.TargetAngle = targetAngle;
            }
        }
    }
}

