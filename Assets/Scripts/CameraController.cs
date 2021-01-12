using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform planet;
    private bool focusedOnPlanet;

    private Planet planetScript;
    private Vector3 previousPosition;
    private Camera cam;

    public float distanceFromPlanetSurface = 20;

    private void Awake()
    {
        cam = gameObject.AddComponent<Camera>();
        gameObject.tag = "MainCamera"; // This is the main camera
        gameObject.name = "Main Camera";
    }

    private void LateUpdate()
    {
        if (focusedOnPlanet)
            RotateAroundPlanet();
    }

    private void RotateAroundPlanet() 
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

            cam.transform.Translate(new Vector3(0, 0, -planetScript.radius - distanceFromPlanetSurface));

            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        }
    }

    public void FocusOnPlanet(GameObject planetGo) 
    {
        focusedOnPlanet = true;
        planet = planetGo.transform;
        planetScript = planet.GetComponent<Planet>();
        cam.transform.Translate(new Vector3(0, 0, -planetScript.radius - distanceFromPlanetSurface));
    }
}