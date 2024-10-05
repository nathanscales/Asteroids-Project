using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearObstacleNode : Node
{
    private AI ai;

    public NearObstacleNode(AI ai) 
    {
        this.ai = ai;
    }

    public override State Evaluate()
    {
        GameObject[] boundaries = GameObject.FindGameObjectsWithTag("Boundary");

        foreach(GameObject b in boundaries) {
            if (Vector3.Distance(ai.transform.position, b.transform.position) < 2) 
            {
                return State.SUCCESS;
            }
        }

        return State.FAILURE;
    }
}
