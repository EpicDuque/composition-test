using CoolTools.Attributes;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(InspectorDisabledAttribute))]
public class InspectorDisabledDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginDisabledGroup(true);

        EditorGUI.PropertyField(position, property, label);
        
        EditorGUI.EndDisabledGroup();
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}
