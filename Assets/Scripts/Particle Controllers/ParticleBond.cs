using UnityEngine;
using System.Collections;

public class ParticleBond : MonoBehaviour {

    public static float[] meltingTemperatures = new float[3] { 273.0f, 573.0f, 823.0f },//Water, Metal, Ceramic
        boilingTemperatures = new float[3] { 373.0f, 1000.0f, 2000.0f },
        bondEnergies = new float[3] { 100.0f, 50.0f, 30.0f };
    //public static float[,] bondReleaseEnergies, inverseBondReleaseEnergies = new float[2, 2];

    public LineRenderer lr;

    public DistanceJoint2D joint;
    ParticleController a, b;
    int typeA, typeB;

    void Start()
    {
        /*bondReleaseEnergies[0, 0] = 600.0f;
        bondReleaseEnergies[1, 1] = 1200.0f;
        bondReleaseEnergies[0, 1] = 900.0f;
        bondReleaseEnergies[1, 0] = 900.0f;
        for (int a = bondReleaseEnergies.Length - 1; a >= 0; a--)
        {
            for (int b = bondReleaseEnergies.Length - 1; b >= 0; b--)
            {
                inverseBondReleaseEnergies[a, b] = 1.0f / bondReleaseEnergies[a, b];
            }
        }*/
        
        lr = GetComponent<LineRenderer>();
        lr.sortingLayerName = "Particle";
        lr.sortingOrder = 0;
    }

    void FixedUpdate()
    {
        lr.SetPosition(0, a.rb.position);
        lr.SetPosition(1, b.rb.position);
        //transform.position = (a.rb.position + b.rb.position) * 0.5f;
        //transform.rotation = Quaternion.LookRotation(b.transform.position - a.transform.position, Vector3.forward);


        /*Vector2 velocityDifference = (b.rb.velocity - a.rb.velocity) * inverseTotalMass * velocityTransferFactor * Time.fixedDeltaTime;
        a.rb.velocity += velocityDifference * b.rb.mass;
        b.rb.velocity -= velocityDifference * a.rb.mass;
        float thermalEnergyDifference = (b.thermalEnergy - a.thermalEnergy) * 0.5f * thermalTransferFactor * Time.fixedDeltaTime;
        a.thermalEnergy += thermalEnergyDifference;
        b.thermalEnergy -= thermalEnergyDifference;*/
        //Vector2 relativeVelocity = (b.rb.velocity - a.rb.velocity);


        //float totalThermalEnergy = a.thermalEnergy + b.thermalEnergy;
        /*if ((a.rb.position - b.rb.position).sqrMagnitude
            > radiusSquared * (1.0f + 0.21f * (1.0f - totalThermalEnergy * inverseBondReleaseEnergy))
            && a.thermalEnergy > aBondEnergy && b.thermalEnergy > bBondEnergy
            && totalThermalEnergy > bondReleaseEnergy)
            //&& joint.GetReactionForce(Time.deltaTime).sqrMagnitude > maxForceSquared)*/
        if (joint != null)
        {
            if (a.temperature > meltingTemperatures[typeA] + 5.0f && b.temperature > meltingTemperatures[typeB] + 5.0f
                || Random.value < 0.0000001 * (joint.GetReactionForce(Time.deltaTime).sqrMagnitude - 100000.0f))
            //* (a.temperature - meltingTemperatures[typeA] + b.temperature - meltingTemperatures[typeB] + 10.0f))
            {
                Delete();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(a.rb.position, 0.05f);
        Gizmos.DrawLine(a.rb.position, b.rb.position);
    }

    public void Attach(ParticleController particleA, ParticleController particleB)
    {
        name = particleA.name + " - " + particleB.name;
        a = particleA;
        b = particleB;
        a.bonds.Add(this);
        b.bonds.Add(this);
        typeA = a.type;
        typeB = b.type;
        a.thermalEnergy += bondEnergies[typeA];
        b.thermalEnergy += bondEnergies[typeB];
        joint = a.gameObject.AddComponent<DistanceJoint2D>();
        joint.connectedBody = b.rb;
        joint.distance = a.radius + b.radius;
        joint.enableCollision = true;
    }

    public void Delete()
    {
        Destroy(joint);
        a.bonds.Remove(this);
        b.bonds.Remove(this);
        a.thermalEnergy -= bondEnergies[typeA];
        b.thermalEnergy -= bondEnergies[typeB];
        ParticleBondManager.StoreBond(this);
    }

    public bool BondsParticle(ParticleController particle)
    {
        return (a.Equals(particle) || b.Equals(particle));
    }
}
