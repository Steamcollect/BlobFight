using System;
using UnityEngine;

public class BlobAudio : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxHitStrenght;
    [Header("References")]
    [SerializeField] private BlobDash blobDash;
    [SerializeField] private BlobMotor blobMotor;
    [Space(10)]
    [SerializeField] private SoundComponent dashSC;
    [Space(10)]
    [SerializeField] private SoundComponent parrySC;
    [SerializeField] private SoundComponent parryHitSC;
    [Space(10)]
    [SerializeField] private SoundComponent deathFromLavaSC;
    [SerializeField] private SoundComponent deathFromVoidSC;
    [Space(10)]
    [SerializeField] private HitSound[] hitFromBlobSound;
    [SerializeField] private SoundComponent hitFromBlobExtandSC;
    [SerializeField] private SoundComponent hitFromLavaSC;
    [SerializeField] private SoundComponent hitFromParrySC;
    [SerializeField] private SoundComponent hitFromLaserSC;
    [SerializeField] private SoundComponent hitFromThunderSC;
    [SerializeField] private SoundComponent hitFromBrumbleSC;
    [Space(10)]
    [SerializeField] private SoundComponent touchGrassSC;
    [SerializeField] private SoundComponent touchStoneSC;
    [SerializeField] private SoundComponent touchMetalSC;

    private void OnEnable()
    {
        blobDash.OnDash += PlayDashClip;
    }

    private void OnDisable()
    {
        blobDash.OnDash -= PlayDashClip;
    }

    private void PlayDashClip()
    {
        dashSC.PlayClip();
    }
    public void PlayParrySound()
    {
        parrySC.PlayClip();
    }
    public void PlayParryHitSound()
    {
        parryHitSC.PlayClip();
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
    [Serializable]
    private class HitSound
    {
        [Range(0, 100)] public float hitStrenght = 0;
        public SoundComponent hitSoundComponent = null;

        public void PlayHitSound()
        {
            hitSoundComponent.PlayClip();
        }
    }
    public void PlayHitFromParrySound()
    {
        hitFromParrySC.PlayClip();
    }
    public void PlayHitFromLavaClip()
    {
        hitFromLavaSC.PlayClip();
    }
    public void PlayHitFromThunderClip()
    {
        hitFromThunderSC.PlayClip();
    }
    public void PlayHitFromLaserClip()
    {
        hitFromLaserSC.PlayClip();
    }
    public void PlayHitFromBrumbleClip()
    {
        hitFromBrumbleSC.PlayClip();
    }
    public void PlayHitFromBlobExtandClip()
    {
        hitFromBlobExtandSC.PlayClip();
    }
    public void PlayHitFromBlobClip(float strenght)
    {
        if(hitFromBlobSound.Length <= 0) return;

        float strenghtPercent = Mathf.Clamp(strenght / maxHitStrenght * 100, 0, 100);
        for (int i = 0; i < hitFromBlobSound.Length; i++)
        {
            if (hitFromBlobSound[i].hitStrenght >= strenghtPercent)
            {
                hitFromBlobSound[i].PlayHitSound();
                break;
            }
        }
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