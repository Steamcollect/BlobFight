using System.IO;
using UnityEditor;
using UnityEngine.SceneManagement;

public static class SceneEnumGenerator
{
	private const string EnumFilePath = "Assets/App/Scripts/Container/ScenesEnum.cs";

	[MenuItem("Tools/Generate Scenes Enum")]
	public static void GenerateScenesEnum()
	{
		// Get all scene paths in the build settings
		int sceneCount = SceneManager.sceneCountInBuildSettings;
		string[] scenes = new string[sceneCount];
		for (int i = 0; i < sceneCount; i++)
		{
			scenes[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
		}

        // Start creating the enum code with explicit values based on the index
        string enumCode = "public enum ScenesName\n{\n";
        for (int i = 0; i < scenes.Length; i++)
        {
            // Convert space and dash to underscores, then add an explicit integer value
            string sceneName = scenes[i].Replace(" ", "_").Replace("-", "_");
            enumCode += $"    {sceneName} = {i},\n"; // Assign the index as the enum value
        }
        enumCode += "}";

        // Ensure directory exists and write the enum code to a C# file
        Directory.CreateDirectory(Path.GetDirectoryName(EnumFilePath));
		File.WriteAllText(EnumFilePath, enumCode);

		// Refresh Unity AssetDatabase 
		AssetDatabase.Refresh();
	}
}