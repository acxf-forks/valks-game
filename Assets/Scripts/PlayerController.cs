using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The player is a temporary class to help with debugging.
public class PlayerController : MonoBehaviour
{
    public Transform planet;
    private Planet planetScript;
    public float gravityForce = -10f;
    private float radius;
    public float speed = 1f;

    private Rigidbody rb;

    private Vector3 target;

    private float playerHeight = 2;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        planetScript = planet.GetComponent<Planet>();
    }

    private Vector3 prevPos;

    private void Start()
    {
        radius = planetScript.r;
        
        transform.position = new Vector3(0, radius + playerHeight / 2, 0);

        target = new Vector3(radius, 0, 0);

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector3 gravityUp = (transform.position - planet.position).normalized;
        Vector3 bodyUp = transform.up;

        // Aligns to planets surface
        //Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * transform.rotation;
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 50 * Time.deltaTime);

        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(target - transform.position, gravityUp), gravityUp);

        // Planet gravity
        rb.AddForce(gravityUp * gravityForce);

        Debug.DrawRay(transform.position, transform.forward * 1000f, Color.green);
        Debug.DrawLine(planet.position, transform.position, Color.blue);

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
