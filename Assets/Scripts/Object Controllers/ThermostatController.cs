using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThermostatController : MonoBehaviour
{

    LineRenderer lr;
    GameObject slider;

    public float temperature = 295.0f, minTemperature = 0.0f, maxTemperature = 1200.0f;
    public HeatSource[] heatSources;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        slider = transform.FindChild("Slider").gameObject;
        SliderJoint2D sliderJoint = slider.GetComponent<SliderJoint2D>();
        JointTranslationLimits2D limits = new JointTranslationLimits2D();
        //Set the limits of the slider joint based on the maximum and minimum allowed temperatures
        float height = Mathf.Lerp(-2.25f, 2.25f, ThermometerController.GetRelativeTemperature(minTemperature));
        limits.min = height;
        Transform block = transform.FindChild("Min Block");
        block.localPosition = new Vector3(block.localPosition.x, height - block.lossyScale.y, block.localPosition.z);
        if (minTemperature == 0.0f) Destroy(block.gameObject);
        height = Mathf.Lerp(-2.25f, 2.25f, ThermometerController.GetRelativeTemperature(maxTemperature));
        limits.max = height;
        block = transform.FindChild("Max Block");
        block.localPosition = new Vector3(block.localPosition.x, height + block.lossyScale.y, block.localPosition.z);
        if (maxTemperature == ThermometerController.GetMaxTemperature()) Destroy(block.gameObject);
        sliderJoint.limits = limits;

        sliderJoint.connectedAnchor = slider.transform.position;

        if (heatSources != null)
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
            transform.FindChild("Water Marker").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                ThermometerController.GetRelativeTemperature(ParticleBond.meltingTemperatures[0]));
            transform.FindChild("Water Marker 2").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                ThermometerController.GetRelativeTemperature(ParticleBond.boilingTemperatures[0]));
        }
        else
        {
            Destroy(transform.FindChild("Water Marker").gameObject);//.SetActive(false);
            Destroy(transform.FindChild("Water Marker 2").gameObject);//.SetActive(false);
        }
        if (GameObject.FindObjectOfType<MetalParticleController>() != null)
        {
            transform.FindChild("Metal Marker").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                ThermometerController.GetRelativeTemperature(ParticleBond.meltingTemperatures[1]));
            transform.FindChild("Metal Marker 2").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                ThermometerController.GetRelativeTemperature(ParticleBond.boilingTemperatures[1]));
        }
        else
        {
            Destroy(transform.FindChild("Metal Marker").gameObject);//.SetActive(false);
            Destroy(transform.FindChild("Metal Marker 2").gameObject);//.SetActive(false);
        }
        if (GameObject.FindObjectOfType<CeramicParticleController>() != null)
        {
            transform.FindChild("Ceramic Marker").localPosition += Vector3.Lerp(Vector3.down * 2.25f, Vector3.up * 2.25f,
                ThermometerController.GetRelativeTemperature(ParticleBond.meltingTemperatures[2]));
        }
        else
        {
            Destroy(transform.FindChild("Ceramic Marker").gameObject);//.SetActive(false);
        }
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
