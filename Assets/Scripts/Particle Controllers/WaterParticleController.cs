using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaterParticleController : ParticleController {

    public override int type { get { return 0; } }
    public override float thermalTransferFactor { get { return 0.45f; } }
    public override float velocityTransferFactor { get { return 0.1f; } }
    public override float thermalStayFactor { get { return 0.55f; } }
    public override float velocityStayFactor { get { return 0.9f; } }
    public override float heatCapacity { get { return 5.0f; } }
    public override float mass { get { return 1.0f; } }
    public override float inverseMass { get { return 1.0f; } }
    public override float massxHeatCapacity { get { return 5.0f; } }
    public override float inverseMassxHeatCapacity { get { return 0.2f; } }

}
