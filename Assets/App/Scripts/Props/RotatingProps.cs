using UnityEngine;
public class RotatingProps : GameProps
{
    [Header("Settings")]
    [SerializeField] float rotationSpeed;
    bool isLaunched = false;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public override void Launch()
    {
        isLaunched = true;
    }

    private void Update()
    {
        if (isLaunched) return;

        float zRot = transform.rotation.eulerAngles.z;
        transform.rotation = Quaternion.Euler(0,0, zRot + rotationSpeed * Time.deltaTime);
    }
}