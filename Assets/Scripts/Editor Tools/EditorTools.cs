using JNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EditorTools : MonoBehaviour
{
    public Rigidbody2D SelectedRigidbody;

    private EditorTool[] tools;
    private Vector2 lastVel;
    private Vector2 currentVel;
    private bool PhysicsPaused = false;

    private void Awake()
    {
        tools = GetComponentsInChildren<EditorTool>();

        const int WIDTH = 200;
        const int HEIGHT = 200;
        const int SPACE = 20;
        for (int i = 0; i < tools.Length; i++)
        {
            var tool = tools[i];
            tool.WindowRect = new Rect(60 + i * (WIDTH + SPACE), 60, WIDTH, HEIGHT);
            tool.Tools = this;
        }
        InvokeRepeating("StepTick", 0.2f, 0.2f);
    }

    private void Update()
    {
        bool pressed = Input.GetMouseButtonDown(2);

        if (pressed)
        {
            SelectedRigidbody = null;
            var hit = Physics2D.GetRayIntersection(new Ray((Vector3)InputManager.MousePos + Vector3.forward * -20f, Vector3.forward));
            if (hit)
            {
                SelectedRigidbody = hit.transform.GetComponentInParent<Rigidbody2D>();
                if(SelectedRigidbody != null)
                {
                    lastVel = SelectedRigidbody.velocity;
                }
            }
        }

        PhysicsSim.Paused = PhysicsPaused;
    }

    private void OnGUI()
    {
        bool netWorking = JNet.IsClient || JNet.IsServer;
        if (!netWorking)
            return;

        PhysicsPaused = GUILayout.Toggle(PhysicsPaused, "Physics Pause");

        GUILayout.Label($"Selected Rigidbody: {(SelectedRigidbody == null ? "null" : SelectedRigidbody.gameObject.name)}");
        if(SelectedRigidbody != null)
            GUILayout.Label(SelectedRigidbody.MovementStats() + $", Est. Acceleration: {((currentVel - lastVel) / 0.2f).magnitude:F1} ㎨");

        if (SelectedRigidbody != null)
        {
            Rect r = SelectedRigidbody.GetBounds().ToScreenBounds();
            GuiUtils.DrawBox(r, Color.green);
        }

        for(int i = 0; i < tools.Length; i++)
        {
            var tool = tools[i];
            if(GUILayout.Button($"{(tool.IsEnabled ? "Hide " : "Show ")} {tool.Name.Trim()}"))
            {
                tool.IsEnabled = !tool.IsEnabled;
            }

            if (tool.IsEnabled)
            {
                tool.WindowRect = GUILayout.Window(i, tool.WindowRect, tool.DrawWindowInternal, tool.WindowTitle, GUILayout.MaxWidth(1000), GUILayout.MaxHeight(1000));
            }
        }        
    }

    private void StepTick()
    {
        if (SelectedRigidbody == null)
            return;

        lastVel = currentVel;
        currentVel = SelectedRigidbody.velocity;
    }
}
