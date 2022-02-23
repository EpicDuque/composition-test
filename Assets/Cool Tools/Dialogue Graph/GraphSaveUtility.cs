using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CoolTools.Graphs.Dialogue
{
    public class GraphSaveUtility
    {
        private DialogueGraphView _targetGraphView;
        private DialogueContainer container;

        private List<Edge> Edges => _targetGraphView.edges.ToList();
        private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

        public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
        {
            return new()
            {
                _targetGraphView = targetGraphView,
            };
        }

        public void SaveGraph(string filename)
        {
            if (!Edges.Any()) return;

            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();

            foreach (var edge in connectedPorts)
            {
                var outputNode = edge.output.node as DialogueNode;
                var inputNode = edge.input.node as DialogueNode;

                dialogueContainer.Nodelinks.Add(new NodeLinkData
                {
                    BaseNodeGUID = outputNode.GUID,
                    PortName = edge.output.portName,
                    TargetNodeGuid = inputNode.GUID,
                });
            }

            foreach (var node in Nodes.Where(n => !n.EntryPoint))
            {
                dialogueContainer.DialoguesNodeData.Add(new DialogueNodeData
                {
                    GUID = node.GUID,
                    DialogueText = node.DialogueText,
                    Position = node.GetPosition().position
                });
            }

            var path = $"Assets/Resources/{filename}.asset";
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }

            AssetDatabase.CreateAsset(dialogueContainer, path);
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string filename)
        {
            container = Resources.Load<DialogueContainer>(filename);

            if (container == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Dialogue Graph file does not exist!", "Ok");
                return;
            }

            ClearGraph();
            ClearNodes();
            ConnectNodes();
        }

        private void ConnectNodes()
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                var connections = container.Nodelinks
                    .Where(x => x.BaseNodeGUID == Nodes[i].GUID).ToList();

                for (int j = 0; j < connections.Count; j++)
                {
                    var targetNodeGUID = connections[j].TargetNodeGuid;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGUID);

                    LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);

                    targetNode.SetPosition(new Rect(
                        container.DialoguesNodeData.First(x => x.GUID == targetNodeGUID).Position,
                        _targetGraphView.DefaultNodeSize
                    ));
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };

            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);

            _targetGraphView.Add(tempEdge);
        }

        private void ClearNodes()
        {
            foreach (var nodeData in container.DialoguesNodeData)
            {
                var tempNode = _targetGraphView.CreateDialogueNode(nodeText: nodeData.DialogueText);
                tempNode.GUID = nodeData.GUID;
                _targetGraphView.AddElement(tempNode);

                var nodePorts = container.Nodelinks
                    .Where(x => x.BaseNodeGUID == nodeData.GUID).ToList();

                nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
            }
        }

        private void ClearGraph()
        {
            Nodes.Find(x => x.EntryPoint).GUID = container.Nodelinks[0].BaseNodeGUID;

            foreach (var node in Nodes)
            {
                if (node.EntryPoint) continue;

                Edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => _targetGraphView.RemoveElement(edge));

                _targetGraphView.RemoveElement(node);
            }


        }
    }
}