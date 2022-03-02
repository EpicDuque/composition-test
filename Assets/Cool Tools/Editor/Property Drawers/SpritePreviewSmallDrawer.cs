using CoolTools.Attributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SpritePreviewSmallAttribute))]
public class SpritePreviewDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType != SerializedPropertyType.ObjectReference)
        {
            EditorGUI.PropertyField(position, property, label);
            EditorGUI.EndProperty();
        }

        var iconRect = new Rect(position)
        {
            x = EditorGUIUtility.labelWidth,
            width = 19,
        };

        var sp = (Sprite) property.objectReferenceValue;
        if (sp != null)
        {
            GUI.DrawTexture(iconRect, sp.texture);
        }
        
        EditorGUI.PropertyField(position, property, label);
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}