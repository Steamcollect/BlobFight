using UnityEngine;
public class InstablePlatform : GameProps
{
    //[Header("Settings")]
    bool canRotate = false;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public override void Launch()
    {
        canRotate = true;
    }

    private void FixedUpdate()
    {
        
    }
}