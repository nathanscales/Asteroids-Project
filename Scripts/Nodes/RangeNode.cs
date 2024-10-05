using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeNode : Node
{
    private float range;
    private Player player;
    private Transform ai;

    public RangeNode(float range, Player player, Transform ai)
    {
        this.range = range;
        this.player = player;
        this.ai = ai;
    }

    public override State Evaluate()
    {
        float distance = Vector3.Distance(player.transform.position, ai.position);
        return distance >= range ? State.SUCCESS : State.FAILURE; 
    }
}
