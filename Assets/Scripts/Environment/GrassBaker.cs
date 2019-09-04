
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassBaker : MonoBehaviour
{
    public GrassRenderer GRenderer;
    public LayerMask Mask;
    public Rect Bounds = new Rect(-10, -10, 20, 20);
    public float Density = 1f;
    [Range(0f, 1f)]
    public float Chance = 0.9f;
    public float Randomness = 0.1f;
    public bool StartBake = false;

    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (StartBake)
        {
            Bake();
            StartBake = false;
        }
    }

    public void Bake()
    {
        List<Vector2> points = new List<Vector2>();
        int w = Mathf.RoundToInt(Bounds.width * Density + 1);
        int h = Mathf.RoundToInt(Bounds.height * Density + 1);
        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if(Chance != 1f)
                {
                    if (Random.value > Chance)
                        continue;
                }

                Vector3 pos = new Vector3(Bounds.x + (x / Density), Bounds.y + (y / Density), -10f);

                if(Randomness != 0f)
                {
                    pos += (Vector3)Random.insideUnitCircle * Randomness;
                }

                RaycastHit2D hit = Physics2D.GetRayIntersection(new Ray(pos, Vector3.forward), 20f, Mask);
                if (hit)
                {
                    points.Add(pos);
                }
            }
        }

        Debug.Log($"Finished baking {points.Count} points.");
        GRenderer.Points = points.ToArray();
        points.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Bounds.center, Bounds.size);
    }
}
