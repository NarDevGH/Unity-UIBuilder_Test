using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BehavoiouTreeEditor : EditorWindow
{
    BehaviourTreeView treeView;
    InspectorView inspectorView;



    [MenuItem("Window/BehaviourTreeEditor")]
    public static void OpenWindow()
    {
        BehavoiouTreeEditor wnd = GetWindow<BehavoiouTreeEditor>();
        wnd.titleContent = new GUIContent("BehavoiouTreeEditor");
    }

    // open BehavoiouTreeEditor when double clicking BehaviourTree ScriptableObject
    [UnityEditor.Callbacks.OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line) 
    {
        if (Selection.activeObject is BehaviourTree) 
        {
            OpenWindow();
            return true;
        }
        return false;
    }


    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;


        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviourTreeProject/Editor/BehavoiouTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTreeProject/Editor/BehavoiouTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<InspectorView>();
        treeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChange;
        EditorApplication.playModeStateChanged += OnPlayModeStateChange;
    }

    
    private void OnPlayModeStateChange(PlayModeStateChange obj)
    {
        switch (obj)
        {
            //On EnteredEditMode switch to Editor Tree
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            //On EnteredPlayMode switch to Runtime(Clone) Tree
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
        }
    }

    private void OnSelectionChange()
    {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        if (!tree) 
        {
            if (Selection.activeGameObject) 
            {
                BehaviourTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviourTreeRunner>();
                if (runner) 
                {
                    tree = runner.tree;
                }
            }
        }

        if (Application.isPlaying) 
        {
            if (tree) 
            {
                treeView.PopulateView(tree);
            }
        }
        else{
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())) 
            {
                treeView.PopulateView(tree);
            }
        }
    }

    void OnNodeSelectionChanged(NodeView nodeView) 
    {
        inspectorView.UpdateSelection(nodeView);
    }

    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }
}