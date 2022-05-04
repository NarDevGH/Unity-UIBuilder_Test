using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;

    public Node node;
    public Port input;
    public Port output;
    public NodeView(Node node) : base("Assets/BehaviourTreeProject/Editor/NodeView/NodeView.uxml")
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        //set node position on graph
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutpuPorts();
        SetupClasses();
    }

    private void SetupClasses()
    {
        if (node is RootNode)
        {
            AddToClassList("rootNode");
        }
        else if (node is ActionNode)
        {
            AddToClassList("actionNode");
        }
        else if (node is CompositeNode)
        {
            AddToClassList("compositeNode");
        }
        else if (node is DecoratorNode)
        {
            AddToClassList("decoratorNode");
        }
    }

    private void CreateInputPorts()
    {
        if (node is RootNode){
            
        }
        else if (node is ActionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input,
                                    Port.Capacity.Single,
                                    typeof(bool));// Dummy type
        }
        else if (node is CompositeNode){
            input = InstantiatePort(Orientation.Vertical, Direction.Input,
                                    Port.Capacity.Single,
                                    typeof(bool));
        }
        else if (node is DecoratorNode){
            input = InstantiatePort(Orientation.Vertical, Direction.Input,
                                    Port.Capacity.Single,
                                    typeof(bool));
        }

        if (input != null) 
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }
    private void CreateOutpuPorts()
    {
        if (node is RootNode)
        {
            output = InstantiatePort(Orientation.Vertical,
                                    Direction.Output,
                                    Port.Capacity.Single,
                                    typeof(bool));// Dummy type
        }
        else if (node is ActionNode)
        {
            
        }
        else if (node is CompositeNode)
        {
            output = InstantiatePort(Orientation.Vertical, 
                                    Direction.Output,
                                    Port.Capacity.Multi,
                                    typeof(bool));// Dummy type
        }
        else if (node is DecoratorNode)
        {
            output = InstantiatePort(Orientation.Vertical, 
                                    Direction.Output,
                                    Port.Capacity.Single,
                                    typeof(bool));
        }

        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }


    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        //Save position for Undo
        UnityEditor.Undo.RecordObject(node, "BehaviourTree(SetPosition)");

        node.position.x = newPos.xMin; 
        node.position.y = newPos.yMin;

        UnityEditor.EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();

        if (OnNodeSelected != null) 
        {
            OnNodeSelected.Invoke(this);
        }
    }

    public void SortChildren() 
    {
        // Only compositeNode have multiple childrens
        CompositeNode compositeNode = node as CompositeNode;
        if (compositeNode) 
        {
            compositeNode.children.Sort(SortByHorizontalPosition);
        }
    }

    public void UpdateState() 
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (Application.isPlaying) 
        {
            switch (node.state)
            {
                case Node.State.Running:
                    if (node.started){
                        AddToClassList("running");
                    }
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }

    private int SortByHorizontalPosition(Node left, Node right)
    {
        return left.position.x < right.position.x? -1 : 1;
    }
}