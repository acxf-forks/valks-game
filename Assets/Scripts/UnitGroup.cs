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

    private static int groupCount = 0;
    private Transform groupOrigin;

    public UnitGroupTask unitGroupTask;
    public Vector3 target;

    public float distanceBetweenAgents = 1.1f;

    public UnitGroup(List<GameObject> units, Transform planet) 
    {
        this.units = units;
        this.planet = planet;
        planetRadius = planet.GetComponent<Planet>().radius;

        // Initialize group origin, all units will align with respect to this origin
        groupOrigin = new GameObject().transform;
        groupOrigin.transform.position = units[0].transform.position;
        groupCount++;
        groupOrigin.name = $"({groupCount}) Group";

        // Assign all units to group
        foreach (var unit in units)
            unit.GetComponent<Unit>().group = this;

        unitGroupTask = UnitGroupTask.Idle;
    }

    public void Update() 
    {
        if (unitGroupTask == UnitGroupTask.MoveToTarget) 
        {
            // Align group origin to planets surface
            PlanetUtils.AlignToPlanetSurface(groupOrigin, planet, target);

            AlignWithGroupOrigin();

            var speed = 10f;
            groupOrigin.position = Vector3.RotateTowards(groupOrigin.position, target, (speed / planetRadius) * Time.deltaTime, 1);

            if (Vector3.Distance(groupOrigin.position, target) < 1) 
            {
                unitGroupTask = UnitGroupTask.Idle;
            }
        } 
    }

    public void RemoveUnit(Unit unit)
    {
        if (!units.Remove(unit.gameObject)) // Remove unit from this group
            return;

        if (RemoveGroupIfMemberCountLow()) // Remove this group if member count is too low
            return;

        unit.Idle(); // Make sure the unit stops trying to move to their target
        unit.group = null;
    }

    public bool IsSelected() =>
        units.Any(unit => unit.GetComponent<Unit>().selected);

    public bool RemoveGroupIfMemberCountLow()
    {
        if (GetMemberCount() > 1)
            return false;

        groupCount--;
        Object.Destroy(groupOrigin.gameObject);
        Game.groups.Remove(this);
        return true;
    }

    public void MoveToTarget(Vector3 target) 
    {
        this.target = target;
        unitGroupTask = UnitGroupTask.MoveToTarget;
    }
    
    public void AlignWithGroupOrigin()
    {
        Debug.DrawRay(groupOrigin.position, groupOrigin.forward * 10f, Color.green);
        LineRowFormation();
    }

    private void LineRowFormation() 
    {
        var horzDist = 0f;
        var vertDist = 0f;

        for (int i = 0; i < units.Count; i++)
        {
            // Swap between left and right directions
            Vector3 direction;
            if (i % 2 == 0)
            {
                direction = groupOrigin.right * -1;
            }
            else
            {
                direction = groupOrigin.right;
                horzDist += distanceBetweenAgents;
            }

            // Start a new row behind
            if (i % 10 == 0)
            {
                horzDist = 0f;
                vertDist += distanceBetweenAgents;
            }

            // Calculate positions at end of each blue line
            var pos = groupOrigin.position + (direction * horzDist) + (groupOrigin.forward * -vertDist);

            //var arcLength = Vector3.Angle(groupOrigin.position, pos) * Mathf.Deg2Rad * (planetRadius + 1);

            // Snap back to planets surface
            var newGravityUp = (pos - planet.position).normalized * (planetRadius + 1);
            pos = newGravityUp;

            // Slowly move towards these positions
            units[i].GetComponent<Unit>().MoveToTarget(pos);
            //Debug.DrawLine(groupOrigin.position, pos, Color.blue);
        }
    }

    public int GetMemberCount() => units.Count;
}
