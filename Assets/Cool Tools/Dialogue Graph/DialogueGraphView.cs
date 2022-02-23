using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CoolTools.Graphs.Dialogue
{
    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new(150, 200);
        private NodeSearchWindow searchWindow;

        public DialogueGraphView(EditorWindow window)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
            
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GenerateEntryPoint());
            AddSearchWindow(window);
        }

        private void AddSearchWindow(EditorWindow window)
        {
            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Init(window, this);

            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        private DialogueNode GenerateEntryPoint()
        {
            var node = new DialogueNode()
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                DialogueText = "Entrypoint",
                EntryPoint = true,
            };

            var generatedPort = GeneratePort(node, Direction.Output);
            generatedPort.portName = "Next";
            node.outputContainer.Add(generatedPort);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(100, 200, 100, 150));
            return node;
        }

        public void CreateNode(Vector2 mousePos, string nodeText = "Text")
        {
            AddElement(CreateDialogueNode(mousePos, nodeText));
        }

        private Port GeneratePort(DialogueNode node, Direction portDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }

        public DialogueNode CreateDialogueNode(Vector2 pos = default, string nodeText = "")
        {
            var dialogueNode = new DialogueNode()
            {
                title = "Dialogue Node",
                DialogueText = nodeText,
                GUID = Guid.NewGuid().ToString()
            };

            dialogueNode.AddToClassList("dialogueNode");

            // Add Styles
            dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            // Add Input Port
            var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            dialogueNode.inputContainer.Add(inputPort);

            // Add Choice Button
            var button = new Button(() => AddChoicePort(dialogueNode)) {text = "New Choice"};
            dialogueNode.titleContainer.Add(button);

            // Add text field
            var textField = new TextField(string.Empty);
            textField.RegisterValueChangedCallback(evt => { dialogueNode.DialogueText = evt.newValue; });
            textField.SetValueWithoutNotify(nodeText);
            dialogueNode.mainContainer.Add(textField);

            // Finishing Up refresh
            dialogueNode.SetPosition(new Rect(pos, DefaultNodeSize));
            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();

            return dialogueNode;
        }

        public void AddChoicePort(DialogueNode dialogueNode, string overridenPortName = "")
        {
            var generatedPort = GeneratePort(dialogueNode, Direction.Output);

            var oldLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.contentContainer.Remove(oldLabel);

            var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
            var outputPortName = $"Choice {outputPortCount + 1}";

            var choicePortName = string.IsNullOrEmpty(overridenPortName) ? outputPortName : overridenPortName;

            var textField = new TextField()
            {
                name = string.Empty,
                value = choicePortName,
            };

            textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
            generatedPort.contentContainer.Add(new Label("->"));
            generatedPort.contentContainer.Add(textField);

            var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);


            generatedPort.portName = choicePortName;
            dialogueNode.outputContainer.Add(generatedPort);
            dialogueNode.RefreshPorts();
            dialogueNode.RefreshExpandedState();
        }

        private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
        {
            var targetEdge = edges.ToList().Where(x =>
                x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            dialogueNode.outputContainer.Remove(generatedPort);
            dialogueNode.RefreshPorts();
            dialogueNode.RefreshExpandedState();

        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) =>
            ports.Where(p => startPort != p && startPort.node != p.node).ToList();

    }
}