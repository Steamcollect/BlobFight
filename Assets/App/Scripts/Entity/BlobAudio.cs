using UnityEngine;
public class BlobAudio : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] SoundComponent dashSoundComponent;
    [SerializeField] SoundComponent destroySoundComponent;
    [SerializeField] SoundComponent deathSoundComponent;
    [SerializeField] SoundComponent hitBlobSoundComponent;
    [SerializeField] SoundComponent hitGroundSoundComponent;
    [Space(20)]
    [SerializeField] BlobDash blobDash;
    [SerializeField] BlobMotor blobMotor;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    private void OnEnable()
    {
        blobDash.OnDash += PlayDashClip;
        blobMotor.GetHealth().onDeath += PlayDeathClip;
        blobMotor.GetHealth().OnDestroyBlob += PlayDestroyClip;
        blobMotor.GetTrigger().OnGroundTouch += PlayHitGroundClip;
        blobMotor.GetCombat().OnHitBlob += PlayHitBlobClip;
    }
    private void OnDisable()
    {
        blobDash.OnDash -= PlayDashClip;
        blobMotor.GetHealth().onDeath -= PlayDeathClip;
        blobMotor.GetHealth().OnDestroyBlob -= PlayDestroyClip;
        blobMotor.GetTrigger().OnGroundTouch -= PlayHitGroundClip;
        blobMotor.GetCombat().OnHitBlob -= PlayHitBlobClip;
    }
    private void PlayDashClip()
    {
        dashSoundComponent.PlayClip();
    }
    private void PlayDestroyClip()
    {
        destroySoundComponent.PlayClip();
    }
    private void PlayDeathClip()
    {
        deathSoundComponent.PlayClip();
    }
    private void PlayHitBlobClip(float impactForce)
    {
        hitBlobSoundComponent.PlayClip();
    }
    private void PlayHitGroundClip()
    {
        hitGroundSoundComponent.PlayClip();
    }
}