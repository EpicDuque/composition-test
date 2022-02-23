using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObjectReplacementTool : ToolWindow
{
    private List<GameObject> selectedObjects;
    private GameObject replacementObject;
    private bool applyScale;
    private bool disable;
    private bool applyRotation;

    [MenuItem("Window/CoolTools/Object Replace")]
    private static void ShowWindow()
    {
        var window = GetWindow<ObjectReplacementTool>();
        window.titleContent = new GUIContent("Object Replace");
        window.Show();
    }
    
    public void OnInspectorUpdate()
    {
        // This will only get called 10 times per second.
        Repaint();
    }

    private void OnFocus()
    {
        
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Assign an object to replace selection with.", 
            MessageType.Info);

        replacementObject = EditorGUILayout.ObjectField(replacementObject, typeof(GameObject), true) as GameObject;
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox("Select Objects in the Hierarchy to replace. " +
                                "\n TIP: Save your selection using Edit -> Selection -> Save Selection",
            MessageType.Info);
        
        if(hasFocus)
            selectedObjects = Selection.gameObjects.ToList();
        
        EditorGUILayout.LabelField($"Selected Object Count: {selectedObjects.Count}");

        if (selectedObjects.Count == 0) return;
        
        EditorGUILayout.Space();

        applyScale = EditorGUILayout.Toggle("Apply Scale", applyScale);
        applyRotation = EditorGUILayout.Toggle("Apply Rotation", applyRotation);
        disable = EditorGUILayout.Toggle("Disable Instead of Delete", disable, GUILayout.Width(300));
        
        if (GUILayout.Button("Replace"))
        {
            var positions = selectedObjects.Select(o => o.transform.position).ToArray();
            var rotations = selectedObjects.Select(o => o.transform.rotation).ToArray();
            var parents = selectedObjects.Select(o => o.transform.parent).ToArray();
            var scales = selectedObjects.Select(o => o.transform.localScale).ToArray();
            
            for (int i = 0; i < selectedObjects.Count; i++)
            {
                var spawn = PrefabUtility.InstantiatePrefab(replacementObject) as GameObject;
                
                spawn.transform.position = positions[i];
                
                if(applyRotation)
                    spawn.transform.rotation = rotations[i];
                
                spawn.transform.SetParent(parents[i]);
                
                if (applyScale)
                {
                    spawn.transform.localScale = scales[i];
                }
            }

            if (disable)
            {
                selectedObjects.ForEach(o => o.SetActive(false));
            }
            else
            {
                selectedObjects.ForEach(DestroyImmediate);
            }
            
            selectedObjects.Clear();
        }
    }

    // private void DrawSelectionArray()
    // {
    //     var selArray = selectedObjects.Cast<Object>().ToArray();
    //     
    //     if (selectedObjects.Count == 0) return;
    //
    //     for (var i = 0; i < selArray.Length; i++)
    //     {
    //         DrawField(ref selArray[i]);
    //     }
    // }
}
