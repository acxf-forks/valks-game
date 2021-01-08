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

        target = new Vector3(-radius, 0, 0);
    }

    private void Update()
    {
        Vector3 gravityDir = (planet.position - transform.position).normalized;

        // Always stand on planet
        transform.up = -gravityDir;

        if (Vector3.Distance(transform.position, target) > ((playerHeight / 2) + 1))
        {
            // Planet Gravity
            rb.AddForce(gravityDir * gravityForce);

            // Move to Target
            Vector3 targetDir = (target - transform.position).normalized;
            rb.AddForce(targetDir * speed);
        }
        else 
        {
            rb.velocity = Vector3.zero;
        }
    }
}
