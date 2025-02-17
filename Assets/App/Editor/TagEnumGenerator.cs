using System.IO;
using UnityEditor;

public static class TagEnumGenerator
{
    private const string EnumFilePath = "Assets/App/Scripts/Container/TagsEnum.cs";

    [MenuItem("Tools/Generate Tags Enum")]
    public static void GenerateTagsEnum()
    {
        string[] tags = UnityEditorInternal.InternalEditorUtility.tags;

        // Start creating the enum code with explicit values based on the index
        string enumCode = "public enum TagsName\n{\n";
        for (int i = 0; i < tags.Length; i++)
        {
            // Convert space and dash to underscores, then add an explicit integer value
            string tagName = tags[i].Replace(" ", "_").Replace("-", "_");
            enumCode += $"    {tagName} = {i},\n"; // Assign the index as the enum value
        }
        enumCode += "}";

        // Ensure directory exists and write the enum code to a C# file
        Directory.CreateDirectory(Path.GetDirectoryName(EnumFilePath));
        File.WriteAllText(EnumFilePath, enumCode);

        // Refresh Unity AssetDatabase 
        AssetDatabase.Refresh();
    }
}
