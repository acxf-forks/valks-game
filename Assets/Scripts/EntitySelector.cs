using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySelector : MonoBehaviour
{
    private Vector3 p1;
    private Vector3 p2;

    private Camera cam;

    private bool selectionEnabled;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            p1 = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0)) 
        {
            p2 = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            selectionEnabled = true;
        }
        else 
        {
            selectionEnabled = false;
        }
    }

    private void OnGUI()
    {
        if (selectionEnabled) 
        {
            var rect = GetScreenRect(p1, Input.mousePosition);
            DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.8f, 0.2f));
            DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.8f));
        }
    }

    // Everything below copied directly from https://github.com/pickles976/RTS_selection/blob/master/Utils.cs
    // Unity RTS - Box Selection Tutorial https://www.youtube.com/watch?v=OL1QgwaDsqo
    static Texture2D _whiteTexture;
    public static Texture2D WhiteTexture
    {
        get
        {
            if (_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }

            return _whiteTexture;
        }
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
