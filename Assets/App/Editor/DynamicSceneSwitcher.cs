using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;

public class DynamicSceneSwitcher : EditorWindow
{
    private Vector2 scrollPosition;
    private string[] scenePaths;

    private GUIStyle buttonStyle;
    private float scale;

    private GUIContent refreshIconContent;

    [MenuItem("Tools/Dynamic Scenes Switcher")]
    public static void ShowWindow()
    {
        DynamicSceneSwitcher window = GetWindow<DynamicSceneSwitcher>();
        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(1000, 1000);
        window.position = new Rect(100, 100, 400, 400);
    }

    private void OnEnable()
    {
        RefreshSceneList();
        refreshIconContent = new GUIContent(
            EditorGUIUtility.IconContent("d_RotateTool").image,
            "Refresh the scene list"
        );
    }

    private void RefreshSceneList()
    {
        scenePaths = AssetDatabase.FindAssets("t:Scene")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => path.StartsWith("Assets/"))
            .ToArray();
    }

    private void SetupStyles()
    {
        float width = position.width;
        scale = Mathf.Clamp(width / 400f, 0.75f, 2f);

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            margin = new RectOffset(20, 20, 5, 5),
            padding = new RectOffset(10, 10, 8, 8),
            fontStyle = FontStyle.Normal,
            fontSize = Mathf.RoundToInt(14 * scale),
            alignment = TextAnchor.MiddleCenter
        };
    }

    private void OnGUI()
    {
        SetupStyles();

        EditorGUILayout.Space(5);

        if (scenePaths == null || scenePaths.Length == 0)
        {
            EditorGUILayout.LabelField("No Scenes Found", EditorStyles.centeredGreyMiniLabel);
        }
        else
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

            foreach (string scenePath in scenePaths)
            {
                if (GUILayout.Button(Path.GetFileNameWithoutExtension(scenePath), buttonStyle) &&
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        // --- Refresh Icon en bas à droite proprement ---
        float iconSize = 20f;
        float bgSize = 28f;
        float spacingFromEdge = 8f;

        // On s'assure qu'il ne touche pas la scrollbar
        float xPos = position.width - bgSize - spacingFromEdge;
        float yPos = position.height - bgSize - spacingFromEdge;

        Rect bgRect = new Rect(xPos, yPos, bgSize, bgSize);
        Rect iconRect = new Rect(
            xPos + (bgSize - iconSize) / 2,
            yPos + (bgSize - iconSize) / 2,
            iconSize,
            iconSize
        );

        // Fond discret derrière l’icône
        GUI.Box(bgRect, GUIContent.none);

        // Bouton invisible sur l’icône, avec tooltip
        if (GUI.Button(iconRect, refreshIconContent, GUIStyle.none))
        {
            RefreshSceneList();
            Repaint();
        }
    }
}