using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitTask 
{
    Idle,
    MoveToTarget
}

// The player is a temporary class to help with debugging.
public class Unit : MonoBehaviour
{
    public Transform planet;
    private Planet planetScript;

    private float planetRadius;

    private static int unitCount = 0;

    private Vector3 gravityUp;

    public UnitTask unitTask;

    private float speed = 10f;

    public bool groupLeader; // Is this unit a leader of a group?
    public UnitGroup group; // The group this unit belongs to

    public bool selected;

    private Vector3 target;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Units");
    }

    private void Start()
    {
        planetScript = planet.GetComponent<Planet>();
        planetRadius = planetScript.radius;

        transform.position = new Vector3(0, planetRadius + 1, 0);

        gameObject.name = $"({++unitCount}) Unit";

        AddRandomForce(0.00001f);

        unitTask = UnitTask.Idle;
    }

    private void Update()
    {
        Separation();
        AlignToPlanetSurface();

        if (unitTask == UnitTask.MoveToTarget)
        {
            MovementLogic();
        }
    }

    public void MoveToTarget(Vector3 target) 
    {
        unitTask = UnitTask.MoveToTarget;
        this.target = target;
    }

    private void MovementLogic()
    {
        // Move towards the target
        var distanceToTarget = Vector3.Distance(transform.position, target);
        if (distanceToTarget > 0)
        {
            // Moving towards target
            transform.position = Vector3.RotateTowards(transform.position, target, (speed / planetRadius) * Time.deltaTime, 1);
        }
    }

    public void AlignToPlanetSurface()
    {
        Vector3 unitPosition = transform.position;

        // Rotate towards the target on the y axis whilst maintaining a standing rotation on the surface of the planet
        gravityUp = (unitPosition - planet.position).normalized;
        var forward = Vector3.ProjectOnPlane(target - unitPosition, gravityUp);
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

        int maxColliders = Game.units.Count;
        Collider[] hitColliders = new Collider[maxColliders];
        var numColliders = Physics.OverlapSphereNonAlloc(transform.position, 1.1f, hitColliders, LayerMask.GetMask("Units"));

        for (int i = 0; i < numColliders; i++)
        {
            var agentA = this.transform.position;
            var agentB = hitColliders[i].transform.position;

            var unit = hitColliders[i].GetComponent<Unit>();
            if (unit.groupLeader)
                continue;

            var maxDist = 1.1f; // Default separation force
            if (unit.group != null)
                maxDist = unit.group.distanceBetweenAgents; // Adjust to groups distance between agents

            var curDist = (agentB - agentA).sqrMagnitude;

            if (curDist < maxDist)
            {
                var dir = (agentA - agentB).normalized;

                // Separate the agents from each other
                var separationForce = maxDist - curDist;
                this.transform.position += dir * separationForce * Time.deltaTime;
            }
        }
    }

    /*
     * Add a random directional force to the unit.
     * Mainly used for separating units on top of each other.
     * A force of 0.00001f is the absolute minimum required to make any difference.
     */
    private void AddRandomForce(float force) 
    {
        // Add random force so they separate if spawned on top of each other
        float separationAngle = Random.Range(0, 2 * Mathf.PI);
        Vector3 separationDirection = new Vector3(Mathf.Cos(separationAngle), 0, Mathf.Sin(separationAngle));
        transform.position += separationDirection * force;
    }

    /*
     * Leave current group if any.
     */
    public void LeaveCurrentGroup() 
    {
        if (group != null)
            group.units.Remove(this.gameObject);
    }

    public void SetMaxSpeed(float value) => speed = value;
}
