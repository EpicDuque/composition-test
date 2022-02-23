using UnityEditor;
using UnityEngine;

namespace CoolTools.BehaviourTree
{
    [CustomEditor(typeof(BehaviourTreeRunner))]
    public class BehaviourTreeRunnerEditor : Editor
    {
        private BehaviourTreeRunner script;

        private void OnEnable()
        {
            script = target as BehaviourTreeRunner;
            if (script == null) return;
            
#if VISUAL_SCRIPTING
            script.ContainerType = BehaviourTreeRunner.DataContainerType.VisualScriptingVariables;
#else
            script.ContainerType = BehaviourTreeRunner.DataContainerType.BehaviourTreeBlackboard;
#endif
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var iterator = serializedObject.GetIterator();
            iterator.NextVisible(true); // Hide Script Field

            while (iterator.NextVisible(true))
            {
                if (iterator.name.Equals("blackboard") && script.ContainerType !=
                    BehaviourTreeRunner.DataContainerType.BehaviourTreeBlackboard)
                {
                    continue;
                }
                
                if (iterator.name.Equals("variables") && script.ContainerType !=
                    BehaviourTreeRunner.DataContainerType.VisualScriptingVariables)
                {
                    continue;
                }
                    // Draw open asset button below tree property
                if (iterator.name.Equals("tree"))
                {
                    var oldLabelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth -= 55;

                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUILayout.PropertyField(iterator);
                    if (GUILayout.Button("Edit Tree", GUILayout.Width(100)))
                    {
                        BehaviourTreeEditor.OpenTreeAset(script.gameObject, script.Tree);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.Space(10f);
                    EditorGUIUtility.labelWidth = oldLabelWidth;
                    continue;
                }

                EditorGUILayout.PropertyField(iterator);

            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}