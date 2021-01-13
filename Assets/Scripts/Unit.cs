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

    private Vector3 startPos;
    private bool getStartPos = true;

    [Header("Movement")]
    public float progress = 0;
    public float curSpeed = 0;

    private const float maxSpeed = 0.03f;
    private const float accSpeed = 0.0005f;

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
    }

    public void MoveToTarget(Vector3 target) 
    {
        if (getStartPos) 
        {
            getStartPos = false;
            startPos = transform.position;
        }

        var gravityUp = (transform.position - planet.position).normalized;

        // Rotate towards the target on the y axis whilst maintaining a standing rotation on the surface of the planet
        Vector3 forward = Vector3.ProjectOnPlane(target - transform.position, gravityUp);
        if (forward != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(forward, gravityUp);

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

            // Moving towards target
            transform.position = Vector3.Slerp(startPos, target, progress);
            progress += (curSpeed / planetRadius);
        }
        else 
        {
            // Reached target
            getStartPos = true;
        }
    }

    private void AStar(Vector3 start, Vector3 goal) 
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
    }
}
