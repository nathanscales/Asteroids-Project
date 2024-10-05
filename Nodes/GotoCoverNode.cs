using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GotoCoverNode : Node
{
    private AI ai;

    public GotoCoverNode(AI ai)
    {
        this.ai = ai;
    }

    public override State Evaluate()
    {
        Vector3 coverSpot = ai.GetBestCoverSpot();

        if(coverSpot == null) 
        {
            return State.FAILURE;
        }

        float dis = Vector3.Distance(coverSpot, ai.transform.position);

        if(dis > 0.2f) 
        {
            ai.Goto(coverSpot);
            return State.RUNNING;
        }
        else
        {
            return State.SUCCESS;
        }
    }
}
