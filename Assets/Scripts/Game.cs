using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public List<GameObject> units = new List<GameObject>();
    private Planet planet;
    private UnitGroup group;
    public int planetRadius = 50;

    private void Awake()
    {
        // Create main camera
        var cameraGo = new GameObject();
        cameraGo.transform.Rotate(new Vector3(90, 0, 0));
        var camera = cameraGo.AddComponent<CameraController>();

        // Create planet
        var planetGo = new GameObject();
        var planet = planetGo.AddComponent<Planet>();
        planet.planetName = "Yomolla";
        planet.radius = planetRadius;
        camera.FocusOnPlanet(planetGo);
        this.planet = planet;

        // Create units
        var unitGoPrefab = Resources.Load<GameObject>("Prefabs/Unit");
        for (int i = 0; i < 100; i++) 
        {
            var unitGo = Instantiate(unitGoPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            var unit = unitGo.GetComponent<Unit>();
            planet.AddAttractedEntity(unitGo.GetComponent<Rigidbody>());

            unit.planet = planetGo.transform;

            unitGo.transform.position = unit.planet.position + new Vector3(0, planet.radius + 1, 0);

            units.Add(unitGo);
        }

        group = new UnitGroup(units, planetGo.transform);
        
        // Create entity selector
        var entitySelector = GameObject.Find("Manager").AddComponent<EntitySelector>();
        entitySelector.planet = planetGo;
    }

    private void Update()
    {
        foreach (var unit in units) 
        {
            Unit unitScript = unit.GetComponent<Unit>();
            unitScript.AlignToPlanetSurface();
            unitScript.Separation();
        }

        group.MoveToTarget(new Vector3(planet.radius + 1, 0, 0));
    }

    private void OnDrawGizmos()
    {
        if (planet == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(planet.radius + 1, 0, 0), 0.5f);
    }
}
