using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetalParticleController : ParticleController {

    public  float voltage = 0.0f;
    MetalParticleController otherMetal;//Is this better than dynamic allocation?

	protected override void Start ()
    {
        type = 1;
        thermalTransferFactor = 0.6f;
        velocityTransferFactor = 0.2f;
        heatCapacity = 3.0f;
        mass = 3.0f;
        base.Start();
	}

    public override void EarlyFixedUpdate()
    {
        voltage *= 0.975f;
        base.EarlyFixedUpdate();
    }

    protected override void TransferProperties(ParticleController other, float inverseDistanceSquared)
    {
        base.TransferProperties(other, inverseDistanceSquared);
        if (other is MetalParticleController)
        {
            inverseDistanceSquared *= 0.75f;
            otherMetal = (MetalParticleController)other;
            float maxVoltage = Mathf.Max(voltage, otherMetal.voltage);
            voltage = Mathf.Lerp(voltage, maxVoltage, inverseDistanceSquared);
            otherMetal.voltage = Mathf.Lerp(otherMetal.voltage, maxVoltage, inverseDistanceSquared);
        }
    }

}
