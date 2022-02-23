#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using CoolTools.BehaviourTree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = CoolTools.BehaviourTree.Node;

public class TreeNodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private BehaviourTreeView graphView;
    private EditorWindow editorWindow;

    public void Init(EditorWindow window, BehaviourTreeView view)
    {
        editorWindow = window;
        graphView = view;
    }
    
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var decoratorNodes = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
        var compositeNodes = TypeCache.GetTypesDerivedFrom<CompositeNode>();
        var actionNodes = TypeCache.GetTypesDerivedFrom<ActionNode>();

        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
        };
        
        AddNodesToTree("Decorator Nodes", decoratorNodes, tree);
        AddNodesToTree("Composite Nodes", compositeNodes, tree);
        AddNodesToTree("Action Nodes", actionNodes, tree);

        return tree;
    }

    private void AddNodesToTree(string categoryName, TypeCache.TypeCollection col, List<SearchTreeEntry> tree)
    {
        tree.Add(new SearchTreeGroupEntry(new GUIContent($" {categoryName}"), 1));
        foreach (var nodeType in col)
        {
            tree.Add(new SearchTreeEntry(new GUIContent($" {nodeType.Name}"))
            {
                userData = nodeType, level = 2,
            });
        }
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePos = editorWindow.rootVisualElement.ChangeCoordinatesTo(
            editorWindow.rootVisualElement.parent,
            context.screenMousePosition - editorWindow.position.position);
        
        var localMousePos = graphView.contentContainer.WorldToLocal(worldMousePos);

        graphView.CreateNode(SearchTreeEntry.userData.ToString(), localMousePos);
        // graphView.CreateNode(Type.GetType(SearchTreeEntry.userData.ToString()), localMousePos);

        return true;
    }
}
#endif
