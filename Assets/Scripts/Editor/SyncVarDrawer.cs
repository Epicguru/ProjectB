using JNetworking;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SyncVarAttribute))]
public class SyncVarDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public static Texture2D icon;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (icon == null)
            icon = Resources.Load<Texture2D>("JNetResources/SyncVarIcon");

        label.image = icon;
        var attr = attribute as SyncVarAttribute;
        label.tooltip = label.tooltip?.Trim() + $" SyncVar - Hook: {attr.Hook ?? "none"}, First Only: {attr.FirstOnly}";

        EditorGUI.PropertyField(position, property, label, true);
    }
}