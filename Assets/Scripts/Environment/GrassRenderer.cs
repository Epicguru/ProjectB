using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GrassRenderer : MonoBehaviour
{
    public SpriteShapeController Renderer;
    public Mesh Mesh;
    public Material Mat;

    public Vector2 Pos;
    public Vector2Int Size;
    public float Spacing;

    public float WindAngle = 0f;
    public float WindMagnitude = 0.1f;

    [Header("Wave")]
    public float OffsetAngleSpeed = 1f;
    public float OffsetAnglemag = 0.01f;
    public float OffsetSpeed = 1f;

    private Vector2 offset;
    [SerializeField]
    private float angle;
    private float timer;

    [HideInInspector]
    public Vector2[] Points;

    private void Update()
    {
        if (Points == null)
            return;

        Vector2 wind = new Vector2(Mathf.Cos(WindAngle * Mathf.Deg2Rad), Mathf.Sin(WindAngle * Mathf.Deg2Rad)) * WindMagnitude;
        Mat.SetVector("_WindDir", wind);

        timer += Time.deltaTime * OffsetAngleSpeed;
        angle += (Mathf.PerlinNoise(timer, 0f) - 0.5f) * OffsetAnglemag;
        offset += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * OffsetSpeed * Time.deltaTime;

        Mat.SetVector("_WaveOffset", offset);

        for (int i = 0; i < Points.Length; i++)
        {
            Vector3 pos = Points[i];
            pos.z = -0.1f;

            if (!InputManager.CameraBounds.Contains(pos))
                continue;

            Graphics.DrawMesh(Mesh, pos, Quaternion.identity, Mat, 0);
        }
    }
}
