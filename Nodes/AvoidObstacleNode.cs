using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidObstacleNode : Node
{
    private AI ai;
    private GameObject[] boundaries;

    public AvoidObstacleNode(AI ai, GameObject[] boundaries)
    {
        this.ai = ai;
        this.boundaries = boundaries;
    }

    public override State Evaluate()
    {
        Vector3 dir = new Vector3(0, 0, 0);
        bool avoiding = false;

        foreach(GameObject b in boundaries) 
        {
            if (Vector3.Distance(ai.transform.position, b.transform.position) < 1.0f) {
                Debug.Log("avoiding");
                avoiding = true;
                dir += (ai.transform.position - b.transform.position) * 2.0f;
            }
        }

        if(avoiding) {
            ai.Goto(ai.transform.position + dir);
        }
        return State.FAILURE;
    }
}
