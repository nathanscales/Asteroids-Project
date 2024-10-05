using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Node
{
    protected List<Node> nodes = new List<Node>();

    public Selector(List<Node> nodes) 
    {
        this.nodes = nodes;
    }

    public override State Evaluate()
    {
        foreach (var node in nodes) 
        {
            switch (node.Evaluate()) 
            {
                case State.RUNNING:
                    _nodeState = State.RUNNING;
                    return _nodeState;
                case State.SUCCESS:
                    _nodeState = State.SUCCESS;
                    return _nodeState;
                case State.FAILURE:
                    break;
                default:
                    break;
            }
        }
        _nodeState = State.FAILURE;
        return _nodeState;
    }
}
