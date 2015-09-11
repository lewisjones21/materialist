using UnityEngine;
using System.Collections;

public class CannonController : MonoBehaviour {

    Rigidbody2D rb;
    LineRenderer lr_aim;
    //ParticleSystem ps;
    AudioSource audioSource;
    public Dragable dragable;

    public GameObject particle;
    public int particleType = 0;
    public float speed = 5.0f, temperature = 295.0f;
    public bool automatic = false, shouldFire = false;
    public float minAngle = -60.0f, maxAngle = 60.0f;

    float timeOutPeriod = 0.5f, currentTimeOut = 0.0f;

    Vector2 hitPoint;

	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr_aim = transform.FindChild("Handle").GetComponent<LineRenderer>();
        //ps = lr_beam.transform.FindChild("End Effect").GetComponent<ParticleSystem>();
        //pivot = transform.FindChild("Pivot");
        audioSource = GetComponent<AudioSource>();
        dragable = GetComponent<Dragable>();
        lr_aim.SetPosition(0, transform.position);
        lr_aim.sortingLayerName = "Cannon";
        lr_aim.sortingOrder = 0;
        //ps.enableEmission = false;

        HingeJoint2D hinge = GetComponent<HingeJoint2D>();
        hinge.connectedAnchor = transform.TransformPoint(GetComponent<HingeJoint2D>().anchor);
        if (maxAngle - minAngle < 360.0f)
        {
            JointAngleLimits2D newLimits = new JointAngleLimits2D();
            newLimits.min = minAngle;
            newLimits.max = maxAngle;
            hinge.limits = newLimits;
        }
        else
        {
            hinge.useLimits = false;
        }
        if (maxAngle - minAngle <= 120.0f)//If they wouldn't look untangible
        {
            //Rotate the blocks and unparent them to prevent further rotation
            Transform blockContainer = transform.FindChild("Min Block Container");
            blockContainer.localRotation = Quaternion.AngleAxis(-minAngle, Vector3.forward);
            blockContainer.GetChild(0).SetParent(transform.parent);
            Destroy(blockContainer.gameObject);
            blockContainer = transform.FindChild("Max Block Container");
            blockContainer.localRotation = Quaternion.AngleAxis(-maxAngle, Vector3.forward);
            blockContainer.GetChild(0).SetParent(transform.parent);
            Destroy(blockContainer.gameObject);
        }
        else
        {
            //Destroy the blocks
            Destroy(transform.FindChild("Min Block Container").gameObject);
            Destroy(transform.FindChild("Max Block Container").gameObject);
        }
        //Rotate the hand icon to start upright
        transform.FindChild("Handle").FindChild("Hand").rotation = Quaternion.identity;
        //Change the symbol letter
        transform.FindChild("Symbol").GetComponent<TextMesh>().text = (particleType == 2 ? "C" : (particleType == 1 ? "M" : "W"));
        transform.FindChild("Symbol").rotation = Quaternion.identity;

        timeOutPeriod = 0.5f / speed;//Enough time for the previous particle to get clear
	}
	
	void Update()
    {
        if (dragable.held)
        {
            //Position aiming line
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * 2.75f, transform.right, 500.0f);
            if (hit.collider != null)
            {
                hitPoint = hit.point;
            }
            else
            {
                hitPoint = transform.position + transform.right * 500.0f;
            }
            lr_aim.SetPosition(1, hitPoint);
            lr_aim.enabled = true;
        }
        else
        {
            lr_aim.enabled = false;
        }

        /*if (currentTimeOut > 0.0f)
        {
            currentTimeOut -= Time.deltaTime;
        }
        else
        {
            currentTimeOut = 0.0f;
            if (automatic && shouldFire) Fire();
        }*/
        if (automatic && shouldFire) Fire();
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + transform.right * 2.75f, 0.125f);
        Gizmos.DrawLine(transform.position, hitPoint);
    }

    public void Fire()
    {
        Vector2 position = transform.position + transform.right;
        if (/*currentTimeOut == 0.0f && */Physics2D.OverlapCircle(position, 0.5f, ParticleManager.layerMask) == null)
        {
            GameObject newParticle = (GameObject)Instantiate(particle, position, Quaternion.identity);
            //currentParticle = newParticle.GetComponent<ParticleController>();
            Vector2 velocity = speed * transform.right;
            Rigidbody2D rb_particle = newParticle.GetComponent<Rigidbody2D>();
            rb_particle.velocity = velocity;
            ParticleController particleController = newParticle.GetComponent<ParticleController>();
            //particleController.Add();
            particleController.SetTemperature(temperature, true);
            //Set energy-related variables so that the temperature doesn't get reduced again when attempting to conserve energy
            particleController.lastVelocitySquared = Vector2.Dot(velocity, velocity);
            particleController.kineticEnergy = 0.5f * rb_particle.mass * particleController.lastVelocitySquared;
            //currentTimeOut = timeOutPeriod;
            shouldFire = automatic && shouldFire;
        }
    }
}
