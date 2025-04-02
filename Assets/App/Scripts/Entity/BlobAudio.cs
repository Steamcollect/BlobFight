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
    [Space(10)]
    [SerializeField] private SoundComponent touchGrassSC;
    [SerializeField] private SoundComponent touchStoneSC;
    [SerializeField] private SoundComponent touchMetalSC;

    private void OnEnable()
    {
        blobDash.OnDash += PlayDashClip;
        blobMotor.GetCombat().OnHitBlob += PlayHitFromBlobClip;
    }

    private void OnDisable()
    {
        blobDash.OnDash -= PlayDashClip;
        blobMotor.GetCombat().OnHitBlob -= PlayHitFromBlobClip;
    }

    private void PlayDashClip()
    {
        dashSC.PlayClip();
    }
    #region Death
    public void PlayDeathFromLavaClip()
    {
        deathFromLavaSC.PlayClip();
    }

    public void PlayDeathFromVoidClip()
    {
        deathFromVoidSC.PlayClip();
    }
    #endregion
    #region Hit
    public void PlayHitFromLavaClip()
    {

    }
    public void PlayHitFromThunderClip()
    {

    }
    public void PlayHitFromLaserClip()
    {

    }
    public void PlayHitFromBrumbleClip()
    {

    }
    private void PlayHitFromBlobClip(float impactForce)
    {
    }
    #endregion
    #region Touch
    public void PlayTouchGrassClip()
    {
        touchGrassSC.PlayClip();
    }
    public void PlayTouchStoneClip()
    {
        touchStoneSC.PlayClip();
    }
    public void PlayTouchMetalClip()
    {
        touchMetalSC.PlayClip();
    }
    #endregion
}