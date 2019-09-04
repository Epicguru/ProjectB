
using UnityEngine;

public class ProjectileSpawn : EditorTool
{
    public Projectile Projectile;
    public float Speed = 10f;

    private Vector2 startPos;

    public override void DrawWindow()
    {
        GUILayout.Label("Press and hold space bar to aim and fire a projectile.");
        GUILayout.Space(10);
        GUILayout.Label($"Speed: {Speed:F1}");
        Speed = GUILayout.HorizontalSlider(Speed, 0f, 500f);
    }

    private void Update()
    {
        if (Projectile == null)
            return;
        if (!IsEnabled)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            startPos = InputManager.MousePos;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Vector2 direction = InputManager.MousePos - startPos;
            direction.Normalize();

            Projectile p = PoolObject.Spawn(Projectile);
            p.transform.position = startPos;
            p.Direction = direction;
            p.Speed = Speed;
        }
    }
}
