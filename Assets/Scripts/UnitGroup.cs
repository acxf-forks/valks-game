using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitGroup
{
    public List<GameObject> units;
    public Transform planet;
    private float planetRadius;
    private Transform leader;

    public UnitGroup(List<GameObject> units, Transform planet) 
    {
        this.units = units;
        this.planet = planet;
        planetRadius = planet.GetComponent<Planet>().radius;

        // Elect Leader for Group
        leader = units[0].transform;
        leader.GetComponent<Renderer>().material.color = Color.blue;
        leader.GetComponent<Unit>().groupLeader = true;

        // Assign all units to group
        foreach (var unit in units)
            unit.GetComponent<Unit>().group = this;
    }

    public void MoveToTarget(Vector3 target) 
    {
        AlignWithLeader();
        leader.GetComponent<Unit>().MoveToTarget(target);
    }

    private void AlignWithLeader()
    {   
        // Align with Leader
        foreach (var agentGo in units)
        {
            var agentPos = agentGo.transform.position;
            var leaderPos = leader.position;

            var curDistFromLeader = Vector3.Distance(agentPos, leader.position);
            var maxDist = 1.1f;

            if (agentGo == leader.gameObject)
                continue;

            // Too far away from leader, move closer.
            var dir = (agentPos - leaderPos).normalized;

            // If their positions are the same add a small offset
            if (agentPos == leaderPos)
                dir += new Vector3(1, 0, 0);

            // Separate the agents from each other
            var separationForce = maxDist - curDistFromLeader;
            agentGo.transform.position += dir * separationForce * Time.deltaTime;
        }
    }
}
