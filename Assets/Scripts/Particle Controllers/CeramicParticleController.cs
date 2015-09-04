using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CeramicParticleController : ParticleController {

	protected override void Start ()
    {
        type = 2;
        thermalTransferFactor = 0.7f;
        velocityTransferFactor = 0.35f;
        heatCapacity = 2.0f;
        mass = 8.0f;
        base.Start();
	}

    /*protected override void EarlyFixedUpdate()
    {
        base.EarlyFixedUpdate();
    }*/

}
