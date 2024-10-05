using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsCoveredNode : Node
{
    private Player player;
    private Transform origin;

    public IsCoveredNode(Player player, Transform origin) 
    {
        this.player = player;
        this.origin = origin;
    }

    public override State Evaluate()
    {
        RaycastHit hit;
        if(Physics.Raycast(origin.position, player.transform.position - origin.position, out hit))
        {
            if(hit.collider.transform != player.transform)
            {
                return State.SUCCESS;
            }
        }
        return State.FAILURE;
    }
}
