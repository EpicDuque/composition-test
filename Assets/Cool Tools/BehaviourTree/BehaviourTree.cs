using System.Collections.Generic;
using CoolTools.Attributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

namespace CoolTools.BehaviourTree
{
    [CreateAssetMenu(menuName = "Behaviour Tree/Tree")]
    public class BehaviourTree : ScriptableObject
    {
        [SerializeField, InspectorDisabled] protected List<Node> nodes = new();
        [SerializeField, HideInInspector] protected RootNode rootNode;
        [SerializeField, InspectorDisabled] protected Node.State treeState;
        [SerializeField, HideInInspector] protected Blackboard blackboard;
        
        
        public RootNode RootNode
        {
            get => rootNode;
            set
            {
                rootNode = value;
                
                Traverse(rootNode, node => node.Tree = this);
            }
        }

        public Blackboard Blackboard
        {
            get => blackboard;
            set => blackboard = value;
        }
        
        public Node.State TreeState
        {
            get => treeState;
            set => treeState = value;
        }
        
        public List<Node> Nodes => nodes;
        
#if VISUAL_SCRIPTING
        [SerializeField, HideInInspector] private Variables variables;
        public Variables Variables
        {
            get => variables;
            set => variables = value;
        }
#endif

        public void Restart()
        {
            RootNode.Restart();
            TreeState = Node.State.Running;
        }
        
        public Node.State Update()
        {
            if(RootNode.state == Node.State.Running)
                TreeState = RootNode.Update();

            return TreeState;
        }

        #if UNITY_EDITOR
        public Node CreateNode(string className)
        {
            var node = CreateInstance(className) as Node;
            node.name = className.Replace("CoolTools.BehaviourTree.", "");
            
            return SetupNewNode(node);
        }
        
        public Node CreateNode(System.Type type)
        {
            var node = CreateInstance(type) as Node;
            node.name = type.Name;

            return SetupNewNode(node);
        }

        private Node SetupNewNode(Node node)
        {
            node.guid = GUID.Generate().ToString();
            node.Tree = this;

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            Nodes.Add(node);

            // if (!Application.isPlaying)
            // {
            //     AssetDatabase.AddObjectToAsset(node, this);
            // }
            AssetDatabase.AddObjectToAsset(node, this);
            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");
            
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
            Nodes.Remove(node);
            // AssetDatabase.RemoveObjectFromAsset(node);
            
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            if (parent is DecoratorNode decNode)
            {
                Undo.RecordObject(decNode, "Behaviour Tree (AddChild DecoratorNode)");
                decNode.Child = child;
                EditorUtility.SetDirty(decNode);
            }

            if (parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (Addchild RootNode)");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }
            
            if (parent is CompositeNode compNode)
            {
                Undo.RecordObject(compNode, "Behaviour Tree (AddChild CompositeNode)");
                compNode.Children.Add(child);
                EditorUtility.SetDirty(compNode);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            if (parent is DecoratorNode decNode)
            {
                Undo.RecordObject(decNode, "Behaviour Tree (RemoveChild DecoratorNode)");
                decNode.Child = null;
                EditorUtility.SetDirty(decNode);
            }
            
            if (parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild RootNode)");
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
            }
            
            if (parent is CompositeNode compNode)
            {
                Undo.RecordObject(compNode, "Behaviour Tree (RemoveChild CompNode)");
                compNode.Children.Remove(child);
                EditorUtility.SetDirty(compNode);
            }
        }
        #endif

        public List<Node> GetChildren(Node parent)
        {
            var list = new List<Node>();
            if (parent is DecoratorNode decNode && decNode.Child != null)
            {
                list.Add(decNode.Child);
            }
            
            if (parent is RootNode rootNode && rootNode.child != null)
            {
                list.Add(rootNode.child);
            }
            
            if (parent is CompositeNode compNode)
            {
                return compNode.Children;
            }

            return list;
        }

        public void Traverse(Node node, System.Action<Node> visitor)
        {
            if (!node) return;
            
            visitor.Invoke(node);
            var children = GetChildren(node);
            children.ForEach(c => Traverse(c, visitor));
        }

        public BehaviourTree Clone()
        {
            var tree = Instantiate(this);
            
            tree.RootNode = RootNode.Clone() as RootNode;
            tree.nodes = new List<Node>();
            tree.Blackboard = Blackboard;
            tree.Variables = Variables;
            
            Traverse(tree.RootNode, n =>
            {
                n.Tree = tree;
                tree.nodes.Add(n);
            });
            
            return tree;
        }
        
    }
}