using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThermostatController : MonoBehaviour
{

    LineRenderer lr;
    GameObject slider;
    Rigidbody2D rb_slider;
    AudioSource audioSource;

    public float a, b, c;

    public float temperature = 295.0f, minTemperature = 0.0f, maxTemperature = 1200.0f;
    public HeatSource[] heatSources;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        slider = transform.Find("Slider").gameObject;
        rb_slider = slider.GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        SliderJoint2D sliderJoint = slider.GetComponent<SliderJoint2D>();
        JointTranslationLimits2D limits = new JointTranslationLimits2D();
        //Set the limits of the slider joint based on the maximum and minimum allowed temperatures
        float height = Mathf.Lerp(-2.25f, 2.25f, ThermometerController.GetRelativeTemperature(minTemperature));
        limits.min = height;
        Transform block = transform.Find("Min Block");
        block.localPosition = new Vector3(block.localPosition.x, height - block.lossyScale.y, block.localPosition.z);
        if (minTemperature == 0.0f) Destroy(block.gameObject);
        height = Mathf.Lerp(-2.25f, 2.25f, ThermometerController.GetRelativeTemperature(maxTemperature));
        limits.max = height;
        block = transform.Find("Max Block");
        block.localPosition = new Vector3(block.localPosition.x, height + block.lossyScale.y, block.localPosition.z);
        if (maxTemperature == ThermometerController.GetMaxTemperature()) Destroy(block.gameObject);
        sliderJoint.limits = limits;

        sliderJoint.connectedAnchor = slider.transform.position;

        if (heatSources.Length > 0)
        {
            lr.SetPosition(1, heatSources[0].transform.position);
        }
        else
        {
            Debug.LogWarning("Thermostat (" + name + ") has no HeatSource assigned");
        }
        lr.SetPosition(0, transform.TransformPoint(Vector3.down * 3.05f));
        lr.sortingLayerName = "Door";
        lr.sortingOrder = 1;

        height = Mathf.Lerp(-2.25f, 2.25f, ThermometerController.GetRelativeTemperature(temperature));
        slider.transform.localPosition = new Vector3(slider.transform.localPosition.x, height, slider.transform.localPosition.z);

        if (GameObject.FindObjectOfType<WaterParticleController>() != null)
        {
            transform.Find("Water Marker").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                ThermometerController.GetRelativeTemperature(ParticleBond.meltingTemperatures[0]));
            transform.Find("Water Marker 2").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                ThermometerController.GetRelativeTemperature(ParticleBond.boilingTemperatures[0]));
        }
        else
        {
            Destroy(transform.Find("Water Marker").gameObject);//.SetActive(false);
            Destroy(transform.Find("Water Marker 2").gameObject);//.SetActive(false);
        }
        if (GameObject.FindObjectOfType<MetalParticleController>() != null)
        {
            transform.Find("Metal Marker").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                ThermometerController.GetRelativeTemperature(ParticleBond.meltingTemperatures[1]));
            transform.Find("Metal Marker 2").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                ThermometerController.GetRelativeTemperature(ParticleBond.boilingTemperatures[1]));
        }
        else
        {
            Destroy(transform.Find("Metal Marker").gameObject);//.SetActive(false);
            Destroy(transform.Find("Metal Marker 2").gameObject);//.SetActive(false);
        }
        if (GameObject.FindObjectOfType<CeramicParticleController>() != null)
        {
            transform.Find("Ceramic Marker").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                ThermometerController.GetRelativeTemperature(ParticleBond.meltingTemperatures[2]));
        }
        else
        {
            Destroy(transform.Find("Ceramic Marker").gameObject);//.SetActive(false);
        }

        a = 4.0f / (minTemperature * maxTemperature - Mathf.Pow(minTemperature + maxTemperature, 2.0f));
        b = -a * (minTemperature + maxTemperature);
        c = a * minTemperature * maxTemperature;
    }

    void Update()
    {
        temperature
            = ThermometerController.GetAbsoluteTemperature(Mathf.Lerp(0.0f, 1.0f, (slider.transform.localPosition.y / 4.5f) + 0.5f));
        if (heatSources != null)
        {
            foreach (HeatSource heatSource in heatSources)
            {
                heatSource.SetTemperature(temperature);
            }
        }
        float velocitySquared = rb_slider.velocity.sqrMagnitude;
        if (velocitySquared > 0.01f)
        {
            if (!audioSource.isPlaying) audioSource.Play();
            audioSource.volume = velocitySquared * 0.1f * (a * temperature * temperature + b * temperature + c);
            audioSource.pitch = 3.0f * ThermometerController.GetRelativeTemperature(temperature);
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (HeatSource heatSource in heatSources)
        {
            Gizmos.DrawSphere(heatSource.transform.position, 0.1f);
        }
    }
}
