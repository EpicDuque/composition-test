using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CoolTools.Graphs.Dialogue
{
    public class DialogueGraph : EditorWindow
    {
        private DialogueGraphView graphView;
        private DialogueContainer targetContainerFile;

        [MenuItem("Graph/Dialogue Graph")]
        public static void OpenDialogueGraphWindow()
        {
            var window = GetWindow<DialogueGraph>();

            window.titleContent = new GUIContent("Dialogue Graph");
            window.Show();
        }

        private void OnEnable()
        {
            ConstructGraphView();
            
            GenerateToolbar();

            GenerateMinimap();
        }

        private void GenerateMinimap()
        {
            var miniMap = new MiniMap {anchored = true};
            miniMap.SetPosition(new Rect(10, 30, 200, 140));
            graphView.Add(miniMap);
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
            
            SaveGraph();
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();

            var dialogueFile = new ObjectField
            {
                objectType = typeof(DialogueContainer),
                allowSceneObjects = false
            };
            dialogueFile.RegisterValueChangedCallback(evt =>
            {
                targetContainerFile = evt.newValue as DialogueContainer;
                LoadGraph();
            });
            toolbar.Add(dialogueFile);

            toolbar.Add(new Button(SaveGraph) {text = "Save"});

            rootVisualElement.Add(toolbar);
        }

        public void SaveGraph()
        {
            if (targetContainerFile == null)
            {
                EditorUtility.DisplayDialog("No File Specified", "Please enter a valid file.", "Ok");
                return;
            }

            var saveUtil = GraphSaveUtility.GetInstance(graphView);
            saveUtil.SaveGraph(targetContainerFile.name);
        }

        public void LoadGraph()
        {
            if (targetContainerFile == null)
            {
                return;
            }

            var saveUtil = GraphSaveUtility.GetInstance(graphView);
            saveUtil.LoadGraph(targetContainerFile.name);
        }

        private void ConstructGraphView()
        {
            graphView = new DialogueGraphView(this)
            {
                name = "Dialogue Graph",
            };

            graphView.StretchToParentSize();

            rootVisualElement.Add(graphView);
        }
    }
}
