#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using CoolTools.BehaviourTree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine;
using Node = CoolTools.BehaviourTree.Node;

public class BehaviourTreeView : GraphView
{
    private BehaviourTree tree;
    public Action<NodeView> OnNodeSelected;
    private TreeNodeSearchWindow searchWindow;

    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits>
    {
    }

    public BehaviourTreeView()
    {
        InitializeGraph();
    }

    private void InitializeGraph()
    {
        Insert(0, new GridBackground());

        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        // this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet =
            AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/Cool Tools/Editor/Windows/BehaviourTree/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    public void SetEditorWindow(EditorWindow window)
    {
        AddSearchWindow(window);
    }
    
    private void AddSearchWindow(EditorWindow window)
    {
        searchWindow = ScriptableObject.CreateInstance<TreeNodeSearchWindow>();
        searchWindow.Init(window, this);

        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
    }

    ~BehaviourTreeView()
    {
        Undo.undoRedoPerformed -= OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    private NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    internal void PopulateView(BehaviourTree tree)
    {
        if (tree == null) return;

        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (this.tree.Nodes.All(n => n is not RootNode))
        {
            this.tree.RootNode = tree.CreateNode(typeof(RootNode)) as RootNode;

            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }
        else
        {
            this.tree.RootNode = this.tree.Nodes.First(n => n is RootNode) as RootNode;
        }

        // Creates the node view
        tree.Nodes?.ForEach(n => CreateNodeView(n));

        // Create edges
        tree.Nodes.ForEach(n =>
        {
            var children = tree.GetChildren(n);

            children.ForEach(c =>
            {
                var parentView = FindNodeView(n);
                var childView = FindNodeView(c);

                var edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });

    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {
        if (change.elementsToRemove != null)
        {
            change.elementsToRemove.ForEach(e =>
            {
                if (e is NodeView nodeView)
                {
                    tree.DeleteNode(nodeView.node);
                }

                if (e is Edge edge)
                {
                    var parentView = edge.output.node as NodeView;
                    var childView = edge.input.node as NodeView;
                    tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        if (change.edgesToCreate != null)
        {
            change.edgesToCreate.ForEach(e =>
            {
                var parentView = e.output.node as NodeView;
                var childView = e.input.node as NodeView;

                tree.AddChild(parentView.node, childView.node);
            });
        }

        if (change.movedElements != null)
        {
            nodes.ForEach(n =>
            {
                var view = n as NodeView;
                view.SortChildren();
            });
        }

        return change;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(ep =>
            ep.direction != startPort.direction && ep.node != startPort.node).ToList();
    }

    public Vector2 GetMousePosFromMouseEvent(IMouseEvent evt)
    {
        Vector3 screenMousePosition = evt.localMousePosition;
        Vector2 mousePos = screenMousePosition - contentViewContainer.transform.position;

        mousePos *= 1 / contentViewContainer.transform.scale.x;

        return mousePos;
    }

    public void CreateNode(string className, Vector2 mousePos = default)
    {
        var node = tree.CreateNode(className);
        node.position = mousePos;
        
        CreateNodeView(node);
    }
    
    public void CreateNode(Type type, Vector2 mousePos = default)
    {
        var node = tree.CreateNode(type);
        node.position = mousePos;

        CreateNodeView(node);
    }

    private void CreateNodeView(Node node)
    {
        var nodeView = new NodeView(node) {OnNodeSelected = OnNodeSelected};
        AddElement(nodeView);
    }

    public void UpdateNodeStates()
    {
        nodes.ForEach(n =>
        {
            var view = n as NodeView;
            view.UpdateState();
        });
    }
}
#endif