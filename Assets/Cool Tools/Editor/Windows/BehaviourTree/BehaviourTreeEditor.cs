#if UNITY_EDITOR
using System;
using CoolTools.BehaviourTree;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeEditor : EditorWindow
{
    private BehaviourTreeView treeView;
    private InspectorView inspectorView;

    [MenuItem("Window/BehaviourTreeEditor")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<BehaviourTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
        wnd.Show();
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviourTree)
        {
            OpenWindow();

            return true;
        }

        return false;
    }

    public static void OpenTreeAset(GameObject context, BehaviourTree tree)
    {
        if(AssetDatabase.CanOpenForEdit(tree))
            Selection.activeObject = context;
        
        OpenWindow();
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        var root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/Cool Tools/Editor/Windows/BehaviourTree/BehaviourTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
            "Assets/Cool Tools/Editor/Windows/BehaviourTree/BehaviourTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();

        treeView.OnNodeSelected = OnNodeSelectionChanged;
        treeView.SetEditorWindow(this);
        
        // GenerateMinimap();
        
        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }
    
    private void GenerateMinimap()
    {
        var miniMap = new MiniMap {anchored = true};
        miniMap.SetPosition(new Rect(10, 30, 200, 140));
        treeView.Add(miniMap);
    }

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                OnSelectionChange();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(obj), obj, null);
        }
    }

    private void OnSelectionChange()
    {
        var tree = Selection.activeObject as BehaviourTree;
        if (!tree)
        {
            if (Selection.activeGameObject)
            {
                var runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if (runner && runner.Tree != null)
                {
                    tree = runner.Tree;
                    tree.Blackboard = runner.Blackboard;
                }
            }
        }

        if (treeView == null) return;

        if (Application.isPlaying)
        {
            if (tree)
            {
                treeView.PopulateView(tree);
            }
        }
        else
        {
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                treeView.PopulateView(tree);
            }
        }
    }

    private void OnNodeSelectionChanged(NodeView nodeView)
    {
        inspectorView.UpdateSelection(nodeView);
    }

    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }
}
#endif