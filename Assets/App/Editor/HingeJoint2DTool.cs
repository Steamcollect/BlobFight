using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class HingeJoint2DTool : EditorWindow
{
    private List<GameObject> selectedObjects = new List<GameObject>();
    private bool allHaveRigidbody = false;

    private Vector2 scrollPosition;
    private GUIStyle labelStyle;
    private GUIStyle buttonCreateHingeStyle;

    private ReorderableList reorderableList;
    private bool modeHammer = false;
    private int life = 0;

    [MenuItem("Tools/Hinge Joint 2D Creator")]
    public static void ShowWindow()
    {
        // Create the UI
        HingeJoint2DTool window = GetWindow<HingeJoint2DTool>("Hinge Joint 2D Creator");
        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(600, 600);

        window.position = new Rect(100, 100, 300, 300);
    }

    private void OnEnable()
    {
        Selection.selectionChanged += UpdateSelection;
        UpdateSelection();
        SetupReorderableList();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= UpdateSelection;
    }

    private void UpdateSelection()
    {
        // Get All Selected GameObjects
        selectedObjects = new List<GameObject>(Selection.gameObjects);
        allHaveRigidbody = selectedObjects.Count > 0 && selectedObjects.TrueForAll(obj => obj.GetComponent<Rigidbody2D>() != null);

        SetupReorderableList();
        Repaint();
    }

    private void SetupStyles()
    {
        labelStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(10, 10, 10, 5),
        };

        buttonCreateHingeStyle = new GUIStyle(GUI.skin.button)
        {
            margin = new RectOffset(30, 30, 10, 5),
            padding = new RectOffset(10, 10, 15, 15),
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            fontSize = 20
        };
    }

    private void SetupReorderableList()
    {
        reorderableList = new ReorderableList(selectedObjects, typeof(GameObject), true, true, false, true);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Selected GameObjects", EditorStyles.boldLabel);
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            GameObject obj = selectedObjects[index];
            EditorGUI.LabelField(rect, obj.name);
        };

        reorderableList.onReorderCallback = (ReorderableList list) =>
        {
            selectedObjects = new List<GameObject>(list.list as List<GameObject>);
        };
    }

    private void OnGUI()
    {
        SetupStyles();

        GUILayout.Label("Selected GameObjects:", labelStyle);

        if (selectedObjects.Count == 0)
        {
            EditorGUILayout.HelpBox("No GameObjects selected!", MessageType.Error);
        }
        else if(selectedObjects.Count == 1)
        {
            EditorGUILayout.HelpBox("2 GameObjects must be selected!", MessageType.Error);
        }
        else if(!allHaveRigidbody)
        {
            EditorGUILayout.HelpBox("All selected objects must have a Rigidbody2D!", MessageType.Error);
        }
        else
        {
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(30);

            modeHammer = EditorGUILayout.Toggle("Hammer?", modeHammer);

            GUILayout.Space(30);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(30);

            life = EditorGUILayout.IntField("Life", life);

            GUILayout.Space(30);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Space(30);

            reorderableList.DoLayoutList();

            GUILayout.Space(30);
            GUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        GUILayout.Space(10);

        EditorGUI.BeginDisabledGroup(!allHaveRigidbody || selectedObjects.Count < 2);
        if (GUILayout.Button("CREATE HINGE JOINTS 2D", buttonCreateHingeStyle))
        {
            CreateHingeJoints();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(5);

        EditorGUI.BeginDisabledGroup(!allHaveRigidbody || selectedObjects.Count < 2);
        if (GUILayout.Button("CLEAR HINGE JOINTS 2D", buttonCreateHingeStyle))
        {
            ClearHingeJoints();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space(5);
    }

    private void ClearHingeJoints()
    {
        Undo.RecordObjects(selectedObjects.ToArray(), "CLEAR HINGE JOINTS 2D");

        for (int i = 0; i < selectedObjects.Count; i++)
        {
            foreach (var hinge in selectedObjects[i].GetComponents<HingeJoint2D>())
            {
                Undo.DestroyObjectImmediate(hinge);
            }

            if (selectedObjects[i].GetComponent<HingeHealth>() != null)
            {
                Undo.DestroyObjectImmediate(selectedObjects[i].GetComponent<HingeHealth>());
            }

            if (selectedObjects[i].GetComponent<HingeTrigger>() != null)
            {
                Undo.DestroyObjectImmediate(selectedObjects[i].GetComponent<HingeTrigger>());
            }
        }
    }

    private void CreateHingeJoints()
    {
        Undo.RecordObjects(selectedObjects.ToArray(), "CREATE HINGE JOINTS 2D");

        ClearHingeJoints();

        for (int i = 0; i < selectedObjects.Count; i++)
        {
            if (modeHammer)
            {
                GameObject current = selectedObjects[i];
                Rigidbody2D currentRb = current.GetComponent<Rigidbody2D>();

                if (i > 0)
                {
                    GameObject before = selectedObjects[i - 1];
                    Rigidbody2D beforeRb = before.GetComponent<Rigidbody2D>();

                    if (beforeRb == null)
                        continue;

                    HingeJoint2D hinge2 = current.AddComponent<HingeJoint2D>();
                    Vector2 localAnchor2;

                    // Special case for the first hinge
                    if (i == 1)
                    {
                        localAnchor2 = new Vector2(0, 1);
                        hinge2.connectedBody = selectedObjects[0].GetComponent<Rigidbody2D>();
                    }
                    // Default case: Midpoint
                    else
                    {
                        Vector2 worldMidpoint = (current.transform.position + before.transform.position) / 2;
                        localAnchor2 = current.transform.InverseTransformPoint(worldMidpoint);
                        hinge2.connectedBody = beforeRb;
                    }

                    hinge2.anchor = localAnchor2;
                    hinge2.autoConfigureConnectedAnchor = true;


                    if(i != selectedObjects.Count - 1)
                    {
                        GameObject next = selectedObjects[i + 1];
                        Rigidbody2D nextRb = next.GetComponent<Rigidbody2D>();

                        if (currentRb == null || nextRb == null)
                            continue;

                        HingeJoint2D hinge = current.AddComponent<HingeJoint2D>();
                        Vector2 localAnchor;

                        // Special case for the last hinge
                        if (i == selectedObjects.Count - 2)
                        {
                            localAnchor = new Vector2(0, -1);
                            hinge.connectedBody = selectedObjects[0].GetComponent<Rigidbody2D>();
                            hinge.connectedBody = nextRb;
                        }
                        // Default case: Midpoint
                        else
                        {
                            Vector2 worldMidpoint = (current.transform.position + next.transform.position) / 2;
                            localAnchor = current.transform.InverseTransformPoint(worldMidpoint);
                            hinge.connectedBody = nextRb;
                        }

                        hinge.anchor = localAnchor;
                        hinge.autoConfigureConnectedAnchor = true;
                    }
                }
            }
            else
            {
                if (i > 0)
                {
                    GameObject current = selectedObjects[i];
                    Rigidbody2D currentRb = current.GetComponent<Rigidbody2D>();
                    Rigidbody2D previousRb = selectedObjects[i - 1].GetComponent<Rigidbody2D>();

                    current.AddComponent<HingeHealth>();
                    current.AddComponent<HingeTrigger>();

                    current.GetComponent<HingeTrigger>().SetHealthScript(current.GetComponent<HingeHealth>());
                    current.GetComponent<HingeHealth>().maxHealth = life;
                    current.GetComponent<HingeHealth>().currentHealth = life;

                    current.GetComponent<HingeHealth>().SetHingeColor(current.GetComponent<SpriteRenderer>(), new Color32(168, 101, 38, 255), new Color32(168, 40, 38, 255));

                    if (i == 1 || i == selectedObjects.Count - 1)
                    {
                        HingeJoint2D hinge = current.AddComponent<HingeJoint2D>();
                        hinge.connectedBody = selectedObjects[0].GetComponent<Rigidbody2D>();
                        hinge.anchor = new Vector2(0, i == 1 ? -1 : 1);
                        hinge.autoConfigureConnectedAnchor = true;

                        current.GetComponent<HingeHealth>().SetHingeJoint(hinge);
                    }

                    if (i < selectedObjects.Count - 1)
                    {
                        GameObject next = selectedObjects[i + 1];
                        Rigidbody2D nextRb = next.GetComponent<Rigidbody2D>();
                        if (nextRb == null) continue;

                        HingeJoint2D hinge = current.AddComponent<HingeJoint2D>();
                        hinge.connectedBody = nextRb;
                        hinge.anchor = current.transform.InverseTransformPoint((current.transform.position + next.transform.position) / 2);
                        hinge.autoConfigureConnectedAnchor = true;

                        current.GetComponent<HingeHealth>().SetHingeJoint(hinge);
                    }
                }
            }
        }
    }
}