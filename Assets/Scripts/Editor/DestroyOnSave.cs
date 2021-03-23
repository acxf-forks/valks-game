using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Will stop prieview meshes being saved.

// Must be a monobehaviour to acess `FindObjectsOfType`
public class DestroyOnSave : MonoBehaviour{
	public static void Handle(){
		var planets = FindObjectsOfType<Planet>();

		foreach (var item in planets)
		{
			item.DestroyTemp();
			UnityEditor.EditorApplication.delayCall += item.GeneratePlanet;
		}
	}
}


public class DestroyOnSaveProcessor : UnityEditor.AssetModificationProcessor
{
	static string[] OnWillSaveAssets(string[] paths)
	{
		DestroyOnSave.Handle();
		return paths;
	}
}
