using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleManager : MonoBehaviour {

    public static ParticleManager instance;

    public static List<ParticleController> particles;
    public static Queue<KeyValuePair<ParticleController, ParticleController>> bondsToMake;

    public static LayerMask layerMask;

    public float totalEnergy;
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

            layerMask = LayerMask.NameToLayer("PArticle");
        }
        else
        {
            Destroy(gameObject);
        }
	}
	
	void FixedUpdate()
    {
        foreach (ParticleController particle in particles)
        {
            particle.EarlyFixedUpdate();
        }
        foreach (ParticleController particle in particles)
        {
            particle.LateFixedUpdate();//This gets funny if particles get deleted, because they get removed from the list
        }
        for (int n = particles.Count - 1; n >= 0; n--)
        {
            if (particles[n].temperature > 3000.0f || particles[n].transform.position.sqrMagnitude > 2500.0f)
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
        /*else if (updates < 30)
        {
            updates++;
            foreach (ParticleController particle in particles)
            {
                particle.thermalEnergy = 0.0f;
            }
        }*/

        totalEnergy = 0.0f;
        foreach (GameObject go_particle in GameObject.FindGameObjectsWithTag("Particle"))
        {
            totalEnergy += go_particle.GetComponent<ParticleController>().totalEnergy;
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
