using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public List<GameObject> units = new List<GameObject>();
    private Planet planet;

    private void Start()
    {
        // Create main camera
        var cameraGo = new GameObject();
        cameraGo.transform.Rotate(new Vector3(90, 0, 0));
        var camera = cameraGo.AddComponent<CameraController>();

        // Create planet
        var planetGo = new GameObject();
        var planet = planetGo.AddComponent<Planet>();
        planet.planetName = "Yomolla";
        planet.radius = 10;
        camera.FocusOnPlanet(planetGo);
        this.planet = planet;

        // Create unit
        var unitGoPrefab = Resources.Load<GameObject>("Prefabs/Unit");
        var unitGo = Instantiate(unitGoPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        var unit = unitGo.GetComponent<Unit>();
        planet.AddAttractedEntity(unitGo.GetComponent<Rigidbody>());

        unit.planet = planetGo.transform;
        unit.speed = 20;

        var playerHeight = 2;
        unitGo.transform.position = unit.planet.position + new Vector3(0, planet.radius + playerHeight / 2, 0);

        units.Add(unitGo);

        // Create entity selector
        var entitySelector = GameObject.Find("Manager").AddComponent<EntitySelector>();
        entitySelector.planet = planetGo;
    }

    private void FixedUpdate()
    {
        foreach (var unit in units) 
        {
            // Seems inefficient to get the component everytime
            unit.GetComponent<Unit>().MoveToTarget(new Vector3(planet.radius, 0, 0));
        }
    }
}
