using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleController : MonoBehaviour {

    public Rigidbody2D rb;
    SpriteRenderer sr;

    public GameObject bond;
    public Color colourCold, colourSet, colourMelt, colourHot, colourSelected;

    //public List<ParticleController> connectedParticles = new ParticleController[4];
    public int type = 0;
    public List<ParticleBond> bonds;
    public static int maxBonds = 6;
    public static float thermalTransferFactor = 0.6f, velocityTransferFactor = 0.1f,//h x A, mu
        thermalStayFactor, velocityStayFactor;
    public static float heatCapacity = 5.0f;//c
    public static float mass = 1.0f, inverseMass, massxHeatCapacity, inverseMassxHeatCapacity;

    public float pressure, temperature;
    public float thermalEnergy;
    public float kineticEnergy;
    public float gravitationalEnergy;
    public float totalEnergy;
    Vector2 nextVelocity;
    float nextThermalEnergy;

    public float lastVelocitySquared;

    public bool boiled = false;
    public bool solidified = false;

    public float startTemperature = 295.0f;

    public float radius, radiusSquared;
    public Vector2 selectionOffset;
    public bool selected, shouldDelete = false;

	protected virtual void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        bonds = new List<ParticleBond>();

        thermalStayFactor = 1.0f - thermalTransferFactor;
        velocityStayFactor = 1.0f - velocityTransferFactor;

        inverseMass = 1 / rb.mass;
        massxHeatCapacity = rb.mass * heatCapacity;
        inverseMassxHeatCapacity = 1 / massxHeatCapacity;

        radius = transform.lossyScale.x * 0.5f;
        radiusSquared = radius * radius;
	}

    void Update()
    {
        if (temperature < ParticleBond.meltingTemperatures[type] - 25.0f)
        {
            sr.color = Color.Lerp(colourSet, colourCold, (ParticleBond.meltingTemperatures[type] - 25.0f - temperature) * 0.005f);
        }
        else if (temperature < ParticleBond.meltingTemperatures[type] + 25.0f)
        {
            sr.color = Color.Lerp(colourMelt, colourSet, (ParticleBond.meltingTemperatures[type] + 25.0f - temperature) * 0.02f);
        }
        else
        {
            sr.color = Color.Lerp(colourMelt, colourHot, (temperature - ParticleBond.meltingTemperatures[type]) * 0.005f);
        }
        if (selected)
        {
            sr.color = Color.Lerp(sr.color, colourSelected, 0.5f);
        }

        if (temperature > ParticleBond.boilingTemperatures[type] - 5.0f)
        {
            rb.gravityScale
                = 1.0f - Mathf.Min(1.5f, Mathf.Max(0.0f, (temperature - ParticleBond.boilingTemperatures[type] + 5.0f) * 0.2f));
        }
        else
        {
            rb.gravityScale = 1.0f;
            boiled = false;
        }
        if (temperature > ParticleBond.boilingTemperatures[type] + 5.0f && boiled == false)
        {
            boiled = true;
            AudioController.IncreaseEvaporationVolume((rb.position.x - Camera.main.transform.position.x) * 0.1f);
        }

        if (temperature > ParticleBond.meltingTemperatures[type] + 5.0f)
        {
            solidified = false;
        }
        if (temperature < ParticleBond.meltingTemperatures[type] - 5.0f && solidified == false)
        {
            solidified = true;
            AudioController.IncreaseSolidificationVolume(type, (rb.position.x - Camera.main.transform.position.x) * 0.1f);
        }
    }

    public virtual void EarlyFixedUpdate()
    {
        //lastVelocitySquared = rb.velocity.sqrMagnitude;//This is updated by the ParticleManager
        pressure *= 0.1f * (1 + Mathf.Min(3.0f, Mathf.Max(0.0f, (temperature - ParticleBond.boilingTemperatures[type] + 5.0f) * 0.1f)));
        //pressure *= 0.1f;//0.1f works
        Collider2D[] colliders = Physics2D.OverlapCircleAll(rb.position, radius * 2.5f, ParticleManager.layerMask);
        if (colliders != null)
        {
            Vector2 displacement;//, velocityDifference;
            float distanceSquared, inverseDistanceSquared;//, thermalEnergyDifference, transferFactor;
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Particle" && !collider.gameObject.Equals(gameObject))
                {
                    ParticleController other = collider.GetComponent<ParticleController>();
                    displacement = collider.transform.position - transform.position;
                    distanceSquared = displacement.sqrMagnitude;
                    inverseDistanceSquared = 1.0f / distanceSquared;
                    //The following line goes weird if the temperature goes negative
                    //(clearly that shouldn't happen (but does because of breaking bonds, velocity changes, etc.))
                    pressure += Mathf.Max(0.0f, inverseDistanceSquared * other.rb.mass
                        * Mathf.Min(temperature + 25.0f - ParticleBond.meltingTemperatures[type], 100.0f) * 0.01f);//0.05f);//* 5.0f;
                    TransferProperties(other, inverseDistanceSquared);

                    if (distanceSquared < radiusSquared * 4.5f)
                    {
                        Connect(other);
                    }
                    if (distanceSquared < radiusSquared * 0.9f)
                    {
                        //shouldDelete = true;
                        DisconnectAll();
                    }
                }
            }
            //if (pressure < 0.0f) pressure = 0.0f;
            //pressure -= 1.0f;//For surface tension?
            //if (pressure < 1.5f) pressure -= 4.0f;
            foreach (Collider2D collider in colliders)
            {
                if (collider.tag == "Particle" && !collider.gameObject.Equals(gameObject))
                {
                    displacement = collider.transform.position - transform.position;
                    collider.GetComponent<Rigidbody2D>().AddForce(displacement * pressure * pressure / displacement.sqrMagnitude);
                }
            }
        }
    }
    public virtual void LateFixedUpdate()
    {
        //Apply diffusion based velocity and thermal energy transfers
        rb.velocity = rb.velocity * velocityStayFactor + nextVelocity * velocityTransferFactor;
        thermalEnergy = thermalEnergy * thermalStayFactor + nextThermalEnergy * thermalTransferFactor;

        //Update thermal energy to ensure overal conservation of energy
        float deltaKineticEnergy = 0.5f * rb.mass * rb.velocity.sqrMagnitude - kineticEnergy;
        kineticEnergy += deltaKineticEnergy;
        float deltaGravitationalEnergy = -rb.mass * Vector2.Dot(Physics2D.gravity, rb.position) * rb.gravityScale - gravitationalEnergy;
        gravitationalEnergy += deltaGravitationalEnergy;
        if (!selected)
        {
            thermalEnergy -= deltaKineticEnergy + deltaGravitationalEnergy;
        }
        thermalEnergy = Mathf.Max(thermalEnergy, 0.0f);
        temperature = thermalEnergy * inverseMassxHeatCapacity;
        totalEnergy = thermalEnergy + kineticEnergy + gravitationalEnergy - ParticleBond.bondEnergies[type] * bonds.Count;

        nextVelocity = rb.velocity;
        nextThermalEnergy = thermalEnergy;

        /*if (thermalEnergy < 0.0f)
        {
            Debug.Log(name + "'s thermalEnergy is negative");
            Debug.Break();
        }*/
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Particle")
        {
            ParticleController other = collision.collider.GetComponent<ParticleController>();
            //Update thermal energy to ensure overal conservation of energy
            float myDeltaKineticEnergy = 0.5f * rb.mass * rb.velocity.sqrMagnitude - kineticEnergy;
            float otherDeltaKineticEnergy = 0.5f * other.rb.mass * other.rb.velocity.sqrMagnitude - other.kineticEnergy;
            float deltaThermalEnergy = -(myDeltaKineticEnergy + otherDeltaKineticEnergy) * 0.5f;
            thermalEnergy += deltaThermalEnergy;
            other.thermalEnergy += deltaThermalEnergy;
            kineticEnergy += myDeltaKineticEnergy;
            other.kineticEnergy += otherDeltaKineticEnergy;

        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag != "Particle")
        {
            rb.AddForce(collision.contacts[0].normal * Mathf.Abs(pressure) * 25.0f);
        }
    }

    protected virtual void TransferProperties(ParticleController other, float inverseDistanceSquared)
    {
        //Move particles toward a mutual equilibrium
        float transferFactor = inverseDistanceSquared * Time.fixedDeltaTime;
        Vector2 velocityDifference = (other.rb.velocity - rb.velocity) * Mathf.Min(0.5f, transferFactor);
        nextVelocity += velocityDifference;
        other.nextVelocity -= velocityDifference;
        float thermalEnergyDifference = (other.temperature - temperature) * Mathf.Min(0.5f, transferFactor * 10.0f);
        nextThermalEnergy += thermalEnergyDifference;
        other.nextThermalEnergy -= thermalEnergyDifference;
    }

    public bool IsConnected(ParticleController other)
    {
        foreach (ParticleBond bond in bonds)
        {
            if (bond.BondsParticle(other))
            {
                return true;
            }
        }
        return false;
    }

    public void Connect(ParticleController other)
    {
        if (bonds.Count >= ParticleController.maxBonds || other.bonds.Count >= ParticleController.maxBonds) return;
        if (other.GetType() != this.GetType()) return;
        foreach (ParticleBond bond in bonds)
        {
            if (bond.BondsParticle(other)) return;
        }
        //Debug.Log(other);
        
            //&& thermalEnergy > ParticleController.bondEnergy && other.thermalEnergy > ParticleController.bondEnergy
            if (temperature < ParticleBond.meltingTemperatures[type] - 5.0f
            && other.temperature < ParticleBond.meltingTemperatures[other.type] - 5.0f)
            //+ other.thermalEnergy < ParticleBond.bondReleaseEnergies[type, other.type])
        {
            ParticleManager.QueueToConnect(this, other);
        }
    }

    public void Disconnect(ParticleBond bond)
    {
        if (bonds.Contains(bond))
        {
            bond.Delete();
        }
    }

    public void DisconnectAll()
    {
        for (int n = bonds.Count - 1; n >= 0; n--)
        {
            bonds[n].Delete();
        }
    }

    public void SetTemperature(float newTemperature = -1.0f)
    {
        if (newTemperature == -1.0f)
        {
            thermalEnergy = startTemperature * massxHeatCapacity;
        }
        else
        {
            thermalEnergy = newTemperature * massxHeatCapacity;
        }
        nextThermalEnergy = thermalEnergy;
    }

    public bool SetVelocity(Vector2 velocity)
    {
        if (rb != null)
        {
            rb.velocity = velocity;
            lastVelocitySquared = Vector2.Dot(velocity, velocity);
            kineticEnergy = 0.5f * rb.mass * lastVelocitySquared;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Select(bool select = true)
    {
        rb.isKinematic = select;
        selected = select;
        /*if (select)
        {
            sr.color = colourSelected;
        }
        else
        {
            sr.color = colourDefault;
        }*/
    }

    public void Add()//Must be called to notify relevant managers of instantiation
    {
        ParticleManager.particles.Add(this);
        //ParticleHandler.DeleteSelection(this);
        //Destroy(gameObject);
    }
    public void Delete()
    {
        DisconnectAll();
        ParticleManager.particles.Remove(this);
        ParticleHandler.DeleteSelection(this);
        Destroy(gameObject);
    }
}
