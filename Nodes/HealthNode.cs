using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthNode : Node
{
    private AI ai;
    private float threshold;

    public HealthNode(AI ai, float threshold) 
    {
        this.ai = ai;
        this.threshold = threshold;
    }

    public override State Evaluate() 
    {
        return ai.GetCurrentHealth() <= threshold ? State.SUCCESS : State.FAILURE;
    }
}
