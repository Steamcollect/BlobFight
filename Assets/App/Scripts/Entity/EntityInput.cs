using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInput : MonoBehaviour
{
    public Input[] inputs;
    Dictionary<string, Input> outputs = new();

    private void Awake()
    {
        if(inputs.Length == 0)
        {
            Debug.LogError($"{gameObject.name} contain \"EntityInput\" but havnt any input on it");
            return;
        }
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i].inputName == "")
            {
                Debug.LogError($"An input in {gameObject.name} havnt any name");
                continue;
            }
            outputs.Add(inputs[i].inputName, inputs[i]);
        }
    }

    private void Update()
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            switch (inputs[i].inputType)
            {
                case InputType.Key:
                    HandleKeyInput(ref inputs[i]);
                    break;
                case InputType.Float:
                    HandleFloatInput(ref inputs[i]);
                    break;
                case InputType.Vector2:
                    HandleVector2Input(ref inputs[i]);
                    break;
            }
        }
    }

    public Input GetInput(string inputName)
    {
        if (!outputs.ContainsKey(inputName))
        {
            Debug.LogError($"There is no input of name \"{inputName}\"");
            return null;
        }
        return outputs[inputName];
    }

    void HandleKeyInput(ref Input input)
    {
        bool isPressed = false;
        foreach (KeyCode key in input.keys)
        {
            if (UnityEngine.Input.GetKeyDown(key)) input.OnKeyDown?.Invoke();
            if (UnityEngine.Input.GetKey(key)) isPressed = true;
            if (UnityEngine.Input.GetKeyUp(key)) input.OnKeyUp?.Invoke();
        }

        if (isPressed)
        {
            input.OnPress?.Invoke();
        }

        input.OnUpdateKey?.Invoke(isPressed);
    }
    void HandleFloatInput(ref Input input)
    {
        if (input.keys.Length < 2) return;

        float value = 0;
        if (UnityEngine.Input.GetKey(input.keys[0])) value = -1;
        if (UnityEngine.Input.GetKey(input.keys[1])) value = 1;

        input.OnUpdateFloat?.Invoke(value);
    }
    void HandleVector2Input(ref Input input)
    {
        if (input.keys.Length < 4) return;

        Vector2 direction = Vector2.zero;
        if (UnityEngine.Input.GetKey(input.keys[0])) direction.y = 1; // Haut
        if (UnityEngine.Input.GetKey(input.keys[1])) direction.y = -1; // Bas
        if (UnityEngine.Input.GetKey(input.keys[2])) direction.x = -1; // Gauche
        if (UnityEngine.Input.GetKey(input.keys[3])) direction.x = 1; // Droite

        input.OnUpdateVector2?.Invoke(direction);
    }

    void OnValidate()
    {
        if(inputs.Length == 0) return;

        for (int i = 0; i < inputs.Length; i++)
        {
            int inputCount = 1;
            switch (inputs[i].inputType)
            {
                case InputType.Key:
                    inputCount = 1;
                    break;
                case InputType.Float:
                    inputCount = 2;
                    break;
                case InputType.Vector2:
                    inputCount = 4;
                    break;
            }

            // Si la taille est déjà correcte, ne rien faire
            if (inputs[i].keys != null && inputs[i].keys.Length == inputCount)
                continue;

            KeyCode[] newKeys = new KeyCode[inputCount];

            // Copier les anciennes valeurs si possible
            for (int j = 0; j < Mathf.Min(inputs[i].keys?.Length ?? 0, inputCount); j++)
            {
                newKeys[j] = inputs[i].keys[j];
            }

            // Assigner le nouveau tableau
            inputs[i].keys = newKeys;
        }
    }
}

[Serializable]
public class Input
{
    public string inputName;
    public InputType inputType;
    public KeyCode[] keys;

    public Action OnKeyDown;
    public Action OnPress;
    public Action OnKeyUp;
    public Action OnChange;
    public Action<bool> OnUpdateKey;
    public Action<float> OnUpdateFloat;
    public Action<Vector2> OnUpdateVector2;
}

public enum InputType
{
    Key,     // une touche
    Float,   // 2 touches (ex: axe horizontal)
    Vector2  // 4 touches (ex: déplacement en 2D)
}
