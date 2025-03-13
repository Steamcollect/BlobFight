using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ParticleColorSettable : ParticleCallback
{
    [SerializeField] ParticleSystem[] particles;
    MyParticle[] myParticles;

    struct MyParticle
    {
        public ParticleSystem particle;
        public MainModule mainModule;

        public MyParticle(ParticleSystem particle, MainModule mainModule)
        {
            this.particle = particle;
            this.mainModule = mainModule;
        }
    }

    private void Awake()
    {
        myParticles = new MyParticle[particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            myParticles[i] = new MyParticle(particles[i], particles[i].main);
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