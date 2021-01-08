using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform planet;
    public float gravityForce = 1f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 gravityDir = (planet.position - transform.position).normalized;
        rb.AddForce(gravityDir * gravityForce);
        transform.up = -gravityDir;
    }
}
