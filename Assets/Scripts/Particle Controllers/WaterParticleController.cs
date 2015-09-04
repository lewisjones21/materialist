using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterParticleController : ParticleController {

	protected override void Start ()
    {
        type = 0;
        thermalTransferFactor = 0.45f;
        velocityTransferFactor = 0.1f;
        heatCapacity = 5.0f;
        mass = 1.0f;
        base.Start();
	}

}
