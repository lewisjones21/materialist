using UnityEngine;
using System.Collections;

public class BatteryController : MonoBehaviour {

    LineRenderer lr;
    Transform energyMeter;

    public float maxEnergy = 10000.0f, energy;

    Vector3 targetPosition;
    bool targetSet = true;

	void Start()
    {
        lr = GetComponent<LineRenderer>();
        energyMeter = transform.Find("Energy Meter");
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position);
        lr.sortingLayerName = "Battery";
        lr.sortingOrder = 0;

        energy = maxEnergy;
	}
	
	void Update()
    {
        if (!targetSet)
        {
            lr.SetPosition(1, targetPosition);
            targetSet = true;
        }

        if (energy < 0.0f) energy = 0.0f;

        float value = 2.0f * energy / maxEnergy;
	    energyMeter.localScale = new Vector3(energyMeter.localScale.x, value, 1.0f);
        energyMeter.localPosition = new Vector3(energyMeter.localPosition.x, value * 0.5f - 1.0f, 0.0f);
	}

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        targetSet = false;
    }
}
