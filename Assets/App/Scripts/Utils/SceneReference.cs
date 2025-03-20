using System;
using UnityEngine;

[Serializable]
public class SceneReference
{
    [SerializeField] private string sceneName;
    [SerializeField] private string sceneGUID;

    public string Name => sceneName;

    public string GUID => sceneGUID;

    public static implicit operator string(SceneReference sceneRef)
    {
        return sceneRef.sceneName;
    }
}