using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    protected List<Node> nodes = new List<Node>();

    public Sequence(List<Node> nodes) 
    {
        this.nodes = nodes;
    }

    public override State Evaluate()
    {
        bool nodeRunning = false;

        foreach (var node in nodes) 
        {
            switch (node.Evaluate()) 
            {
                case State.RUNNING:
                    nodeRunning = true;
                    break;
                case State.SUCCESS:
                    break;
                case State.FAILURE:
                    _nodeState = State.FAILURE;
                    return _nodeState;
                default:
                    break;
            }
        }
        _nodeState = nodeRunning ? State.RUNNING : State.SUCCESS;
        return _nodeState;
    }
}
