using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour {

    public static ParticleManager instance;

    public static List<ParticleController> particles;
    public static Queue<KeyValuePair<ParticleController, ParticleController>> bondsToMake;

    public static LayerMask layerMask;

    AudioSource audioSourceFlow, audioSourceSplash;

    public float totalEnergy;
    public float totalVelocitySquared, totalDeltaVelocitySquared;
    int updates = 0;

	void Start()
    {
        if (Application.loadedLevelName.Contains("Screen"))
        {
            Destroy(gameObject);
            return;
        }
        if (instance == null)
        {
            instance = this;
            particles = new List<ParticleController>();
            GameObject[] go_particles = GameObject.FindGameObjectsWithTag("Particle");
            foreach (GameObject go_particle in go_particles)
            {
                particles.Add(go_particle.GetComponent<ParticleController>());
            }
            bondsToMake = new Queue<KeyValuePair<ParticleController, ParticleController>>();

            layerMask = LayerMask.GetMask("Particle");

            AudioSource[] sources = GetComponents<AudioSource>();
            audioSourceFlow = sources[0];
            audioSourceSplash = sources[1];
            Debug.Log("Particle Manager initialised with " + particles.Count + " particles");
        }
        else
        {
            Destroy(gameObject);
        }
	}

    void Update()
    {
        totalVelocitySquared = Mathf.Max(0.0f, totalVelocitySquared);
        audioSourceFlow.volume = Mathf.Sqrt(totalVelocitySquared) * 0.0075f;
        if (audioSourceFlow.volume > 0.01f)
        {
            if (!audioSourceFlow.isPlaying) audioSourceFlow.Play();
        }
        else if (audioSourceFlow.isPlaying)
        {
            audioSourceFlow.Stop();
        }
        audioSourceFlow.pitch = audioSourceFlow.pitch * 0.9f + 0.1f;
        audioSourceFlow.panStereo *= 0.975f;
        audioSourceFlow.panStereo = Mathf.Clamp(audioSourceFlow.panStereo, -0.8f, 0.8f);

        audioSourceSplash.volume = audioSourceSplash.volume * 0.7f
            + (Mathf.Sqrt(totalDeltaVelocitySquared) - 7.0f) * 0.03f;//10*0.2
        //audioSourceSplash.volume = (Mathf.Sqrt(totalDeltaVelocitySquared) - 7.0f) * 0.1f;
        if (audioSourceSplash.volume > 0.01f)
        {
            if (!audioSourceSplash.isPlaying) audioSourceSplash.Play();
        }
        else if (audioSourceSplash.isPlaying)
        {
            audioSourceSplash.Stop();
        }
        audioSourceSplash.panStereo *= 0.975f;
        audioSourceSplash.panStereo = Mathf.Clamp(audioSourceSplash.panStereo, -0.8f, 0.8f);
    }
	
	void FixedUpdate()
    {
        totalVelocitySquared = 0.0f;
        totalDeltaVelocitySquared = 0.0f;
        foreach (ParticleController particle in particles)
        {
            particle.EarlyFixedUpdate();
        }
        float velocitySquared, deltaVelocitySquared;
        foreach (ParticleController particle in particles)
        {
            particle.LateFixedUpdate();//This gets funny if particles get deleted, because they get removed from the list
            if (particle.temperature < ParticleBond.boilingTemperatures[particle.type])
            {
                velocitySquared = particle.rb.velocity.sqrMagnitude;
                totalVelocitySquared += velocitySquared - 1.0f;
                audioSourceFlow.pitch *= (particle.type == 0 ? 1.0f : 0.997f);
                audioSourceFlow.panStereo += (particle.rb.position.x - Camera.main.transform.position.x) * 0.001f * velocitySquared;
                //if (Mathf.Abs(particle.lastVelocitySquared - velocitySquared) > 2.0f)
                deltaVelocitySquared = Mathf.Abs(particle.lastVelocitySquared - velocitySquared);
                totalDeltaVelocitySquared += deltaVelocitySquared;
                particle.lastVelocitySquared = velocitySquared;
                audioSourceSplash.panStereo += (particle.rb.position.x - Camera.main.transform.position.x) * 0.0001f * deltaVelocitySquared;
            }
        }
        for (int n = particles.Count - 1; n >= 0; n--)
        {
            if (particles[n].temperature > 3000.0f || particles[n].transform.position.sqrMagnitude > 2500.0f || particles[n].shouldDelete)
            {
                particles[n].Delete();
            }
        }

        if (bondsToMake.Count > 0)
        {
            StartCoroutine("Connect");
        }
        if (bondsToMake.Count == 0)
        {
            StopCoroutine("Connect");
        }

        if (updates < 15)
        {
            updates++;
            foreach (ParticleController particle in particles)
            {
                particle.SetTemperature();//ParticleBond.[particle.type] * 1.5f;
            }
        }

        totalEnergy = 0.0f;
        foreach (ParticleController particle in particles)
        {
            totalEnergy += particle.totalEnergy;
        }
	}

    public static void QueueToConnect(ParticleController one, ParticleController two)
    {
        //bondsToMake.Enqueue(new KeyValuePair<ParticleController, ParticleController>(one, two));
        ParticleBondManager.RetrieveBond().Attach(one, two);
    }

    IEnumerator Connect()
    {
        int bondsMade = 0;
        while (bondsToMake.Count > 0)
        {
            KeyValuePair<ParticleController, ParticleController> kvp = bondsToMake.Dequeue();
            ParticleBondManager.RetrieveBond().Attach(kvp.Key, kvp.Value);
            if (bondsMade > bondsToMake.Count * 0.2f)
            {
                bondsMade = 0;
                yield return null;
            }
        }
        yield return null;
    }
}
