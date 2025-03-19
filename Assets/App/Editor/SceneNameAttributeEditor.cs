using System.Linq;
using UnityEditor;
using UnityEngine;

namespace App.Scripts.Utils
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class TagsNameAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Get Property
            SerializedProperty guidProperty = property.FindPropertyRelative("sceneGUID");
            SerializedProperty nameProperty = property.FindPropertyRelative("sceneName");

            if (guidProperty == null || nameProperty == null)
            {
                EditorGUI.LabelField(position, label.text, "Invalid SceneReference.");
                EditorGUI.EndProperty();
                return;
            }

            var buildScenes = EditorBuildSettings.scenes;
            if (buildScenes.Length == 0)
            {
                EditorGUI.LabelField(position, label.text, "No scenes in Build Settings.");
                EditorGUI.EndProperty();
                return;
            }

            // Get List of Scene Paths and Names
            var scenePaths = buildScenes.Select(scene => scene.path).ToArray();
            var sceneNames = scenePaths.Select(System.IO.Path.GetFileNameWithoutExtension).ToArray();
            var sceneGUIDs = scenePaths.Select(AssetDatabase.AssetPathToGUID).ToArray();

            // Find index based on GUID
            int selectedIndex = System.Array.IndexOf(sceneGUIDs, guidProperty.stringValue);
            if (selectedIndex == -1) selectedIndex = 0;

            // Display Dropdown
            int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, sceneNames);
            if (newIndex != selectedIndex)
            {
                guidProperty.stringValue = sceneGUIDs[newIndex];
                nameProperty.stringValue = sceneNames[newIndex];
            }

            EditorGUI.EndProperty();
        }
    }
}