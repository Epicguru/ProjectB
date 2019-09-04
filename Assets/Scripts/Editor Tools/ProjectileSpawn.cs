
using JNetworking;
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

        if (!JNet.IsServer)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            startPos = InputManager.MousePos;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Vector2 direction = InputManager.MousePos - startPos;
            direction.Normalize();

            Projectile.Spawn(startPos, direction, Speed);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Projectile.Spawn(new Vector2(0f, -10f), Vector2.up, 15f);
        }
    }
}
