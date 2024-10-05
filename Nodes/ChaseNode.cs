using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChaseNode : Node
{
    private Player player;
    private AI ai;

    public ChaseNode(Player player, AI ai)
    {
        this.player = player;
        this.ai = ai;
    }

    public override State Evaluate()
    {
        float distance = Vector3.Distance(player.transform.position, ai.transform.position);
        if(distance > 0.2f) 
        {
            ai.Goto(player.transform.position);
            return State.RUNNING;
        }
        else
        {
            return State.SUCCESS;
        }
    }
}
