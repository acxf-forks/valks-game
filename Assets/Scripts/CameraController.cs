using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform planet;
    public GameObject player;

    private Planet planetScript;
    private Vector3 previousPosition;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        planetScript = planet.GetComponent<Planet>();
    }

    private void Start()
    {
        cam.transform.Translate(new Vector3(0, 0, -planetScript.r - 20));
    }

    private void LateUpdate()
    {
        // 0 = primary
        // 1 = secondary
        // 2 = middle
        if (Input.GetMouseButtonDown(2)) 
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);

        if (Input.GetMouseButton(2)) 
        {
            Vector3 dir = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);

            cam.transform.position = planet.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), (dir.y * 180) / 4);
            cam.transform.Rotate(new Vector3(0, 1, 0), -(dir.x * 180) / 4);

            cam.transform.Translate(new Vector3(0, 0, -planetScript.r - 20));

            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
    }
}