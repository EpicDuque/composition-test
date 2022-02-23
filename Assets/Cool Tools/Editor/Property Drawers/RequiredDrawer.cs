using CoolTools.Attributes;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RequiredAttribute))]
public class RequiredDrawer : PropertyDrawer
{
    private bool error;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        error = property.propertyType != SerializedPropertyType.ObjectReference ||
                property.objectReferenceValue == null;
        
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            if (property.objectReferenceValue == null)
            {
                EditorGUI.HelpBox(new Rect(position)
                {
                    height = EditorGUIUtility.singleLineHeight * 2,
                },"This Field is required.", MessageType.Error);
                position.y += EditorGUIUtility.singleLineHeight * 2 + 5;
            }
        }
        else
        {
            EditorGUI.HelpBox(new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight * 2,
            },"RequiredAttribute only works on Object Reference properties.", MessageType.Warning);
            position.y += EditorGUIUtility.singleLineHeight * 2 + 5;
        }

        var propHeight = base.GetPropertyHeight(property, label);
        EditorGUI.PropertyField(new Rect(position) {height = propHeight}, property, label);
        
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return error ? 
            base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight * 2 + 5 :
            base.GetPropertyHeight(property, label);
    }
}
