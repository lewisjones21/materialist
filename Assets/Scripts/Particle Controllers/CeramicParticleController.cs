using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CeramicParticleController : ParticleController {

    public override int type { get { return 2; } }
    public override float thermalTransferFactor { get { return 0.7f; } }
    public override float velocityTransferFactor { get { return 0.35f; } }
    public override float thermalStayFactor { get { return 0.3f; } }
    public override float velocityStayFactor { get { return 0.65f; } }
    public override float heatCapacity { get { return 2.0f; } }
    public override float mass { get { return 8.0f; } }
    public override float inverseMass { get { return 0.125f; } }
    public override float massxHeatCapacity { get { return 16.0f; } }
    public override float inverseMassxHeatCapacity { get { return 0.0625f; } }

}
