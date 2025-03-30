using UnityEngine;

public class BlobAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SoundComponent dashSoundComponent;
    [SerializeField] private SoundComponent destroySoundComponent;
    [SerializeField] private SoundComponent deathSoundComponent;
    [SerializeField] private SoundComponent hitBlobSoundComponent;
    [SerializeField] private SoundComponent hitGroundSoundComponent;
    [SerializeField] private BlobDash blobDash;
    [SerializeField] private BlobMotor blobMotor;

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