using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;

    public List<Node> nodes = new List<Node>();

    public Node.State Update() 
    {
        if (rootNode.state == Node.State.Running) 
        {
            treeState = rootNode.Update();
        }

        return treeState;
    }

#if UNITY_EDITOR

    public Node CreateNode(System.Type type) 
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid  = GUID.Generate().ToString();

        Undo.RecordObject(this, "BehaviourTree(CreateNode)");

        nodes.Add(node);

        // Create Node only in editor (Cant create on runtime, Not allowed)
        if (!Application.isPlaying) 
        {
            AssetDatabase.AddObjectToAsset(node, this);
        }
        Undo.RegisterCompleteObjectUndo(node, "BehaviourTree(CreateNode)");
        AssetDatabase.SaveAssets();

        return node;
    }

    public void DeleteNode(Node node) 
    {
        Undo.RecordObject(this, "BehaviourTree(CreateNode)");

        nodes.Remove(node);

        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node); // Better Solution for both deleting the SO and UndoRedo
        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child) 
    {
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "BehaviourTree(AddChild)");
            rootNode.child = child;
            EditorUtility.SetDirty(rootNode);
        }

        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode)
        {
            Undo.RecordObject(decoratorNode, "BehaviourTree(AddChild)");
            decoratorNode.child = child;
            EditorUtility.SetDirty(decoratorNode);
        }

        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            Undo.RecordObject(compositeNode, "BehaviourTree(AddChild)");
            compositeNode.children.Add(child);
            EditorUtility.SetDirty(compositeNode);
        }
    }
    public void RemoveChild(Node parent, Node child) 
    {
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "BehaviourTree(RemoveChild)");
            rootNode.child = null;
            EditorUtility.SetDirty(rootNode);
        }

        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode)
        {
            Undo.RecordObject(decoratorNode, "BehaviourTree(RemoveChild)");
            decoratorNode.child = null;
            EditorUtility.SetDirty(decoratorNode);
        }

        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            Undo.RecordObject(compositeNode, "BehaviourTree(RemoveChild)");
            compositeNode.children.Remove(child);
            EditorUtility.SetDirty(compositeNode);
        }
    }
    public List<Node> GetChildren(Node parent) 
    {
        List<Node> children = new List<Node>();

        RootNode rootNode = parent as RootNode;
        if (rootNode && rootNode.child != null)
        {
            children.Add(rootNode.child);
        }

        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode && decoratorNode.child != null)
        {
            children.Add(decoratorNode.child);
        }

        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode)
        {
            return compositeNode.children;
        }

        return children;
    }

#endif

    // Traverse tree executing Action visiter on each node.
    public void Traverse(Node node, System.Action<Node> visiter) 
    {
        if (node) 
        {
            visiter.Invoke(node);
            var children = GetChildren(node);
            children.ForEach((node) => Traverse(node, visiter));
        }

    }

    public BehaviourTree Clone() 
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        tree.nodes = new List<Node>();
        Traverse(tree.rootNode, (node) =>{
            tree.nodes.Add(node);
        });
        return tree;
    }
}
