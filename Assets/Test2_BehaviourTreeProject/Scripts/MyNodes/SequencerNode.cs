using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// executes all the children nodes until one fail or all of them are succesfully executed
public class SequencerNode : CompositeNode
{
    int currentChildNode;
    protected override void OnStart()
    {
        currentChildNode = 0;
    }

    protected override void OnStop()
    {
        
    }

    protected override State OnUpdate()
    {
        var child = children[currentChildNode];
        switch (child.Update())
        {
            case State.Running:
                return State.Running;
            case State.Failure:
                return State.Failure;
            case State.Success:
                currentChildNode++;
                break;
        }


        return currentChildNode == children.Count ? State.Success : State.Running;
    }
}