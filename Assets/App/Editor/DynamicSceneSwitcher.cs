using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;

public class DynamicSceneSwitcher : EditorWindow
{
    private Vector2 scrollPosition;
    private string[] scenePaths;

    private GUIStyle labelStyle;
    private GUIStyle buttonStyle;
    private GUIStyle buttonRefreshStyle;
    private GUIStyle scrollStyle;

    [MenuItem("Tools/Dynamic Scene Switcher")]
    public static void ShowWindow()
    {
        // Create the UI
        DynamicSceneSwitcher window = GetWindow<DynamicSceneSwitcher>("Scene Switcher");
        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(600, 600);

        window.position = new Rect(100, 100, 300, 300);
    }

	private void RefreshSceneList()
    {
        // Get All Scenes in the Project
		scenePaths = AssetDatabase.FindAssets("t:Scene").Select(AssetDatabase.GUIDToAssetPath).ToArray();
	}

    private void SetupStyles()
    {
        labelStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(10, 10, 10, 5),
        };

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            margin = new RectOffset(30, 30, 10, 5),
            padding = new RectOffset(10, 10, 5, 5),
            fontStyle = FontStyle.Bold,
        };

        buttonRefreshStyle = new GUIStyle(GUI.skin.button)
        {
            margin = new RectOffset(30, 30, 10, 5),
            padding = new RectOffset(10, 10, 15, 15),
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            fontSize = 20
        };
    }

    private void OnGUI()
    {
        SetupStyles();

        RefreshSceneList();

		GUILayout.Label("All scenes in the project:", labelStyle);

        EditorGUILayout.Space(5);

        if (scenePaths == null || scenePaths.Length == 0)
        {
			GUILayout.Label("No scene found");
			return;
		}

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        foreach (string scenePath in scenePaths)
        {
            if (GUILayout.Button(Path.GetFileNameWithoutExtension(scenePath), buttonStyle) && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
				EditorSceneManager.OpenScene(scenePath);
			}
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(5);

        if (GUILayout.Button("REFRESH SCENES", buttonRefreshStyle))
        {
            RefreshSceneList();
        }

        EditorGUILayout.Space(5);
    }
}