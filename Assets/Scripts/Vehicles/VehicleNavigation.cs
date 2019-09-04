
using System.Collections.Generic;
using ThreadedPathfinding;
using ThreadedPathfinding.Internal;
using UnityEngine;

public class VehicleNavigation : MonoBehaviour
{
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

    public List<PNode> Targets;

    public float PointReachedDistance = 2f;

    [Header("Thrust")]
    public float ThrustForce = 1000f;
    public float ThrustDecreaseDistance = 10f;

    [Header("Torque")]
    public float TorqueForce = 100f;
    public float TorqueAngleMax = 90f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var start = Navigation.GetClosestTile(transform.position);
            var end = Navigation.GetClosestTile(InputManager.MousePos);
            var result = new Pathfinding().Run(start.x, start.y, end.x, end.y, Navigation.Instance, Targets);
            Debug.Log(result);
        }

        if(Targets == null || Targets.Count == 0)
        {
            Movement.ForwardsThrust = 0f;
            Movement.Torque = 0f;
        }
        else
        {
            Vector2 point = Navigation.GetWorldPos(Targets[0]);
            if (point.DistanceCheck(transform.position, PointReachedDistance))
            {
                if (Targets.Count > 1)
                {
                    point = Navigation.GetWorldPos(Targets[1]);
                    Targets.RemoveAt(0);
                }
                else
                {
                    Targets.Clear();
                }
            }
            Vector2 currentPos = transform.position;
            float currentAngle = transform.localEulerAngles.z;
            float dst = currentPos.DistanceTo(point);

            float angle = currentPos.AngleTowards(point);
            float deltaAngle = Mathf.DeltaAngle(currentAngle, angle);
            float scale = Mathf.Abs(deltaAngle) / TorqueAngleMax;

            float torque = TorqueForce * scale * (deltaAngle > 0f ? 1f : -1f);
            float thrust = ThrustForce * Mathf.Clamp01(dst / ThrustDecreaseDistance);

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
    }
}
