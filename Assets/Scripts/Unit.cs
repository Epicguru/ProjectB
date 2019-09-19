
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public static List<Unit> AllActiveUnits = new List<Unit>();

    public static void DeselectAll()
    {
        foreach (var unit in AllActiveUnits)
        {
            unit.IsSelected = false;
        }
    }

    public static void GL_DrawAllSelected()
    {
        foreach (var unit in AllActiveUnits)
        {
            if (unit.IsSelected)
            {
                unit.GL_DrawSelected();
            }
        }
    }

    public string Name = "Unit Name";
    public bool IsSelected;
    public Rect Bounds { get; set; }

    private void Awake()
    {
        AllActiveUnits.Add(this);
    }

    private void OnDestroy()
    {
        AllActiveUnits.Remove(this);
    }

    public virtual void UpdateBounds()
    {
        // Standard implementation.
        const float SIZE = 0.25f; // 2.5 meter in each axis.
        Rect r = new Rect((Vector2)transform.position - new Vector2(SIZE, SIZE) * 0.5f, new Vector2(SIZE, SIZE));
        Bounds = r;

        SendMessage("UpdateUnitBounds", this, SendMessageOptions.DontRequireReceiver);
    }

    public virtual void GL_DrawSelected()
    {
        SendMessage("GL_DrawUnitSelected", this, SendMessageOptions.DontRequireReceiver);
    }
}
