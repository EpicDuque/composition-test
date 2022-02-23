using UnityEngine.UIElements;
using UnityEditor;

#if UNITY_EDITOR
public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
    {
    }

    private Editor editor;

    public InspectorView()
    {

    }

    public void UpdateSelection(NodeView nodeView)
    {
        Clear();

        if (nodeView == null) return;
        UnityEngine.Object.DestroyImmediate(editor);

        editor = Editor.CreateEditor(nodeView.node);

        var container = new IMGUIContainer(() =>
        {
            if (!editor.target) return;

            editor.OnInspectorGUI();
        });

        Add(container);
    }
}

#endif