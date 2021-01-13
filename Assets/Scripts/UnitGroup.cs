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
        //leader.GetComponent<Unit>().MoveToTarget(target);
    }

    private void AlignWithLeader()
    {
        var leaderPos = leader.position;

        Debug.DrawRay(leader.position, leader.transform.forward * 10f, Color.green);

        // Calculate Unit Formation
        for (int i = 0; i < units.Count; i++)
        {
            if (i == 0) // Ignore leader
                continue;

            var distFromLeader = 5;
            var unitSpreadAngle = 180;

            
            var gravityUp = (units[i].transform.position - planet.position).normalized;

            // Learn how remaps work (https://www.fundza.com/rfm/osl/f_remap/index.html)
            // Calculate each individual blue line direction
            var leaderBackDir = Quaternion.Euler(0, (unitSpreadAngle / 2) * ((float)i).Remap(1, units.Count - 1, -1, 1), 0) * -leader.transform.forward;

            //var forward = Vector3.ProjectOnPlane(leaderBackDir, gravityUp);
            //var leaderBackDirAligned = Quaternion.LookRotation(forward, gravityUp);

            // Calculate positions at end of each blue line
            var pos = leader.transform.position + leaderBackDir * distFromLeader;

            // Slowly move towards these positions
            units[i].GetComponent<Unit>().MoveToTarget(pos);
            Debug.DrawLine(leader.position, pos, Color.blue);
        }

        // Align with Leader
        /*foreach (var agentGo in units)
        {
            var agentPos = agentGo.transform.position;

            var curDistFromLeader = Vector3.Distance(agentPos, leader.position);
            var maxDist = 1.1f;

            if (agentGo == leader.gameObject)
                continue;

            // Too far away from leader, move closer.
            var dir = (agentPos - leaderPos).normalized;

            // Prevent agents stacking up on a line
            if (dir.x == 0)
                dir += new Vector3(1, 0, 0) * 0.00001f;
            if (dir.z == 0)
                dir += new Vector3(0, 0, 1) * 0.00001f;

            // If their positions are the same add a small offset
            if (agentPos == leaderPos)
                dir += new Vector3(1, 0, 0) * Mathf.Epsilon;

            // Separate the agents from each other
            var separationForce = maxDist - curDistFromLeader;
            agentGo.transform.position += dir * separationForce * Time.deltaTime;
        }*/
    }
}
