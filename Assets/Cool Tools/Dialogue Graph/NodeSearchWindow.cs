using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace CoolTools.Graphs.Dialogue
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DialogueGraphView graphView;
        private EditorWindow editorWindow;

        public void Init(EditorWindow window, DialogueGraphView view)
        {
            editorWindow = window;
            graphView = view;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
                new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
                new(new GUIContent("Dialogue Node"))
                {
                    userData = new DialogueNode(), level = 2,
                },
            };

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var worldMousePos = editorWindow.rootVisualElement.ChangeCoordinatesTo(
                editorWindow.rootVisualElement.parent,
                context.screenMousePosition - editorWindow.position.position);

            var localMousePos = graphView.contentContainer.WorldToLocal(worldMousePos);

            switch (SearchTreeEntry.userData)
            {
                case DialogueNode:
                    graphView.CreateNode(localMousePos);
                    return true;
                    break;

                default:
                    return false;
            }
        }
    }
}
