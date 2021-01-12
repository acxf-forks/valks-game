using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Planet")]
    public Transform planet;

    [Header("Speed")]

    [Range(0.0f, 1.0f)]
    public float rotationSpeed = .25f;

    [Range(0.0f, .5f)]
    public float scrollFactor = .1f;

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

    // Why late update?
    private void LateUpdate()
    {
        // Mouse button codes
        // 0 = primary
        // 1 = secondary
        // 2 = middle

        // Handle zoom with scrolling in and out.
        // Zoom speed is handled based on how close you are to the planet.
        distanceFromPlanetSurface *= 1 - scrollFactor * Input.mouseScrollDelta.y;

        
        // Handle rotation around planet with middle mouse button drag
        if (Input.GetMouseButton(2)) 
        {
            Vector3 dir = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);

            cam.transform.Rotate(new Vector3(1, 0, 0),  (dir.y * 180) * rotationSpeed);
            cam.transform.Rotate(new Vector3(0, 1, 0), -(dir.x * 180) * rotationSpeed);
        }
        previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);

        // Recalculate position
        cam.transform.position = planet.position;
        cam.transform.Translate(new Vector3(0, 0, -planetScript.radius - distanceFromPlanetSurface));
    }

    public void FocusOnPlanet(GameObject planetGo) 
    {
        focusedOnPlanet = true;
        planet = planetGo.transform;
        planetScript = planet.GetComponent<Planet>();
        cam.transform.Translate(new Vector3(0, 0, -planetScript.radius - distanceFromPlanetSurface));
    }
}