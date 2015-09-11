using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetalParticleController : ParticleController {

    public override int type { get { return 1; } }
    public override float thermalTransferFactor { get { return 0.6f; } }
    public override float velocityTransferFactor { get { return 0.2f; } }
    public override float thermalStayFactor { get { return 0.4f; } }
    public override float velocityStayFactor { get { return 0.8f; } }
    public override float heatCapacity { get { return 3.0f; } }
    public override float mass { get { return 3.0f; } }
    public override float inverseMass { get { return 0.33333333f; } }
    public override float massxHeatCapacity { get { return 9.0f; } }
    public override float inverseMassxHeatCapacity { get { return 0.11111111f; } }

    public  float voltage = 0.0f;
    MetalParticleController otherMetal;//Is this better than dynamic allocation?

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
