using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

    private Editor _editor;

    public InspectorView()
    {
    }

    internal void UpdateSelection(NodeView nodeView)
    {
        // clear previus selections
        Clear();

        // editor most be destroy every time a new one is created
        UnityEngine.Object.DestroyImmediate(_editor);

        _editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(() => {
            // if visualElement exist(in case it get deleted)
            if (_editor.target) 
            {
                _editor.OnInspectorGUI(); 
            }
        });
        Add(container);
    }
}
