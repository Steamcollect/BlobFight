using UnityEngine;
public class BlobCombat : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] BlobTrigger blobTrigger;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void OnDisable()
    {
        blobTrigger.OnCollisionEnterWithBlob -= OnEnterCollision;
    }

    private void Start()
    {
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        blobTrigger.OnCollisionEnterWithBlob += OnEnterCollision;
    }

    void OnEnterCollision(BlobMotor blob)
    {
        print(blob.joint.GetVelocity());
    }
}