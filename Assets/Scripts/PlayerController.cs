using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform planet;
    public float gravityForce = 1f;
    public float speed = 1f;
    private float angle = 0f;

    private Rigidbody rb;

    private Vector3 target = new Vector3(-100, 0, 0);

    private void Awake()
    {
        //rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector3 gravityDir = (planet.position - transform.position).normalized;
        //rb.AddForce(gravityDir * gravityForce);
        //transform.up = -gravityDir;

        var radius = 101;
        Vector3 newPos = planet.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
        transform.position = newPos;
        angle += 0.1f;
    }
}
