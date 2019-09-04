using UnityEngine;

[RequireComponent(typeof(VehicleMovement))]
public class Vehicle : MonoBehaviour
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

    public string Name = "My Vehicle";
}
