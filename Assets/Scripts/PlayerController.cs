using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The player is a temporary class to help with debugging.
public class PlayerController : MonoBehaviour
{
    public Transform planet;
    private Planet planetScript;
    public float gravityForce = 1f;
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
    }

    private void Update()
    {

        

        // Always stand on planet
        

        if (Vector3.Distance(transform.position, target) > ((playerHeight / 2) + 1))
        {
            Vector3 gravityDir = (transform.position - planet.position).normalized;

            Vector3 targetAngle = Quaternion.LookRotation(target - transform.position).eulerAngles;
            transform.eulerAngles = new Vector3(0, targetAngle.y, 0);
            transform.up = gravityDir;

            // Planet Gravity
            rb.AddForce(gravityDir * gravityForce);

            // Move to Target
            //
            //rb.AddForce(targetDir * speed);
            //rb.AddForce(transform.forward * speed);
        }
        else 
        {
            rb.velocity = Vector3.zero;
        }
    }
}
