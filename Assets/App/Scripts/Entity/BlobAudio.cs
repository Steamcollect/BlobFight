using UnityEngine;

public class BlobAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlobDash blobDash;
    [SerializeField] private BlobMotor blobMotor;
    [Space(10)]
    [SerializeField] private SoundComponent dashSC;
    [Space(10)]
    [SerializeField] private SoundComponent deathFromLavaSC;
    [SerializeField] private SoundComponent deathFromVoidSC;
    [Space(10)]
    [SerializeField] private SoundComponent hitFromParrySC;

    private void OnEnable()
    {
        blobDash.OnDash += PlayDashClip;
        blobMotor.GetTrigger().OnGroundTouch += PlayHitGroundClip;
        blobMotor.GetCombat().OnHitBlob += PlayHitFromBlobClip;
    }

    private void OnDisable()
    {
        blobDash.OnDash -= PlayDashClip;
        blobMotor.GetTrigger().OnGroundTouch -= PlayHitGroundClip;
        blobMotor.GetCombat().OnHitBlob -= PlayHitFromBlobClip;
    }

    private void PlayDashClip()
    {
        dashSC.PlayClip();
    }

    private void PlayDeathFromLavaClip()
    {
        deathFromLavaSC.PlayClip();
    }

    private void PlayDeathFromVoidClip()
    {
        deathFromVoidSC.PlayClip();
    }

    private void PlayHitFromBlobClip(float impactForce)
    {
    }

    private void PlayHitGroundClip()
    {
    }
}