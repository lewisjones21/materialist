using UnityEngine;
using System.Collections;

public class HeatSource : MonoBehaviour {

    public float rate = 1.0f, temperature = 295.0f;

    void Start()
    {
        SetColour();
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Particle")
        {
            ParticleController particle = collider.GetComponent<ParticleController>();
            particle.thermalEnergy += (temperature - particle.temperature) * rate * Time.fixedDeltaTime;
        }
    }

    public void SetTemperature(float newTemperature)
    {
        temperature = newTemperature;
        SetColour();
    }

    void SetColour()
    {
        Color tempColour = ThermometerController.GetColor(temperature);
        GetComponent<SpriteRenderer>().color = tempColour;
        transform.FindChild("Aura").GetComponent<SpriteRenderer>().color = new Color(tempColour.r, tempColour.g, tempColour.b, 0.2f);
        //GetComponent<Halo>().color = new Color(tempColour.r, tempColour.g, tempColour.b, 0.2f);
    }
}
