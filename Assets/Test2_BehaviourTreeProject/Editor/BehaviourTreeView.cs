using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Linq;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;

    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
 
    BehaviourTree _tree;

    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/BehaviourTreeProject/Editor/BehavoiouTreeEditor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        // Refresh Editor
        PopulateView(_tree);
        AssetDatabase.SaveAssets();
    }

    NodeView FindNodeView(Node node) 
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    //Draw graph elements
    internal void PopulateView(BehaviourTree tree)
    {
        this._tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (_tree.rootNode == null) 
        {
            _tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        // Creates the nodes view
        tree.nodes.ForEach(node => CreateNodeView(node));

        // Creates node edges
        tree.nodes.ForEach(node => CreateEdges(tree, node));
    }

    private void CreateEdges(BehaviourTree tree, Node node)
    {
        var children = tree.GetChildren(node);
        children.ForEach(child =>
        {
            NodeView parentNodeView = FindNodeView(node);
            NodeView childNodeView = FindNodeView(child);

            Edge edge = parentNodeView.output.ConnectTo(childNodeView.input);
            AddElement(edge);
        });
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endport => endport.direction != startPort.direction &&
                                               endport.node != startPort.node).ToList();
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        // On Element remove from the graph
        if (graphViewChange.elementsToRemove != null) 
        {
            graphViewChange.elementsToRemove.ForEach(elem => {

                // If node, remove child node from scriptableObject tree. 
                NodeView nodeView = elem as NodeView;
                if (nodeView != null) 
                {
                    _tree.DeleteNode(nodeView.node);
                }

                // If edge, remove child node from scriptableObject parent node.
                Edge edge = elem as Edge;
                if (edge != null)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    _tree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        // On Edge created (Link between Nodes).
        if (graphViewChange.edgesToCreate != null) 
        {
            // Update children of scriptableObject node.
            graphViewChange.edgesToCreate.ForEach(edge => {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                _tree.AddChild(parentView.node, childView.node);
            });
        }

        if(graphViewChange.movedElements != null) 
        {
            nodes.ForEach((n) => {
                NodeView nView = n as NodeView;
                nView.SortChildren();
            });
        }

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        {
            //returns a list of all the types that inherit from ActionNode 
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();

            foreach (var type in types) 
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            //returns a list of all the types that inherit from CompositeNode 
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();

            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            //returns a list of all the types that inherit from DecoratorNode 
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();

            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        } 
    }

    private void CreateNode(System.Type type)
    {
        Node node = _tree.CreateNode(type);
        CreateNodeView(node);
    }

    void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);

    }

    public void UpdateNodeStates() 
    {
        nodes.ForEach(n => {
            NodeView nView = n as NodeView;
            nView.UpdateState();
        });
    }
}
