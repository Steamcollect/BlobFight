using UnityEditor;
using UnityEngine;

namespace App.Scripts.Utils
{
    [CustomPropertyDrawer(typeof(SceneNameAttribute))]
    public class SceneNameAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [Scene] with a string.");
                return;
            }
            
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = System.IO.Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);
            }

            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(scenes, property.stringValue));
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, scenes);

            if (selectedIndex >= 0 && selectedIndex < scenes.Length)
            {
                property.stringValue = scenes[selectedIndex];
            }
        }
    }
}