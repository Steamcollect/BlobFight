using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BlobParticleCallback : ParticleCallback
{
    [SerializeField] ParticleSystem[] particles;
    MyParticle[] myParticles;
    struct MyParticle
    {
        public ParticleSystem particle;
        public MainModule mainModule;
        public ParticleSystemRenderer psr;

        public MyParticle(ParticleSystem particle, MainModule mainModule, Material trailMat)
        {
            this.particle = particle;
            this.mainModule = mainModule;
            psr = particle.GetComponent<ParticleSystemRenderer>();
            psr.trailMaterial = trailMat;
        }
    }

    private void Awake()
    {
        myParticles = new MyParticle[particles.Length];
        Material trailMat = new Material(Shader.Find("Sprites/Default"));

        for (int i = 0; i < particles.Length; i++)
        {
            myParticles[i] = new MyParticle(particles[i], particles[i].main, trailMat);
        }
    }

    public override void SetColor(Color newColor)
    {
        for (int i = 0; i < myParticles.Length; i++)
        {
            myParticles[i].mainModule.startColor = newColor;
        }
    }
}