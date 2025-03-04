using UnityEditor;
using UnityEngine;

namespace App.Scripts.Utils
{
    [CustomPropertyDrawer(typeof(TagNameAttribute))]
    public class TagNameAttributeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use [TagName] with a string.");
                return;
            }

            // Get all Unity tags
            string[] allTags = UnityEditorInternal.InternalEditorUtility.tags;

            // Get currently stored tag
            string currentTag = property.stringValue;

            // Find the index of the current tag
            int selectedIndex = System.Array.IndexOf(allTags, currentTag);
            if (selectedIndex == -1) selectedIndex = 0; // Default to first tag if not found

            // Show tag dropdown
            selectedIndex = EditorGUI.Popup(position, label.text, selectedIndex, allTags);

            // Update property if changed
            if (selectedIndex >= 0 && selectedIndex < allTags.Length)
            {
                property.stringValue = allTags[selectedIndex];
            }
        }
    }
}