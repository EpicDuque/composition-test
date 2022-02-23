using System;
using System.Collections;
using System.Collections.ObjectModel;
using CoolTools.Attributes;
using Unity.VisualScripting;
using UnityEngine;

namespace CoolTools.BehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public enum DataContainerType
        {
            BehaviourTreeBlackboard, VisualScriptingVariables
        }
        
        [SerializeField] private BehaviourTree tree;
        [Tooltip("Runs a cloned copy of this Behaviour Tree instead. Most useful for when objects instantiated at " +
                 "runtime use the same Behaviour Tree. This prevents all copies of that object from referencing the " +
                 "same Behaviour Tree asset (will obviously cause undesired results).")]
        [SerializeField] private bool cloneTree;

        [Space(10f)] 
        [SerializeField] private Blackboard blackboard;
        
        private BehaviourTree original;
        
        public BehaviourTree Tree
        {
            get => tree;
            set => tree = value;
        }

        public Blackboard Blackboard
        {
            get => blackboard;
            set => blackboard = value;
        }

        private DataContainerType containerType;

        public DataContainerType ContainerType
        {
            get => containerType;
            set => containerType = value;
        }

#if VISUAL_SCRIPTING
        [SerializeField] private Variables variables;
        public Variables Variables => variables;
#endif
        private void OnValidate()
        {
            #if VISUAL_SCRIPTING
            ContainerType = DataContainerType.VisualScriptingVariables;
            #endif
            UpdateBlackboard();
        }

        public void UpdateBlackboard()
        {
            if (tree == null) return;

            if(containerType == DataContainerType.BehaviourTreeBlackboard)
                tree.Blackboard = blackboard;
#if VISUAL_SCRIPTING
            else if (containerType == DataContainerType.VisualScriptingVariables)
                tree.Variables = variables;
#endif
        }

        private IEnumerator Start()
        {
            if (tree == null) yield break;

            enabled = false;
                
            tree.Blackboard = null;
            tree.Variables = null;
            
            if (cloneTree)
            {
                original = tree;
                tree = tree.Clone();
                
                yield return null;
                
            }
            
            UpdateBlackboard();

            yield return null;
            yield return null;
            yield return null;
            
            enabled = true;
            RestartTree();
        }

        private void Update()
        {
            tree.Update();
        }

        [ContextMenu("Restart Tree")]
        public void RestartTree()
        {
            tree.Restart();
        }
        
    }
}