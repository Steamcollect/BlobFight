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
    [Space(10)]
    // RSO
    [SerializeField] RSO_BlobInGame RSO_BlobInGame;
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    private void OnEnable()
    {
        rSE_CallRumble.action += RumbleController;
    }
    private void OnDisable()
    {
        rSE_CallRumble.action -= RumbleController;
    }
    private void RumbleController(int index, string currentSche)
    {
        if(currentSche == "Controller")
        {
            var devices = InputSystem.devices;

            foreach (var device in devices)
            {
                if (device is Gamepad pad)
                {
					foreach (var blob in RSO_BlobInGame.Value)
					{
                        /*if (blob.GetComponent<PlayerInput>(). == pad)
						{
							pad.SetMotorSpeeds(lowRumbleForce, highRumbleForce);
							StartCoroutine(DelayRumble(rumbleDuration, pad));
						}*/
					}
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