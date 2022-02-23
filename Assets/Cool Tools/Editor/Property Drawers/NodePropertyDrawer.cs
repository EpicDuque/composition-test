using System;
using System.Collections.Generic;
using System.Linq;
using CoolTools.BehaviourTree;
using CoolTools.Types;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(NodeProperty), true)]
public class NodePropertyDrawer : PropertyDrawer
{
    private PropertyValueType valueType;
    private string variableName = "";
    private Node node;
    public SerializedProperty propertyValue;

    public override void OnGUI(Rect position, SerializedProperty property,
        GUIContent label)
    {
        if(node == null)
            node = property.serializedObject.targetObject as Node;

        propertyValue = property.FindPropertyRelative("Value");
        valueType = (PropertyValueType) property.FindPropertyRelative("valueType").enumValueIndex;
        
        EditorGUI.BeginProperty(position, label, property);

        var rect = new Rect(position) {height = EditorGUIUtility.singleLineHeight};
        rect.y += 6;
        
        EditorGUI.BeginDisabledGroup(valueType == PropertyValueType.Blackboard || valueType == PropertyValueType.Variable);

#if VISUAL_SCRIPTING
        if (valueType == PropertyValueType.Blackboard)
        {
            property.FindPropertyRelative("valueType").enumValueIndex = (int) PropertyValueType.Variable;
            valueType = PropertyValueType.Variable;
        }
#endif

        if (valueType == PropertyValueType.Blackboard)
        {
            AssignValueFromBlackboard(property, variableName);
        }
#if VISUAL_SCRIPTING
        else if (valueType == PropertyValueType.Variable)
        {
            AssignValueFromVariable(property, variableName);
        }
#endif

        if (propertyValue.propertyType == SerializedPropertyType.ObjectReference)
        {
            EditorGUI.PrefixLabel(rect, label);
            var labelRect = new Rect(rect)
            {
                x = EditorGUIUtility.labelWidth,
                width = rect.width - EditorGUIUtility.labelWidth,
            };

            if (!string.IsNullOrEmpty(variableName))
            {
#if VISUAL_SCRIPTING
                var referenceName = (node.Tree.Variables.declarations.Get(variableName) as Object).name;
#else
                var referenceName = node.Tree.Blackboard.GetVariable(variableName).ObjectRefValue.name;
#endif
                
                EditorGUI.LabelField(labelRect, referenceName);
            }
        }
        else
        {
            EditorGUI.PropertyField(rect, propertyValue, label);
        }
        rect.y += EditorGUIUtility.singleLineHeight + 2;
        
        EditorGUI.EndDisabledGroup();
        
        valueType = (PropertyValueType) EditorGUI.EnumPopup(rect, new GUIContent("Value Type"), valueType);
        property.FindPropertyRelative("valueType").enumValueIndex = (int) valueType;
        rect.y += EditorGUIUtility.singleLineHeight + 2;

        var variableNames = new List<string>();
        switch (valueType)
        {
            case PropertyValueType.Blackboard:
                variableNames = GetQualifyingBlackboardVariables()?.Select(v => v.Name).ToList();
                break;
            
#if VISUAL_SCRIPTING
            case PropertyValueType.Variable:
                variableNames = GetQualifyingVariableDeclarations()?.Select(v => v.name).ToList();
                break;
#endif
            
            case PropertyValueType.Literal:
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (string.IsNullOrEmpty(variableName))
        {
            if (variableNames == null) return;
            
            if (variableNames.Count > 0) variableName = variableNames[0];
        }
        
        if (EditorGUI.DropdownButton(rect, new GUIContent(variableName), FocusType.Keyboard))
        {
            var menu = new GenericMenu();
            foreach (var v in variableNames)
            {
                menu.AddItem(new GUIContent(v), variableName.Equals(v), OnMenuVariableSelected, v);
            }
            menu.ShowAsContext();
        }
        
        rect.y += EditorGUIUtility.singleLineHeight + 6;
        
        EditorGUI.EndProperty();
    }
    
    private void AssignValueFromBlackboard(SerializedProperty property, string varName)
    {
        var variable = node.Tree.Blackboard.Variables.FirstOrDefault(v => v.Name.Equals(varName));
        if (variable == null) return;
        
        switch (propertyValue.propertyType)
        {
            case SerializedPropertyType.Integer:
                propertyValue.intValue = variable.IntValue;
                break;
                    
            case SerializedPropertyType.Boolean:
                propertyValue.boolValue = variable.BoolValue;
                break;
                    
            case SerializedPropertyType.Float:
                propertyValue.floatValue = variable.FloatValue;
                break;
                    
            case SerializedPropertyType.String:
                propertyValue.stringValue = variable.StringValue;
                break;
                    
            case SerializedPropertyType.ObjectReference:
                propertyValue.objectReferenceValue = variable.ObjectRefValue;
                break;

            case SerializedPropertyType.Vector2:
                propertyValue.vector2Value = variable.Vector2Value;
                break;
                    
            case SerializedPropertyType.Vector3:
                propertyValue.vector3Value = variable.Vector3Value;
                break;
        }

        property.serializedObject.ApplyModifiedProperties();
    }
    
    private IEnumerable<Variable> GetQualifyingBlackboardVariables()
    {
        if (node.Tree.Blackboard == null) return Array.Empty<Variable>();
        
        return node.Tree.Blackboard.Variables.Where(v =>
        {
            switch (propertyValue.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return v.TypeCaption.Equals(typeof(int).FullName);

                case SerializedPropertyType.Boolean:
                    return v.TypeCaption.Equals(typeof(bool).FullName);

                case SerializedPropertyType.Float:
                    return v.TypeCaption.Equals(typeof(float).FullName);

                case SerializedPropertyType.String:
                    return v.TypeCaption.Equals(typeof(string).FullName);

                case SerializedPropertyType.ObjectReference:
                    return v.TypeCaption.Equals(typeof(Object).FullName);

                case SerializedPropertyType.Vector2:
                    return v.TypeCaption.Equals(typeof(Vector2).FullName);

                case SerializedPropertyType.Vector3:
                    return v.TypeCaption.Equals(typeof(Vector3).FullName);
            }

            return false;
        });
        
    }
    
    private void OnMenuVariableSelected(object userdata)
    {
        variableName = (string)userdata;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 4f;
    }
    
#if VISUAL_SCRIPTING
    private IEnumerable<VariableDeclaration> GetQualifyingVariableDeclarations()
    {
        if (node.Tree.Variables == null) return null;
        
        return node.Tree.Variables.declarations.Where(v =>
        {
            switch (propertyValue.propertyType)
            {
                case SerializedPropertyType.Integer:
                    return v.value.IsConvertibleTo(typeof(int), true);

                case SerializedPropertyType.Boolean:
                    return v.value.IsConvertibleTo(typeof(bool), true);

                case SerializedPropertyType.Float:
                    return v.value.IsConvertibleTo(typeof(float), true);

                case SerializedPropertyType.String:
                    return v.value.IsConvertibleTo(typeof(string), true);

                case SerializedPropertyType.ObjectReference:
                    return v.value.IsConvertibleTo(typeof(Object), true);

                case SerializedPropertyType.Vector2:
                    return v.value.IsConvertibleTo(typeof(Vector2), true);

                case SerializedPropertyType.Vector3:
                    return v.value.IsConvertibleTo(typeof(Vector3), true);
            }

            return false;
        });
    }

    private void AssignValueFromVariable(SerializedProperty property, string varName)
    {
        if (string.IsNullOrEmpty(varName))
            return;
        
        var value = node.Tree.Variables.declarations.Get(varName);
        
        switch (propertyValue.propertyType)
        {
            case SerializedPropertyType.Integer:
                propertyValue.intValue = (int)value;
                break;
                    
            case SerializedPropertyType.Boolean:
                propertyValue.boolValue = (bool)value;
                break;
                    
            case SerializedPropertyType.Float:
                propertyValue.floatValue = (float)value;
                break;
                    
            case SerializedPropertyType.String:
                propertyValue.stringValue = (string)value;
                break;
                    
            case SerializedPropertyType.ObjectReference:
                propertyValue.objectReferenceValue = (Object)value;
                break;

            case SerializedPropertyType.Vector2:
                propertyValue.vector2Value = (Vector2)value;
                break;
                    
            case SerializedPropertyType.Vector3:
                propertyValue.vector3Value = (Vector3)value;
                break;
        }

        property.serializedObject.ApplyModifiedProperties();

    }
    
#endif
}
