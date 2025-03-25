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

    [Header("Input")]
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]

    int mode = 0;
    bool isPaused = false;

    private new void OnEnable()
    {
        base.OnEnable();
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }
    private new void OnDisable()
    {
        base.OnDisable();
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
    }

    public override void Launch()
    {
        isLaunched = true;
    }

    private void Pause()
    {
        isPaused = true;
    }

    private void Resume()
    {
        isPaused = false;
    }

    private void Update()
    {
        if (!isPaused)
        {
            if (!isLaunched) return;

            if (timeSpeed.Count > 0)
            {
                if (rsoTimerParty.Value >= timeSpeed[mode] && mode < timeSpeed.Count)
                {
                    rotationSpeed = newtimeSpeed[mode];

                    if (mode < timeSpeed.Count - 1)
                    {
                        mode++;
                    }
                }
            }

            float zRot = transform.rotation.eulerAngles.z;
            transform.rotation = Quaternion.Euler(0, 0, zRot + rotationSpeed * Time.deltaTime);
        }
    }
}