using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhysicsPropsManager))]
public class PhysicsPropsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty rigidbodysProperty = serializedObject.FindProperty("rigidbodys");
        EditorGUILayout.PropertyField(rigidbodysProperty);

        PhysicsPropsManager manager = (PhysicsPropsManager)target;

        if (GUILayout.Button("Get All Rigidbodies from Scene"))
        {
            Undo.RecordObject(manager, "Get All Rigidbodies");

            var method = manager.GetType().GetMethod("GetAllRigidbodysFromScene",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                method.Invoke(manager, null);
                EditorUtility.SetDirty(manager);
            }
        }

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script", "rigidbodys");

        serializedObject.ApplyModifiedProperties();
    }
}