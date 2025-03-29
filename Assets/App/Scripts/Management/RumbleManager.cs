using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lowRumbleForce;
    [SerializeField] private float highRumbleForce;
    [SerializeField] private float rumbleDuration;

    [Header("Input")]
    [SerializeField] private RSE_CallRumble rseCallRumble;
    [SerializeField] private RSE_OnGameStart rseOnGameStart;

    [Header("Output")]
    [SerializeField] private RSO_BlobInGame rsoBlobInGame;

    private List<int> listIndex = new();

    private void OnEnable()
    {
        rseCallRumble.action += RumbleController;
        rseOnGameStart.action += Init;
    }

    private void OnDisable()
    {
        rseCallRumble.action -= RumbleController;
        rseOnGameStart.action -= Init;
    }

    private void Init()
    {
        for (int i = 0; i < rsoBlobInGame.Value.Count; i++) 
        {
            listIndex.Add(i);
        }
    }

    private void RumbleController(int index, string currentSche)
    {
        if (currentSche == "Controller" && index < rsoBlobInGame.Value.Count)
        {
            var devices = InputSystem.devices;

            foreach (var device in devices)
            {
                if (device is Gamepad pad && index == listIndex[index])
                {
                    pad.SetMotorSpeeds(lowRumbleForce, highRumbleForce);

                    StartCoroutine(DelayRumble(rumbleDuration, pad));
                }
            }
        }
    }
    private IEnumerator DelayRumble(float duration, Gamepad pad)
    {
        yield return new WaitForSeconds(duration);

        pad.SetMotorSpeeds(0f, 0f);
    }
}