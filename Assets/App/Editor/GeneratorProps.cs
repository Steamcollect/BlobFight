using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Damagable;

public enum Type
{
    None = 0,
    Bridge  = 1,
    Hammer = 2,
}

public class GeneratorProps : EditorWindow
{
    [SerializeField] private Type type = Type.None;

    [SerializeField] private GameObject prefabChain = null;
    [SerializeField] private int number = 5;
    [SerializeField] private float spacing = 1;
    [SerializeField] private int health = 500;
    [SerializeField] private AnimationCurve damageBySpeedCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(10000, 100));

    [SerializeField] private GameObject prefabBall = null;
    [SerializeField] private float spacingBall = 1.2f;
    [SerializeField] private int damage = 100;
    [SerializeField] private DamageType damageType = DamageType.Kill;

    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    private Vector2 scrollPosition = Vector2.zero;
    private GUIStyle labelStyle;
    private GUIStyle labelSecondStyle;
    private GUIStyle buttonGenerateBridgeStyle;

    [MenuItem("Tools/Generator Props")]
    public static void ShowWindow()
    {
        // Create the UI
        GeneratorProps window = GetWindow<GeneratorProps>("Generator Props");
        window.minSize = new Vector2(400, 400);
        window.maxSize = new Vector2(400, 400);

        window.position = new Rect(100, 100, 400, 400);
    }

    private void SetupStyles()
    {
        labelStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(10, 10, 10, 5),
        };

        labelSecondStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(10, 10, 5, 5),
        };

        buttonGenerateBridgeStyle = new GUIStyle(GUI.skin.button)
        {
            margin = new RectOffset(30, 30, 10, 5),
            padding = new RectOffset(10, 10, 15, 15),
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            fontSize = 20
        };
    }

    private void OnGUI()
    {
        SetupStyles();

        GUILayout.Label("Generator Props", labelStyle);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Type");
        type = (Type)EditorGUILayout.EnumPopup("Type", type);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        if (type == Type.Bridge)
        {
            GUIBridge();
        }
        else if (type == Type.Hammer)
        {
            GUIHammer();
        }

        EditorGUILayout.EndScrollView();

        if(type == Type.Bridge)
        {
            EditorGUI.BeginDisabledGroup(prefabChain == null);
            if (GUILayout.Button("GENERATE", buttonGenerateBridgeStyle))
            {
                Generate();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
        }
        else if (type == Type.Hammer)
        {
            EditorGUI.BeginDisabledGroup(prefabChain == null || prefabBall == null);
            if (GUILayout.Button("GENERATE", buttonGenerateBridgeStyle))
            {
                Generate();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);
        }
    }

    private void GUIBridge()
    {
        GUILayout.Label("Parameters", labelSecondStyle);

        if (prefabChain == null)
        {
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Prefab Chain Bridge is Empty!", MessageType.Error);
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Prefab Chain Bridge");
        prefabChain = (GameObject)EditorGUILayout.ObjectField("Prefab Chain Bridge", prefabChain, typeof(GameObject), false);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Bridge Number");
        number = Mathf.Clamp(EditorGUILayout.IntField("Number", number), 1, 100);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Bridge Space");
        spacing = Mathf.Clamp(EditorGUILayout.FloatField("Spacing", spacing), 0, 100);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Bridge Health");
        health = Mathf.Clamp(EditorGUILayout.IntField("Health", health), 0, 1000000);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Damage By Speed Curve");
        damageBySpeedCurve = EditorGUILayout.CurveField("Damage By Speed", damageBySpeedCurve);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed RSE_OnPause");
        rseOnPause = (RSE_OnPause)EditorGUILayout.ObjectField("RSE On Pause", rseOnPause, typeof(RSE_OnPause), false);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed RSE_OnResume");
        rseOnResume = (RSE_OnResume)EditorGUILayout.ObjectField("RSE On Resume", rseOnResume, typeof(RSE_OnResume), false);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();
    }

    private void GenerateBridge()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        Camera sceneCamera = sceneView.camera;
        Vector3 spawnPosition = sceneCamera.transform.position + sceneCamera.transform.forward * 5f;

        List<GameObject> createdObjects = new List<GameObject>();

        // Create Parent
        GameObject bridgeParent = new GameObject("Bridge");
        Undo.RegisterCreatedObjectUndo(bridgeParent, "Create Bridge");
        bridgeParent.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0);
        bridgeParent.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        createdObjects.Add(bridgeParent);

        // Create Children
        for (int i = 0; i < number; i++)
        {
            Vector3 position = bridgeParent.transform.position - new Vector3(i * spacing, 0, 0);

            GameObject newSegment = Instantiate(prefabChain, position, Quaternion.identity);
            newSegment.transform.SetParent(bridgeParent.transform);
            newSegment.transform.Rotate(0, 0, 90);
            createdObjects.Add(newSegment);
        }

        // Create Hinge & Scripts
        for (int i = 0; i < createdObjects.Count; i++)
        {
            if (i > 0)
            {
                GameObject current = createdObjects[i];
                Rigidbody2D currentRb = current.GetComponent<Rigidbody2D>();
                Rigidbody2D previousRb = createdObjects[i - 1].GetComponent<Rigidbody2D>();

                current.AddComponent<HingeHealth>();
                current.AddComponent<HingeTrigger>();

                current.GetComponent<RigidbodyMotor>().SetScripts(rseOnPause, rseOnResume);

                current.GetComponent<HingeTrigger>().SetScript(current.GetComponent<HingeHealth>(), damageBySpeedCurve);
                current.GetComponent<HingeHealth>().maxHealth = health;

                current.GetComponent<HingeHealth>().SetHingeColor(current.GetComponent<SpriteRenderer>(), new Color32(168, 101, 38, 255), new Color32(168, 40, 38, 255));

                if (i == 1 || i == createdObjects.Count - 1)
                {
                    HingeJoint2D hinge = current.AddComponent<HingeJoint2D>();
                    hinge.connectedBody = createdObjects[0].GetComponent<Rigidbody2D>();
                    hinge.anchor = new Vector2(0, i == 1 ? -1 : 1);
                    hinge.autoConfigureConnectedAnchor = true;

                    current.GetComponent<HingeHealth>().SetHingeJoint(hinge);
                }

                if (i < createdObjects.Count - 1)
                {
                    GameObject next = createdObjects[i + 1];
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

    private void GUIHammer()
    {
        GUILayout.Label("Parameters", labelSecondStyle);

        if (prefabChain == null)
        {
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Prefab Chain Hammer is Empty!", MessageType.Error);
        }

        if (prefabBall == null)
        {
            GUILayout.Space(10);

            EditorGUILayout.HelpBox("Prefab Ball is Empty!", MessageType.Error);
        }

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Prefab Chain Hammer");
        prefabChain = (GameObject)EditorGUILayout.ObjectField("Prefab Chain Hammer", prefabChain, typeof(GameObject), false);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Prefab Ball Hammer");
        prefabBall = (GameObject)EditorGUILayout.ObjectField("Prefab Ball", prefabBall, typeof(GameObject), false);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Hammer Number");
        number = Mathf.Clamp(EditorGUILayout.IntField("Number", number), 1, 100);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Hammer Space");
        spacing = Mathf.Clamp(EditorGUILayout.FloatField("Spacing", spacing), 0, 100);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Hammer Space Ball");
        spacingBall = Mathf.Clamp(EditorGUILayout.FloatField("Spacing Ball", spacingBall), 0, 100);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Hammer Health");
        health = Mathf.Clamp(EditorGUILayout.IntField("Health", health), 0, 1000000);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Damage By Speed Curve");
        damageBySpeedCurve = EditorGUILayout.CurveField("Damage By Speed", damageBySpeedCurve);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Hammer Damage");
        damage = Mathf.Clamp(EditorGUILayout.IntField("Damage", damage), 0, 1000000);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed Hammer Type Damage");
        damageType = (DamageType)EditorGUILayout.EnumPopup("Type Damage", damageType);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed RSE_OnPause");
        rseOnPause = (RSE_OnPause)EditorGUILayout.ObjectField("RSE On Pause", rseOnPause, typeof(RSE_OnPause), false);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Space(30);

        Undo.RecordObject(this, "Changed RSE_OnResume");
        rseOnResume = (RSE_OnResume)EditorGUILayout.ObjectField("RSE On Resume", rseOnResume, typeof(RSE_OnResume), false);

        GUILayout.Space(30);
        GUILayout.EndHorizontal();
    }

    private void GenerateHammer()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        Camera sceneCamera = sceneView.camera;
        Vector3 spawnPosition = sceneCamera.transform.position + sceneCamera.transform.forward * 5f;

        List<GameObject> createdObjects = new List<GameObject>();

        // Create Parent
        GameObject bridgeParent = new GameObject("Hammer");
        Undo.RegisterCreatedObjectUndo(bridgeParent, "Create Hammer");
        bridgeParent.transform.position = new Vector3(spawnPosition.x, spawnPosition.y, 0);
        bridgeParent.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        createdObjects.Add(bridgeParent);

        // Create Children
        for (int i = 0; i < number; i++)
        {
            Vector3 position = bridgeParent.transform.position - new Vector3(0, i * spacing, 0);

            GameObject newSegment = Instantiate(prefabChain, position, Quaternion.identity);
            newSegment.transform.SetParent(bridgeParent.transform);
            newSegment.transform.Rotate(0, 0, 0);
            createdObjects.Add(newSegment);
        }

        Vector3 positionBall = bridgeParent.transform.position - new Vector3(0, number + spacingBall, 0);

        GameObject ball = Instantiate(prefabBall, positionBall, Quaternion.identity);
        ball.transform.SetParent(bridgeParent.transform);
        ball.transform.Rotate(0, 0, 0);
        createdObjects.Add(ball);

        // Create Hinge & Scripts
        for (int i = 0; i < createdObjects.Count; i++)
        {
            if (i > 0 && i < createdObjects.Count - 1)
            {
                GameObject current = createdObjects[i];
                Rigidbody2D currentRb = current.GetComponent<Rigidbody2D>();
                Rigidbody2D previousRb = createdObjects[i - 1].GetComponent<Rigidbody2D>();

                current.AddComponent<HingeHealth>();
                current.AddComponent<HingeTrigger>();

                current.GetComponent<RigidbodyMotor>().SetScripts(rseOnPause, rseOnResume);

                current.GetComponent<HingeTrigger>().SetScript(current.GetComponent<HingeHealth>(), damageBySpeedCurve);
                current.GetComponent<HingeHealth>().maxHealth = health;

                current.GetComponent<HingeHealth>().SetHingeColor(current.GetComponent<SpriteRenderer>(), new Color32(0, 0, 0, 255), new Color32(168, 40, 38, 255));

                if (i == 1)
                {
                    HingeJoint2D hinge = current.AddComponent<HingeJoint2D>();
                    hinge.connectedBody = createdObjects[0].GetComponent<Rigidbody2D>();
                    hinge.anchor = new Vector2(0f, 1);
                    hinge.autoConfigureConnectedAnchor = true;

                    current.GetComponent<HingeHealth>().SetHingeJoint(hinge);
                }

                if (i < createdObjects.Count - 2)
                {
                    GameObject next = createdObjects[i + 1];
                    Rigidbody2D nextRb = next.GetComponent<Rigidbody2D>();
                    if (nextRb == null) continue;

                    HingeJoint2D hinge = current.AddComponent<HingeJoint2D>();
                    hinge.connectedBody = nextRb;
                    hinge.anchor = current.transform.InverseTransformPoint((current.transform.position + next.transform.position) / 2);
                    hinge.autoConfigureConnectedAnchor = true;

                    current.GetComponent<HingeHealth>().SetHingeJoint(hinge);
                }

                if(i < createdObjects.Count - 1)
                {
                    GameObject next = createdObjects[i + 1];
                    Rigidbody2D nextRb = next.GetComponent<Rigidbody2D>();
                    if (nextRb == null) continue;

                    HingeJoint2D hinge = current.AddComponent<HingeJoint2D>();
                    hinge.connectedBody = nextRb;
                    hinge.anchor = new Vector2(0, -1);
                    hinge.autoConfigureConnectedAnchor = true;

                    current.GetComponent<HingeHealth>().SetHingeJoint(hinge);
                }
            }
            else if (i == createdObjects.Count - 1)
            {
                ball.GetComponent<Damagable>().SetScripts(damage, damageType);

                Damagable[] damagableComponents = ball.GetComponentsInChildren<Damagable>();

                foreach (Damagable damagable in damagableComponents)
                {
                    damagable.SetScripts(damage, damageType);
                }
            }
        }
    }

    private void Generate()
    {
        if (type == Type.Bridge)
        {
            GenerateBridge();
        }
        else if (type == Type.Hammer)
        {
            GenerateHammer();
        }
    }
}