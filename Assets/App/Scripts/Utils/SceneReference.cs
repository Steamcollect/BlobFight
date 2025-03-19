using System;
using UnityEngine;

[Serializable]
public class SceneReference
{
    [SerializeField] private string sceneName; // Cached name
    [SerializeField] private string sceneGUID;

    public string Name => sceneName;

    public string GUID => sceneGUID;
    

    public void Set(string guid, string name)
    {
        sceneGUID = guid;
        sceneName = name;
    }
}