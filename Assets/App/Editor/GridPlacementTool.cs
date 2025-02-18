using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridPlacementTool : EditorWindow
{
    public GameObject objectToPlace;
    private Tilemap tilemap;

    [MenuItem("Tools/Tilemap Object Placer")]
    public static void ShowWindow()
    {
        GetWindow<GridPlacementTool>("Tilemap Object Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Place GameObject on Tilemap", EditorStyles.boldLabel);

        objectToPlace = (GameObject)EditorGUILayout.ObjectField("GameObject to Place", objectToPlace, typeof(GameObject), false);
        tilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", tilemap, typeof(Tilemap), true);

        if (tilemap != null && objectToPlace != null)
        {
            if (GUILayout.Button("Start Placing"))
            {
                SceneView.duringSceneGui += OnSceneGUI;
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            Vector3 worldPosition = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin;
            Vector3Int gridPosition = tilemap.WorldToCell(worldPosition);

            // Get the tile size and adjust the placement to the center of the tile
            Vector3 tileSize = tilemap.cellSize;
            Vector3 snappedPosition = tilemap.CellToWorld(gridPosition) + new Vector3(tileSize.x / 2, tileSize.y / 2, 0);

            // Ensure the GameObject is placed at the center of the tile
            snappedPosition.z = 0; // Keep the object in the 2D layer (adjust if in 3D)

            Instantiate(objectToPlace, snappedPosition, Quaternion.identity);
            Event.current.Use(); // Consume the event to prevent other operations
        }

        sceneView.Repaint();
    }
}