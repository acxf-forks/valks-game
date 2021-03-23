using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is an editor utility to stop planet meshes being saved as they can be regenerated.

The `OnWillSaveAssets` function in the `DestroyOnSaveProcessor` will be called by the unity editor before saving a scene.

It will then call the `HandleSaving` method on the `OnSave` monobehaviour. This has to be a monobehaviour to access the `FindObjectsOfType` method.

The `HandleSaving` function deletes all the planet meshes, and then queues up the `GeneratePlanet` function to be run after the save is complete.

In summary:
1. Before the save event, `OnWillSaveAssets` calls `HandleSaving`
2. Then every mesh is destroyed item.DestroyTemp();
3.`UnityEditor.EditorApplication.delayCall += item.GeneratePlanet;` will queue up the `GeneratePlanet` function to be called on the next frame.
3. The unity editor will then save the scene without these meshes.
4. The `GeneratePlanet` method will be called on all the processed planets so the meshes are regenerated.

*/



public class OnSaveProcessor : UnityEditor.AssetModificationProcessor
{
	static string[] OnWillSaveAssets(string[] paths)
	{
		OnSave.HandleSaving();
		return paths;
	}
}

// Must be a monobehaviour to acess `FindObjectsOfType`
public class OnSave : MonoBehaviour{
	public static void HandleSaving(){
		var planets = FindObjectsOfType<Planet>();

		foreach (var item in planets)
		{
			item.DestroyTemp();
			UnityEditor.EditorApplication.delayCall += item.GeneratePlanet;
		}
	}
}
