using System.Collections.Generic;
using UnityEngine;
public class RotatingProps : GameProps
{
    [Header("Settings")]
    [SerializeField] float rotationSpeed;
    bool isLaunched = false;
    [SerializeField] List<int> timeSpeed;
    [SerializeField] List<float> newtimeSpeed;

    [Header("Output")]
    [SerializeField] RSO_TimerParty rsoTimerParty;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]

    int mode = 0;

    public override void Launch()
    {
        isLaunched = true;
    }

    private void Update()
    {
        if (!isLaunched) return;

        if (rsoTimerParty.Value >= timeSpeed[mode] && mode < timeSpeed.Count)
        {
            rotationSpeed = newtimeSpeed[mode];

            if (mode < timeSpeed.Count - 1)
            {
                mode++;
            }
        }

        float zRot = transform.rotation.eulerAngles.z;
        transform.rotation = Quaternion.Euler(0,0, zRot + rotationSpeed * Time.deltaTime);
    }
}