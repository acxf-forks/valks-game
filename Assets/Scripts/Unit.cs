using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The player is a temporary class to help with debugging.
public class Unit : MonoBehaviour
{
    public Transform planet;
    private Planet planetScript;

    private float planetRadius;

    private static int unitCount = 0;

    private Game game;
    private List<GameObject> units;

    private Vector3 gravityUp;

    private Vector3 startPos;
    private bool getStartPos = true;

    [Header("Movement")]
    public float curSpeed = 0;

    public float maxSpeed;
    private float accSpeed;

    public bool groupLeader; // Is this unit a leader of a group?
    public UnitGroup group; // The group this unit belongs to

    private Vector3 target = new Vector3(1, 0, 0);

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Units");
        game = GameObject.Find("Manager").GetComponent<Game>();
    }

    private void Start()
    {
        planetScript = planet.GetComponent<Planet>();
        planetRadius = planetScript.radius;

        transform.position = new Vector3(0, planetRadius + 1, 0);

        gameObject.name = $"({++unitCount}) Unit";

        units = game.units;

        maxSpeed = 0.1f;
        accSpeed = maxSpeed / 100;
    }

    public void MoveToTarget(Vector3 target) 
    {
        this.target = target;

        if (getStartPos) 
        {
            getStartPos = false;
            startPos = transform.position;
        }

        // Move towards the target
        var distanceToTarget = Vector3.Distance(transform.position, target);
        if (distanceToTarget > 0)
        {
            if (distanceToTarget > 3)
            {
                curSpeed += accSpeed;
                curSpeed = Mathf.Min(curSpeed, maxSpeed);
            }
            else 
            {
                curSpeed -= accSpeed;
                curSpeed = Mathf.Max(curSpeed, 0);
            }

            //Separation();

            // Moving towards target
            transform.position = Vector3.RotateTowards(transform.position, target, curSpeed * Time.deltaTime, 0);
        }
        else 
        {
            // Reached target
            getStartPos = true;
        }
    }

    public void AlignToPlanetSurface() 
    {
        // Rotate towards the target on the y axis whilst maintaining a standing rotation on the surface of the planet
        gravityUp = (transform.position - planet.position).normalized;
        var forward = Vector3.ProjectOnPlane(target - transform.position, gravityUp);
        if (forward != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(forward, gravityUp);

        // Snap back to planets surface
        this.transform.position = gravityUp * (planetRadius + 1);
    }

    /*
     * Separate agents from each other.
     */
    public void Separation() 
    {
        gravityUp = (transform.position - planet.position).normalized;

        var groupSeparationFactor = 0.05f;

        foreach (var agent in units)
        {
            Unit unit = agent.GetComponent<Unit>();
            if (unit.groupLeader)
                continue;

            var agentA = this.transform.position;
            var agentB = agent.transform.position;

            var maxDist = 1.1f;
            if (unit.group != null) 
            {
                maxDist += (unit.group.units.Count * groupSeparationFactor);
            }

            var curDist = Vector3.Distance(agentA, agentB);

            if (curDist < maxDist)
            {
                var dir = (agentA - agentB).normalized;

                // If their positions are the same add a small offset
                if (agentA == agentB)
                    dir += new Vector3(0, 0, 1);

                // Separate the agents from each other
                var separationForce = maxDist - curDist;
                agent.transform.position -= dir * separationForce * Time.deltaTime;
            }
        }
    }

    // Another challenge for another day!
    /*private void AStar(Vector3 start, Vector3 goal) 
    {
        // Center vertices of all the planets polygons
        Vector3[] centerVertices = planetScript.centerVertices;

        // Priority queue
        List<Vector3> openSet = new List<Vector3>();
        openSet.Add(start);

        // Calculate the shortest path to the next node
        Vector3 nextNode = Vector3.zero;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < centerVertices.Length; i++) 
        {
            float curDist = Vector3.Distance(start, centerVertices[i]);
            if (curDist < minDist) 
            {
                minDist = curDist;
                nextNode = centerVertices[i];
            }
        }

        openSet.Add(nextNode);
    }*/
}
