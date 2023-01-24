using UnityEngine;

public class CannonController : MonoBehaviour
{
    LineRenderer lr_aim;
    public Dragable dragable;

    public GameObject particle;
    public int particleType = 0;
    public float speed = 5.0f, temperature = 295.0f;
    public bool automatic = false, shouldFire = false;
    public float minAngle = -60.0f, maxAngle = 60.0f;

    Vector2 hitPoint;

	void Start()
    {
        lr_aim = transform.Find("Handle").GetComponent<LineRenderer>();
        dragable = GetComponent<Dragable>();
        lr_aim.SetPosition(0, transform.position);
        lr_aim.sortingLayerName = "Cannon";
        lr_aim.sortingOrder = 0;

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
            Transform blockContainer = transform.Find("Min Block Container");
            blockContainer.localRotation = Quaternion.AngleAxis(-minAngle, Vector3.forward);
            blockContainer.GetChild(0).SetParent(transform.parent);
            Destroy(blockContainer.gameObject);
            blockContainer = transform.Find("Max Block Container");
            blockContainer.localRotation = Quaternion.AngleAxis(-maxAngle, Vector3.forward);
            blockContainer.GetChild(0).SetParent(transform.parent);
            Destroy(blockContainer.gameObject);
        }
        else
        {
            //Destroy the blocks
            Destroy(transform.Find("Min Block Container").gameObject);
            Destroy(transform.Find("Max Block Container").gameObject);
        }
        //Rotate the hand icon to start upright
        transform.Find("Handle").Find("Hand").rotation = Quaternion.identity;
        //Change the symbol letter
        transform.Find("Symbol").GetComponent<TextMesh>().text = (particleType == 2 ? "C" : (particleType == 1 ? "M" : "W"));
        transform.Find("Symbol").rotation = Quaternion.identity;
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
        if (Physics2D.OverlapCircle(position, 0.5f, ParticleManager.layerMask) == null)
        {
            GameObject newParticle = Instantiate(particle, position, Quaternion.identity);
            Vector2 velocity = speed * transform.right;
            Rigidbody2D rb_particle = newParticle.GetComponent<Rigidbody2D>();
            rb_particle.velocity = velocity;
            ParticleController particleController = newParticle.GetComponent<ParticleController>();
            particleController.SetTemperature(temperature, true);
            //Set energy-related variables so that the temperature doesn't get reduced again when attempting to conserve energy
            particleController.lastVelocitySquared = Vector2.Dot(velocity, velocity);
            particleController.kineticEnergy = 0.5f * rb_particle.mass * particleController.lastVelocitySquared;
            shouldFire = automatic && shouldFire;
        }
    }
}
