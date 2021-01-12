using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    List<GameObject> units = new List<GameObject>();

    private void Start()
    {
        // Create planet
        var planetGo = new GameObject();
        var planet = planetGo.AddComponent<Planet>();
        planet.planetName = "Yomolla";
        planet.radius = 100;

        // Create unit
        var unitGoPrefab = Resources.Load<GameObject>("Prefabs/Unit");
        var unitGo = Instantiate(unitGoPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        var unit = unitGo.GetComponent<Unit>();

        unit.planet = planetGo.transform;
        unit.speed = 20;

        var playerHeight = 2;
        unitGo.transform.position = unit.planet.position + new Vector3(0, planet.radius + playerHeight / 2, 0);

        units.Add(unitGo);

        // Create main camera
        var cameraGo = new GameObject();
        cameraGo.transform.Rotate(new Vector3(90, 0, 0));
        var camera = cameraGo.AddComponent<CameraController>();
        camera.planet = planetGo.transform;

        // Create entity selector
        var entitySelector = GameObject.Find("Manager").AddComponent<EntitySelector>();
        entitySelector.planet = planetGo;
        entitySelector.debugDrawTime = 2f;
        entitySelector.debugEnabled = true;
    }
}
