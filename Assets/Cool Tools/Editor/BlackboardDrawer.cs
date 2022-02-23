#if UNITY_EDITOR

using System;
using CoolTools.BehaviourTree;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomEditor(typeof(Blackboard))]
public class BlackboardDrawer : Editor
{
    private Blackboard blackboard;
    private ReorderableList variableList;

    private void OnEnable()
    {
        blackboard = target as Blackboard;

        if (variableList == null)
        {
            variableList = new ReorderableList(serializedObject, serializedObject.FindProperty("variables"),
                true, true, true, true);
            
            variableList.drawElementCallback = DrawElementCallback;
            variableList.elementHeightCallback = ElementHeightCallback;
            variableList.drawHeaderCallback = DrawHeaderCallback;
            variableList.onRemoveCallback = ONRemoveCallback;
            variableList.onAddCallback = ONAddCallback;
        }
    }

    private void ONAddCallback(ReorderableList list)
    {
        var variable = new Variable {TypeCaption = typeof(int).FullName};
        blackboard.Variables.Add(variable);
    }

    private void ONRemoveCallback(ReorderableList list)
    {
        foreach (var index in list.selectedIndices)
        {
            var component = blackboard.Variables[index].ArrayHolder;
            
            DestroyImmediate(component);
            
            blackboard.Variables.RemoveAt(index);
        }
    }

    private void DrawHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "Variables");
    }

    private float ElementHeightCallback(int index)
    {
        var variable = blackboard.Variables[index];
        
        if(!variable.TypeCaption.Contains("[]"))
            return (EditorGUIUtility.singleLineHeight * 3) + 10;
        else
        {
            var list = variable.ArrayHolder.GetTypeArray();
            var height = EditorGUIUtility.singleLineHeight * 3 + 65;

            height += list?.Count * 15f ?? 0f;
            return height;
        }
    }

    private void DrawElementCallback(Rect position, int index, bool isactive, bool isfocused)
    {
        var variable = blackboard.Variables[index];

        var rect = new Rect(position){height = EditorGUIUtility.singleLineHeight};
        rect.y += 3; // Add some top padding
        
        DrawDropDownVariableType(rect, variable);
        rect.y += EditorGUIUtility.singleLineHeight + 2;
        
        variable.Name = EditorGUI.TextField(rect,new GUIContent("Name"), variable.Name);
        rect.y += EditorGUIUtility.singleLineHeight + 2;
        
        DrawValueField(rect, variable);
        rect.y += EditorGUIUtility.singleLineHeight + 2;
    }

    private void DrawValueField(Rect rect,Variable variable)
    {
        if (variable.TypeCaption.Equals(typeof(int).FullName))
        {
            variable.IntValue = EditorGUI.IntField(rect, new GUIContent("Value"), variable.IntValue);
        } else if (variable.TypeCaption.Equals(typeof(string).FullName))
        {
            variable.StringValue = EditorGUI.TextField(rect, new GUIContent("Value"), variable.StringValue);
        } else if (variable.TypeCaption.Equals(typeof(float).FullName))
        {
            variable.FloatValue = EditorGUI.FloatField(rect, new GUIContent("Value"), variable.FloatValue);
        } else if (variable.TypeCaption.Equals(typeof(bool).FullName))
        {
            variable.BoolValue = EditorGUI.Toggle(rect, new GUIContent("Value"), variable.BoolValue);
        } else if (variable.TypeCaption.Equals(typeof(Vector2).FullName))
        {
            variable.Vector2Value = EditorGUI.Vector2Field(rect, new GUIContent("Value"), variable.Vector2Value);
        } else if (variable.TypeCaption.Equals(typeof(Vector3).FullName))
        {
            variable.Vector3Value = EditorGUI.Vector3Field(rect, new GUIContent("Value"), variable.Vector3Value);
        } else if (variable.TypeCaption.Equals(typeof(Object).FullName))
        {
            variable.ObjectRefValue = EditorGUI.ObjectField(rect, new GUIContent("Value"), variable.ObjectRefValue, 
                typeof(Object), true);
        } else if (variable.TypeCaption.Contains("[]"))
        {
            var editor = CreateEditor(variable.ArrayHolder);
            DrawArrayTypeField(rect, editor, variable);
        }
    }

    private void DrawArrayTypeField(Rect rect, Editor editor, Variable variable)
    {
        if (editor == null) return;
        var so = editor.serializedObject;
        so.Update();

        var type = variable.ArrayHolder.Type;

        SerializedProperty property;
        switch (type)
        {
            case ArrayTypeValueHolder.ArrayType.Integer:
                property = so.FindProperty("IntegerList");
                break;
            case ArrayTypeValueHolder.ArrayType.Float:
                property = so.FindProperty("FloatList");
                break;
            case ArrayTypeValueHolder.ArrayType.Boolean:
                property = so.FindProperty("BooleanList");
                break;
            case ArrayTypeValueHolder.ArrayType.String:
                property = so.FindProperty("StringList");
                break;
            case ArrayTypeValueHolder.ArrayType.Vector2:
                property = so.FindProperty("Vector2List");
                break;
            case ArrayTypeValueHolder.ArrayType.Vector3:
                property = so.FindProperty("Vector3List");
                break;
            case ArrayTypeValueHolder.ArrayType.Object:
                property = so.FindProperty("ObjectList");
                break;
            default:
                throw new ArgumentOutOfRangeException();
            
        }

        var listRect = new Rect(rect){height = 100};
        
        EditorGUI.PropertyField(listRect, property, true);
        
        so.ApplyModifiedProperties();
    }

    private void DrawDropDownVariableType(Rect rect,Variable variable)
    {
        EditorGUI.PrefixLabel(rect, new GUIContent("Type"));

        var buttonRect = new Rect(rect)
        {
            x = EditorGUIUtility.labelWidth + 39,
            width = rect.width - EditorGUIUtility.labelWidth
        };
        
        if (EditorGUI.DropdownButton(buttonRect, new GUIContent(variable.TypeCaption), FocusType.Keyboard))
        {
            var menu = new GenericMenu();
            
            AddMenuItemType<int>(menu, variable, "Integer");
            AddMenuItemType<float>(menu, variable, "Float");
            AddMenuItemType<bool>(menu, variable, "Boolean");
            AddMenuItemType<string>(menu, variable, "String");
            AddMenuItemType<Vector2>(menu, variable, "Vector2");
            AddMenuItemType<Vector3>(menu, variable, "Vector3");
            AddMenuItemType<Object>(menu, variable, "Object Reference");
            
            menu.AddSeparator("");
            
            AddMenuItemType<int[]>(menu, variable, "Array/Integer");
            AddMenuItemType<float[]>(menu, variable, "Array/Float");
            AddMenuItemType<bool[]>(menu, variable, "Array/Boolean");
            AddMenuItemType<string[]>(menu, variable, "Array/String");
            AddMenuItemType<Vector2[]>(menu, variable, "Array/Vector2");
            AddMenuItemType<Vector3[]>(menu, variable, "Array/Vector3");
            AddMenuItemType<Object[]>(menu, variable, "Array/Object");
            
            menu.ShowAsContext();
        }
    }

    private void AddMenuItemType<T>(GenericMenu menu, Variable variable, string path)
    {
        menu.AddItem(new GUIContent(path),variable.TypeCaption.Equals(typeof(T).FullName),
            o => OnTypeSelected<T>(variable),
            typeof(T));
    }

    private void OnTypeSelected<T>(Variable variable)
    {
        variable.TypeCaption = typeof(T).FullName;

        if (!variable.TypeCaption.Contains("[]")) return;
        
        var holder = variable.ArrayHolder == null ? blackboard.gameObject.AddComponent<ArrayTypeValueHolder>() :
                variable.ArrayHolder;
        
        holder.Hide = true;
        holder.hideFlags = HideFlags.HideInInspector;
        variable.ArrayHolder = holder;
        
        if (typeof(T) == typeof(int[]))
        {
            holder.Type = ArrayTypeValueHolder.ArrayType.Integer;
        } else if (typeof(T) == typeof(float[]))
        {
            holder.Type = ArrayTypeValueHolder.ArrayType.Float;
        } else if (typeof(T) == typeof(bool[]))
        {
            holder.Type = ArrayTypeValueHolder.ArrayType.Boolean;
        } else if (typeof(T) == typeof(string[]))
        {
            holder.Type = ArrayTypeValueHolder.ArrayType.String;
        } else if (typeof(T) == typeof(Vector2[]))
        {
            holder.Type = ArrayTypeValueHolder.ArrayType.Vector2;
        } else if (typeof(T) == typeof(Vector3[]))
        {
            holder.Type = ArrayTypeValueHolder.ArrayType.Vector3;
        } else if (typeof(T) == typeof(Object[]))
        {
            holder.Type = ArrayTypeValueHolder.ArrayType.Object;
        }
    }

    // private void DrawTitleDesc()
    // {
    //     var titleProperty = serializedObject.FindProperty("title");
    //     var descProperty = serializedObject.FindProperty("description");
    //
    //     var oldSize = GUI.skin.textField.fontSize;
    //     
    //     GUI.skin.textField.fontSize = 15;
    //     GUI.skin.textField.fontStyle = FontStyle.Bold;
    //     titleProperty.stringValue = EditorGUILayout.TextField(titleProperty.stringValue, GUILayout.Height(22));
    //     
    //     GUI.skin.textField.fontStyle = FontStyle.Normal;
    //     GUI.skin.textField.fontSize = 13;
    //     descProperty.stringValue = EditorGUILayout.TextArea(descProperty.stringValue, GUILayout.Height(50));
    //     
    //     GUI.skin.textField.fontSize = oldSize;
    // }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // DrawTitleDesc();
        
        EditorGUILayout.Space();
        
        variableList.DoLayoutList();
        
        serializedObject.ApplyModifiedProperties();
    }

    
}
#endif