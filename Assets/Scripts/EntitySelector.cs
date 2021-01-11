using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySelector : MonoBehaviour
{
    private Dictionary<int, GameObject> selectedEntities = new Dictionary<int, GameObject>();
    private bool dragSelect;
    private Vector3 p1; // First mouse position on screen
    
    private Camera cam;
    private CameraController camScript;

    public GameObject planet;
    private Planet planetScript;

    private RaycastHit hit;
    private Vector2[] cornersScreenSpace;
    private Vector3[] cornersWorldSpace;

    // For reference to drawing debug selection box gizmo
    private Vector3 middle;
    private float selectionBoxLength;
    private float selectionBoxWidth;
    private float selectionBoxHeight;

    // Debug
    public float debugDrawTime = 1f;
    public bool debugEnabled = false;

    private void Start()
    {
        cam = Camera.main;
        planetScript = planet.GetComponent<Planet>();
        camScript = cam.GetComponent<CameraController>();
    }

    private void Update()
    {
        #region Left Clicked
        if (Input.GetMouseButtonDown(0)) 
        {
            p1 = Input.mousePosition;
        }
        #endregion

        #region Hold Left Click
        if (Input.GetMouseButton(0))
        {
            if ((p1 - Input.mousePosition).magnitude > 20)
                dragSelect = true;
        }
        #endregion

        #region Left Click Released
        if (Input.GetMouseButtonUp(0))
        {
            // Currently not dragging
            if (!dragSelect)
            {
                // Did p1 hit anything?
                Ray ray = cam.ScreenPointToRay(p1);
                if (Physics.Raycast(ray, out hit, 500000))
                {
                    GameObject go = hit.transform.gameObject;
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        // Shift add select
                        if (!IsSelected(go))
                        {
                            AddSelected(go);
                        }
                        else
                        {
                            Deselect(go);
                        }
                    }
                    else
                    {
                        // Singular select
                        DeslectAll();
                        AddSelected(go);
                    }
                }
                else // We did not hit anything.
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        // Do nothing
                    }
                    else
                    {
                        DeslectAll();
                    }
                }
            }
            else // Marquee Select
            {
                cornersWorldSpace = new Vector3[4];
                cornersScreenSpace = GetBoundingBox(p1, Input.mousePosition);

                bool allPointsHitSurface = true; // Did all selection points hit the surface of the planet?

                for (int i = 0; i < cornersScreenSpace.Length; i++) 
                {
                    Ray ray = cam.ScreenPointToRay(cornersScreenSpace[i]);

                    if (Physics.Raycast(ray, out hit, planetScript.r, LayerMask.NameToLayer("Planets")))
                    {
                        cornersWorldSpace[i] = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    }
                    else 
                    {
                        // Did not hit planet surface
                        allPointsHitSurface = false;
                        cornersWorldSpace[i] = ray.GetPoint(camScript.distanceFromPlanetSurface + planetScript.r / 2);
                    }

                    // DEBUG: Draw visual of 3D selection box.
                    if (debugEnabled) Debug.DrawLine(cam.ScreenToWorldPoint(cornersScreenSpace[i]), hit.point, Color.yellow, debugDrawTime);
                }

                var middleTop = (cornersWorldSpace[0] + cornersWorldSpace[1]) / 2;
                var middleBottom = (cornersWorldSpace[2] + cornersWorldSpace[3]) / 2;
                middle = (middleTop + middleBottom) / 2;

                // DEBUG: Draw visual of center.
                if (debugEnabled) Debug.DrawLine(cam.transform.position, middle, Color.magenta, debugDrawTime);

                selectionBoxLength = Vector3.Distance(cornersWorldSpace[0], cornersWorldSpace[1]);
                selectionBoxWidth = Vector3.Distance(cornersWorldSpace[2], cornersWorldSpace[3]);

                // Adjust selection box height based on if all points hit the planets surface or not
                if (allPointsHitSurface)
                    selectionBoxHeight = planetScript.r / 2; // Not too much or our rectangles will always look like boxes
                else
                    selectionBoxHeight = planetScript.r; // Compensate for missed surfaces

                Vector3 gravityUp = (middle - planet.transform.position).normalized;
                Collider[] hitColliders = Physics.OverlapBox(middle, new Vector3(selectionBoxWidth / 2, selectionBoxHeight / 2, selectionBoxLength / 2), Quaternion.LookRotation(cam.transform.up, gravityUp));

                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    // TODO: Would be more efficient to only deselect the ones that needed to be deselected based on the new selected
                    DeslectAll();
                }

                foreach (var collider in hitColliders)
                {
                    AddSelected(collider.gameObject);
                }
            }

            dragSelect = false;
        }
        #endregion
    }

    #region Selection
    private void AddSelected(GameObject go)
    {
        if (go.layer != LayerMask.NameToLayer("Units"))
            return;

        int id = go.GetInstanceID();

        if (!selectedEntities.ContainsKey(id))
        {
            selectedEntities.Add(id, go);
            go.GetComponent<Renderer>().material.color = Color.green;
        }
    }

    private void Deselect(GameObject go)
    {
        selectedEntities[go.GetInstanceID()].GetComponent<Renderer>().material.color = Color.red;
        selectedEntities.Remove(go.GetInstanceID());
    }

    private void DeslectAll()
    {
        foreach (KeyValuePair<int, GameObject> pair in selectedEntities)
        {
            if (pair.Value != null)
                selectedEntities[pair.Key].GetComponent<Renderer>().material.color = Color.red;
        }
        selectedEntities.Clear();
    }

    private bool IsSelected(GameObject go)
    {
        return selectedEntities.ContainsKey(go.GetInstanceID());
    }
    #endregion

    #region Draw Screen Rect
    private void OnGUI()
    {
        if (dragSelect)
        {
            var rect = GetScreenRect(p1, Input.mousePosition);
            DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.8f, 0.2f));
            DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.8f));
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
        // Calculate cornersScreenSpace
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    //create a bounding box (4 cornersScreenSpace in order) from the start and end mouse position
    Vector2[] GetBoundingBox(Vector2 p1, Vector2 p2)
    {
        Vector2 newP1;
        Vector2 newP2;
        Vector2 newP3;
        Vector2 newP4;

        if (p1.x < p2.x) //if p1 is to the left of p2
        {
            if (p1.y > p2.y) // if p1 is above p2
            {
                newP1 = p1;
                newP2 = new Vector2(p2.x, p1.y);
                newP3 = new Vector2(p1.x, p2.y);
                newP4 = p2;
            }
            else //if p1 is below p2
            {
                newP1 = new Vector2(p1.x, p2.y);
                newP2 = p2;
                newP3 = p1;
                newP4 = new Vector2(p2.x, p1.y);
            }
        }
        else //if p1 is to the right of p2
        {
            if (p1.y > p2.y) // if p1 is above p2
            {
                newP1 = new Vector2(p2.x, p1.y);
                newP2 = p1;
                newP3 = p2;
                newP4 = new Vector2(p1.x, p2.y);
            }
            else //if p1 is below p2
            {
                newP1 = p2;
                newP2 = new Vector2(p1.x, p2.y);
                newP3 = new Vector2(p2.x, p1.y);
                newP4 = p1;
            }

        }

        Vector2[] cornersScreenSpace = { newP1, newP2, newP3, newP4 };
        return cornersScreenSpace;

    }

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
    #endregion

    private void OnDrawGizmos()
    {
        if (!debugEnabled)
            return;

        if (selectionBoxLength == 0 && selectionBoxWidth == 0)
            return;

        Vector3 gravityUp = (middle - planet.transform.position).normalized;
        Gizmos.matrix = Matrix4x4.TRS(middle, Quaternion.LookRotation(cam.transform.up, gravityUp), this.transform.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(selectionBoxWidth, selectionBoxHeight, selectionBoxLength));
    }
}
