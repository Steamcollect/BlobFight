using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace App.Scripts.Utils
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class DictionaryPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Find the serialized Lists inside SerializableDictionary
            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            SerializedProperty valuesProperty = property.FindPropertyRelative("values");

            if (keysProperty == null || valuesProperty == null)
            {
                EditorGUI.LabelField(position, "Dictionary serialization error.");
                return;
            }

            // Foldout for collapsing/expanding
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);
            if (!property.isExpanded) return;

            EditorGUI.indentLevel++;

            // Iterate through dictionary items
            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                SerializedProperty key = keysProperty.GetArrayElementAtIndex(i);
                SerializedProperty value = valuesProperty.GetArrayElementAtIndex(i);

                Rect itemRect = new Rect(position.x, position.y + (i + 1) * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(new Rect(itemRect.x, itemRect.y, itemRect.width / 2, itemRect.height), key, GUIContent.none);
                EditorGUI.PropertyField(new Rect(itemRect.x + itemRect.width / 2, itemRect.y, itemRect.width / 2, itemRect.height), value, GUIContent.none);

                // Delete button
                if (GUI.Button(new Rect(itemRect.x + itemRect.width - 20, itemRect.y, 20, itemRect.height), "X"))
                {
                    keysProperty.DeleteArrayElementAtIndex(i);
                    valuesProperty.DeleteArrayElementAtIndex(i);
                    break;
                }
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
            SerializedProperty keysProperty = property.FindPropertyRelative("keys");
            return (keysProperty.arraySize + 2) * EditorGUIUtility.singleLineHeight;
        }
    }
}
