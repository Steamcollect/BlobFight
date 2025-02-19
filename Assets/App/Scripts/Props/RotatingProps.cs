using UnityEngine;
public class RotatingProps : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float rotationSpeed;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Update()
    {
        float zRot = transform.rotation.eulerAngles.z;
        transform.rotation = Quaternion.Euler(0,0, zRot + rotationSpeed * Time.deltaTime);
    }
}