using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{
    private Planet planet;

    private Editor planetEditor;
    private Editor terrainShapeEditor;
    private Editor colourEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }
        GUILayout.Space(10);

        DrawSettingsEditor(planet.planetSettings, planet.OnPlanetSettingsUpdated, ref planet.planetSettingsFoldout, ref planetEditor);
        GUILayout.Space(10);
        if (GUILayout.Button("Update Planet"))
        {
            planet.OnPlanetSettingsUpdated();
        }
        GUILayout.Space(10);
        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref terrainShapeEditor);
        GUILayout.Space(10);
        if (GUILayout.Button("Update Shape"))
        {
            planet.OnShapeSettingsUpdated();
        }
        GUILayout.Space(10);
        DrawSettingsEditor(planet.colourSettings, planet.OnColourSettingsUpdated, ref planet.colourSettingsFoldout, ref colourEditor);
        GUILayout.Space(10);
        if (GUILayout.Button("Update Colours"))
        {
            planet.OnColourSettingsUpdated();
        }
        GUILayout.Space(10);
    }

    private void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                if (foldout)
                {
                    CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (check.changed)
                    {
                        if (onSettingsUpdated != null)
                        {
                            onSettingsUpdated();
                        }
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        planet = (Planet)target;
    }
}