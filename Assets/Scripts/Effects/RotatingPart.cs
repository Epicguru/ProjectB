
using UnityEngine;

public class RotatingPart : MonoBehaviour
{
    public float RotationSpeed = 360f;

    private void Update()
    {
        transform.Rotate(0f, 0f, RotationSpeed * Time.deltaTime);
    }
}
