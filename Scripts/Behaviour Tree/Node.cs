using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{ 
    protected State _nodeState;
    public State nodeState { get { return _nodeState; } }

    public abstract State Evaluate();

}

public enum State {
    RUNNING, SUCCESS, FAILURE,
}
