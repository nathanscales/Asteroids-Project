using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsCoverAvailableNode : Node
{
    private Player player;
    private AI ai;

    public IsCoverAvailableNode(Player player, AI ai)
    {
        this.player = player;
        this.ai = ai;
    }

    public override State Evaluate()
    {
        Vector3 bestSpot = FindBestCoverSpot();
        ai.SetBestCoverSpot(bestSpot);
        return bestSpot != Vector3.zero ? State.SUCCESS : State.FAILURE;
    }

    private Vector3 FindBestCoverSpot()
    {
        GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
        SpriteRenderer sr;

        Vector3 coverSpot;
        Vector3 bestSpot = Vector3.zero;
        Vector3 dir;

        float coverSpotDistance;
        float bestSpotDistance = 0;

        foreach (GameObject a in asteroids)
        {
            sr = a.GetComponent<SpriteRenderer>();

            if (sr.isVisible)
            {

                dir = (a.transform.position - player.transform.position).normalized;

                coverSpot = a.transform.position + dir*1.25f;
                coverSpotDistance = Vector3.Distance(ai.transform.position, coverSpot);

                if(bestSpot == Vector3.zero || (coverSpotDistance < bestSpotDistance))
                {
                    bestSpot = coverSpot;
                    bestSpotDistance = coverSpotDistance;
                }                
            }
        }

        return bestSpot;
    }
}