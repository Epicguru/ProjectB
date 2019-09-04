
using UnityEngine;

[System.Serializable]
[RequireComponent(typeof(MonoCollisionSurface))]
public class HealthPart : MonoBehaviour
{
    public string Name = "Part Name";
    public HealthPartID ID = HealthPartID.HULL;
    public Collider2D Collider;
    public bool IsVital = false;
    public float MaxHealth = 100f;
    public float Health = 100f;

    private void Awake()
    {
        if(Collider == null)
        {
            Collider = GetComponent<Collider2D>();
        }
    }

    public ICollisionSurface GetCollisionSurface()
    {
        return Collider?.GetComponent<MonoCollisionSurface>();
    }
}