using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThermometerController : MonoBehaviour {

    static KeyValuePair<float, Color>[] temperatureList = new KeyValuePair<float, Color>[6] {
        new KeyValuePair<float, Color>(0.0f, Color.Lerp(Color.cyan, Color.blue, 0.8f)),
        new KeyValuePair<float, Color>(150.0f, Color.Lerp(Color.cyan, Color.blue, 0.3f)),
        new KeyValuePair<float, Color>(300.0f, Color.Lerp(Color.cyan, Color.grey, 0.8f)),
        new KeyValuePair<float, Color>(550.0f, Color.yellow),
        new KeyValuePair<float, Color>(950.0f, Color.red),
        new KeyValuePair<float, Color>(1200.0f, Color.white),
    };

    LineRenderer lr;

    public float temperature = 295.0f;
    public Transform checkArea;

	void Start()
    {
        lr = GetComponent<LineRenderer>();
        LineRenderer lrChild = transform.FindChild("Check Position").GetComponent<LineRenderer>();
        lrChild.SetPosition(0, -lrChild.transform.localPosition + Vector3.down * 2.25f);
        lrChild.sortingLayerName = "Door";
        lrChild.sortingOrder = 1;

        bool hasWater = (GameObject.FindObjectOfType<WaterParticleController>() != null);
        bool hasMetal = (GameObject.FindObjectOfType<MetalParticleController>() != null);
        bool hasCeramic = (GameObject.FindObjectOfType<CeramicParticleController>() != null);

        CannonController[] cannons = GameObject.FindObjectsOfType<CannonController>();
        foreach (CannonController cannon in cannons)
        {
            switch (cannon.particleType)
            {
                case (0):
                    hasWater = true;
                    break;
                case (1):
                    hasMetal = true;
                    break;
                case (2):
                    hasCeramic = true;
                    break;
            }
        }

        if (hasWater)
        {
            transform.FindChild("Water Marker").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                GetRelativeTemperature(ParticleBond.meltingTemperatures[0]));
            transform.FindChild("Water Marker 2").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                GetRelativeTemperature(ParticleBond.boilingTemperatures[0]));
        }
        else
        {
            transform.FindChild("Water Marker").gameObject.SetActive(false);
            transform.FindChild("Water Marker 2").gameObject.SetActive(false);
        }
        if (hasMetal)
        {
            transform.FindChild("Metal Marker").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                GetRelativeTemperature(ParticleBond.meltingTemperatures[1]));
            transform.FindChild("Metal Marker 2").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                GetRelativeTemperature(ParticleBond.boilingTemperatures[1]));
        }
        else
        {
            transform.FindChild("Metal Marker").gameObject.SetActive(false);
            transform.FindChild("Metal Marker 2").gameObject.SetActive(false);
        }
        if (hasCeramic)
        {
            transform.FindChild("Ceramic Marker").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                GetRelativeTemperature(ParticleBond.meltingTemperatures[2]));
        }
        else
        {
            transform.FindChild("Ceramic Marker").gameObject.SetActive(false);
        }
	}
	
	void Update()
    {
        UpdateTemperature(checkArea.position);
        Vector3 height = Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f, GetRelativeTemperature(temperature));
        lr.SetPosition(0, height + Vector3.left * 0.4f);
        lr.SetPosition(1, height + Vector3.right * 0.4f);
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(checkArea.position, 0.1f);
    }

    void UpdateTemperature(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.5f);
        if (colliders != null)
        {
            int particleNumber = 0;
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "HeatSource")
                {
                    temperature = collider.GetComponent<HeatSource>().temperature;
                    return;
                }
                if (collider.tag == "Cannon")
                {
                    temperature = collider.GetComponent<CannonController>().temperature;
                    return;
                }
                if (collider.tag == "Particle")
                {
                    if (particleNumber == 0) temperature = 0.0f;
                    temperature += collider.GetComponent<ParticleController>().temperature;
                    particleNumber++;
                }
            }
            if (particleNumber > 0)
            {
                temperature /= particleNumber;
            }
        }
    }

    public static Color GetColor(float temperature)
    {
        for (int n = 0; n < temperatureList.Length - 1; n++)
        {
            if (temperature > temperatureList[n + 1].Key) continue;
            return Color.Lerp(temperatureList[n].Value, temperatureList[n + 1].Value,
                (temperature - temperatureList[n].Key) / (temperatureList[n + 1].Key - temperatureList[n].Key));
        }
        return temperatureList[temperatureList.Length - 1].Value;
    }

    public static float GetRelativeTemperature(float temperature)
    {
        return temperature / temperatureList[temperatureList.Length - 1].Key;
    }
    public static float GetAbsoluteTemperature(float relativeTemperature)
    {
        return relativeTemperature * temperatureList[temperatureList.Length - 1].Key;
    }

    public static float GetMaxTemperature()
    {
        return temperatureList[temperatureList.Length - 1].Key;
    }
}
