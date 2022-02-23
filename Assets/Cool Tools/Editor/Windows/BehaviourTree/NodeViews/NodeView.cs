#if UNITY_EDITOR
using System;
using CoolTools.BehaviourTree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = CoolTools.BehaviourTree.Node;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Node node;
    public Port input;
    public Port output;
    public Action<NodeView> OnNodeSelected;
    private string descriptionText;

    public string DescriptionText
    {
        get => descriptionText;
        set
        {
            descriptionText = value;
            var description = contentContainer.Q<Label>("description");
            description.text = node.description;
        }
    }

    public NodeView(Node node) : base("Assets/Cool Tools/Editor/Windows/BehaviourTree/NodeViews/NodeView.uxml")
    {
        this.node = node;
        title = node.name.Replace("Node", "");
        viewDataKey = node.guid;
        
        capabilities &= ~Capabilities.Snappable;
        
        if(node is RootNode)
            capabilities &= ~Capabilities.Deletable;
        
        style.left = node.position.x;
        style.top = node.position.y;

        DescriptionText = node.description;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
    }

    private void SetupClasses()
    {
        if (node is ActionNode)
        {
            AddToClassList("action");
        } else if (node is CompositeNode)
        {
            AddToClassList("composite");
        } else if (node is DecoratorNode)
        {
            AddToClassList("decorator");
            
        } else if (node is RootNode)
        {
            AddToClassList("rootnode");
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        
        Undo.RecordObject(node, "BehaviourTree (Set Position)");

        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;

        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();
        
        UpdateDescription();
        OnNodeSelected?.Invoke(this);
    }

    public override void OnUnselected()
    {
        base.OnUnselected();
        
        UpdateDescription();
    }

    private void CreateInputPorts()
    {
        if (node is ActionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            
        } else if (node is CompositeNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            
        } else if (node is DecoratorNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
            
        } else if (node is RootNode)
        {
            
            
        }

        if (input != null)
        {
            input.portName = "";
            // input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }
    
    private void CreateOutputPorts()
    {
        if (node is ActionNode)
        {

        } else if (node is CompositeNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            
        } else if (node is DecoratorNode)
        {
            
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        } else if (node is RootNode)
        {
            
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (output != null)
        {
            output.portName = "";
            // output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    public void SortChildren()
    {
        var composite = node as CompositeNode;
        if (composite)
        {
            composite.Children.Sort(SortByHorizontalPosition);
        }
    }

    private int SortByHorizontalPosition(Node left, Node right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }
    
    public void UpdateState()
    {
        if (!Application.isPlaying)
        {
            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");
            RemoveFromClassList("idle");
            return;
        }
        
        switch (node.state)
        {
            case Node.State.Running:
                if (node.started)
                {
                    RemoveFromClassList("failure");
                    RemoveFromClassList("success");
                    RemoveFromClassList("idle");
                    AddToClassList("running");
                }
                break;
            case Node.State.Failure:
                RemoveFromClassList("running");
                RemoveFromClassList("success");
                RemoveFromClassList("idle");
                AddToClassList("failure");
                break;
            case Node.State.Success:
                RemoveFromClassList("running");
                RemoveFromClassList("failure");
                RemoveFromClassList("idle");
                AddToClassList("success");
                break;
            case Node.State.Idle:
                RemoveFromClassList("running");
                RemoveFromClassList("failure");
                RemoveFromClassList("success");
                AddToClassList("idle");
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void UpdateDescription()
    {
        DescriptionText = node.description;
    }
}
#endif