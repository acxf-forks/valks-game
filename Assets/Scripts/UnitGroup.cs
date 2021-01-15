using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UnitGroupTask 
{
    Idle,
    MoveToTarget
}

public class UnitGroup
{
    public List<GameObject> units;
    public Transform planet;
    private float planetRadius;
    public Transform leader;

    public UnitGroupTask unitGroupTask;
    public Vector3 target;

    public float distanceBetweenAgents = 1.1f;

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

        unitGroupTask = UnitGroupTask.Idle;
    }

    public void Update() 
    {
        if (unitGroupTask == UnitGroupTask.MoveToTarget) 
        {
            AlignWithLeader();

            leader.GetComponent<Unit>().MoveToTarget(target);
            if (Vector3.Distance(leader.position, target) < 1) 
            {
                unitGroupTask = UnitGroupTask.Idle;
            }
        } 
    }

    public void RemoveUnit(Unit unit)
    {
        if (!units.Remove(unit.gameObject))
            return;

        if (RemoveGroupIfMemberCountLow())
            return;

        if (unit.groupLeader)
            ReassignLeader();

        unit.Idle(); // Make sure the unit stops trying to move to their target
        unit.groupLeader = default;
        unit.group = null;
    }

    public void ReassignLeader()
    {
        leader = units[0].transform;
        leader.GetComponent<Unit>().groupLeader = true;
    }

    public bool IsSelected() =>
        units.Any(unit => unit.GetComponent<Unit>().selected);

    public bool RemoveGroupIfMemberCountLow()
    {
        if (GetMemberCount() > 1)
            return false;

        Game.groups.Remove(this);
        return true;
    }

    public void MoveToTarget(Vector3 target) 
    {
        this.target = target;
        unitGroupTask = UnitGroupTask.MoveToTarget;
    }
    
    public void AlignWithLeader()
    {
        Debug.DrawRay(leader.position, leader.transform.forward * 10f, Color.green);
        LineRowFormation();
    }

    private void LineRowFormation() 
    {
        var horzDist = 0f;
        var vertDist = 0f;

        for (int i = 0; i < units.Count; i++)
        {
            if (i == 0) // Ignore leader
                continue;

            // Swap between left and right directions
            Vector3 direction;
            if (i % 2 == 0)
            {
                direction = leader.right * -1;
            }
            else
            {
                direction = leader.right;
                horzDist += distanceBetweenAgents;
            }

            // Start a new row behind
            if (i % 10 == 0)
            {
                horzDist = 0f;
                vertDist += distanceBetweenAgents;
            }

            // Calculate positions at end of each blue line
            var pos = leader.transform.position + (direction * horzDist) + (leader.forward * -vertDist);

            //var arcLength = Vector3.Angle(leader.transform.position, pos) * Mathf.Deg2Rad * (planetRadius + 1);

            // Snap back to planets surface
            var newGravityUp = (pos - planet.position).normalized * (planetRadius + 1);
            pos = newGravityUp;

            // Slowly move towards these positions
            units[i].GetComponent<Unit>().MoveToTarget(pos);
            //Debug.DrawLine(leader.position, pos, Color.blue);
        }
    }

    public int GetMemberCount() => units.Count;
}
