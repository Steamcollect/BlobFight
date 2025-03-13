using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BlobHitParticle : ParticleCallback
{
    [SerializeField] ParticleSystem circleParticle;
    MainModule circleMain;

    [SerializeField] ParticleSystem impactParticle;
    MainModule impactMain;

    [SerializeField] ParticleSystem dustParticle;
    MainModule dustMain;

    private void Awake()
    {
        circleMain = circleParticle.main;
        impactMain = impactParticle.main;
        dustMain = dustParticle.main;
    }

    public override void SetColor(Color newColor)
    {
        circleMain.startColor = newColor;
        impactMain.startColor = newColor;
        dustMain.startColor = newColor;
    }
}