using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float lowRumbleForce;
    [SerializeField] float highRumbleForce;
    [SerializeField] float rumbleDuration;

    [Header("References")]
    [SerializeField] RSE_CallRumble rSE_CallRumble;
    [SerializeField] RSE_OnGameStart rseOnGameStart;
    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame RSO_BlobInGame;
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private List<int> listindex = new();

    private void OnEnable()
    {
        rSE_CallRumble.action += RumbleController;
        rseOnGameStart.action += Init;
    }
    private void OnDisable()
    {
        rSE_CallRumble.action -= RumbleController;
        rseOnGameStart.action -= Init;
    }

    private void Init()
    {
        for (int i = 0; i < RSO_BlobInGame.Value.Count; i++) 
        {
            listindex.Add(i);
        }
    }

    private void RumbleController(int index, string currentSche)
    {
        if (currentSche == "Controller")
        {
            var devices = InputSystem.devices;

            foreach (var device in devices)
            {
                if (device is Gamepad pad && index == listindex[index])
                {
                    pad.SetMotorSpeeds(lowRumbleForce, highRumbleForce);
                    StartCoroutine(DelayRumble(rumbleDuration, pad));
                }
            }
        }
    }
    IEnumerator DelayRumble(float duration, Gamepad pad)
    {
        yield return new WaitForSeconds(duration);
        pad.SetMotorSpeeds(0f, 0f);
    }
}