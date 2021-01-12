using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The player is a temporary class to help with debugging.
public class Unit : MonoBehaviour
{
    public Transform planet;
    private Planet planetScript;

    private float planetRadius;

    public float speed = 1f;

    private Rigidbody rb;

    private Vector3 target;

    private float playerHeight = 2;

    private static int unitCount = 0;

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Units");
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        planetScript = planet.GetComponent<Planet>();
        planetRadius = planetScript.radius;

        transform.position = new Vector3(0, planetRadius + playerHeight / 2, 0);
        target = new Vector3(planetRadius, 0, 0);

        gameObject.name = $"({unitCount}) Unit";
    }

    private void FixedUpdate()
    {
        MoveToTarget();
    }

    private void MoveToTarget() 
    {
        Vector3 gravityUp = (transform.position - planet.position).normalized;

        // Rotate towards the target.
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(target - transform.position, gravityUp), gravityUp);

        if (Vector3.Distance(transform.position, target) > ((playerHeight / 2) + 2))
        {
            if (rb.velocity.magnitude < 2)
            {
                rb.AddForce(transform.forward * speed);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // DEBUG: Draw target visual.
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target, 1);
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
