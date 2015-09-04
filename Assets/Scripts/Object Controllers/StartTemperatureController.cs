using UnityEngine;
using System.Collections;

public class StartTemperatureController : MonoBehaviour {

    public float startTemperature = 295.0f;
    int updates = 0;

    void Start()
    {
        GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    void FixedUpdate()
    {
        updates++;
        if (updates > 10)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Particle")
        {
            collider.GetComponent<ParticleController>().startTemperature = startTemperature;
        }
    }

}
