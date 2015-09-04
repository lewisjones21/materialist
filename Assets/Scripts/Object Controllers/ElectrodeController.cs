using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElectrodeController : MonoBehaviour {

    SpriteRenderer srAura;
    Color colorAuraBase;

    public float voltage = 0.0f;

    public bool isPositive = true;
    public bool canComplete = false, isConnected = false;

    public float completionPeriod = 3.0f;
    float curCompletionTime = 0.0f;

	void Start()
    {
        srAura = transform.FindChild("Aura").GetComponent<SpriteRenderer>();
        colorAuraBase = srAura.color;
        if (isPositive)
        {
            canComplete = (GameObject.Find("Electrode -") != null);
            isConnected = canComplete;
            voltage = 1.0f;
        }
        else
        {
            canComplete = (GameObject.Find("Electrode +") != null);
            isConnected = false;
            //transform.FindChild("Win Text").GetComponent<MeshRenderer>().enabled = false;
        }
	}
	
	void Update()
    {
        if (!isPositive)
        {
            srAura.color = Color.Lerp(Color.clear, colorAuraBase, voltage);
            if (voltage > 0.9f)
            {
                curCompletionTime += Time.deltaTime;
                if (curCompletionTime > completionPeriod)
                {
                    isConnected = true;
                    //transform.FindChild("Win Text").GetComponent<MeshRenderer>().enabled = true;
                }
            }
            else
            {
                curCompletionTime = 0.0f;
            }
        }
	}

    void FixedUpdate()
    {
        if (!isPositive) voltage *= 0.975f;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Particle")
        {
            MetalParticleController metal = collider.GetComponent<MetalParticleController>();
            if (metal != null)
            {
                float maxVoltage = Mathf.Max(voltage, metal.voltage);
                if (!isPositive) voltage = Mathf.Lerp(voltage, maxVoltage, 0.75f);
                metal.voltage = Mathf.Lerp(metal.voltage, maxVoltage, 0.75f);
            }
        }
    }
}
