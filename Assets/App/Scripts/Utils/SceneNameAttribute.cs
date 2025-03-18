using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class SceneNameAttribute : PropertyAttribute
{
    [SerializeField, HideInInspector] private int index;
    [SerializeField] private string sceneGUID;
    [SerializeField] private string sceneName;

    public string GUID => sceneGUID;
    public string Name => sceneName;

    public void SetScene(string guid)
    {
        sceneGUID = guid;
        sceneName = GetSceneNameFromGUID(guid);
    }

    private static string GetSceneNameFromGUID(string guid)
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        return System.IO.Path.GetFileNameWithoutExtension(path);
    }
}