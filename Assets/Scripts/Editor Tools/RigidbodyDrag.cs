using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyDrag : EditorTool
{
    public float ForceScalar = 1000f;

    public override void DrawWindow()
    {
        GUILayout.Label($"Force: {ForceScalar:F1} Newtons");
        ForceScalar = GUILayout.HorizontalSlider(ForceScalar, 0.1f, 10000f);
    }

    private void FixedUpdate()
    {
        if (!IsEnabled)
            return;

        var rb = Tools.SelectedRigidbody;
        if(rb != null)
        {
            Vector2 mouse = InputManager.MousePos;
            Vector2 diff = (mouse - rb.worldCenterOfMass);

            if(Input.GetMouseButton(1))
                rb.AddForce(diff.normalized * ForceScalar, ForceMode2D.Force);
        }
    }
}
