using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour {

    Rigidbody2D rb;
    LineRenderer lr_aim, lr_beam;
    ParticleSystem ps;
    //Transform pivot;
    AudioSource audioSource;
    Dragable dragable;

    public BatteryController battery;

    public bool on = true;
    public float maxPower = 250.0f, power;
    public float minAngle = -60.0f, maxAngle = 60.0f;

    Vector2 hitPoint;
    ParticleController hitParticle;

	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr_aim = transform.FindChild("Handle").GetComponent<LineRenderer>();
        lr_beam = transform.FindChild("Beam").GetComponent<LineRenderer>();
        ps = lr_beam.transform.FindChild("End Effect").GetComponent<ParticleSystem>();
        //pivot = transform.FindChild("Pivot");
        audioSource = GetComponent<AudioSource>();
        dragable = GetComponent<Dragable>();
        lr_aim.SetPosition(0, transform.position);
        lr_aim.sortingLayerName = "Laser";
        lr_aim.sortingOrder = 0;
        lr_beam.sortingLayerName = "Laser";
        lr_beam.sortingOrder = 0;
        ps.enableEmission = false;

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

        if (battery != null) battery.SetTarget(transform.position);

	}
	
	void Update()
    {
        if (battery != null)
        {
            battery.energy -= power * Time.deltaTime;
            if (battery.energy <= 0.0f) on = false;
        }
        if (on) power = power * 0.9f + maxPower * 0.1f;
        else power *= 0.9f;

        float relativePower = power / maxPower;
        //pivot.rotation = Quaternion.identity;
        lr_beam.SetPosition(0, rb.position);
        if (relativePower > 0.01f)
        {
            lr_aim.enabled = false;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * 2.75f, transform.right, 500.0f);
            if (hit.point != null)
            {
                hitPoint = hit.point;
                lr_beam.SetPosition(1, hitPoint);
                hitParticle = hit.collider.GetComponent<ParticleController>();
                if (hitParticle != null)
                {
                    hitParticle.thermalEnergy += power * Time.deltaTime;
                }
                ps.transform.position = hitPoint;
                //ps.transform.rotation = Quaternion.LookRotation(hit.normal);
                ps.enableEmission = true;
                ps.emissionRate = 100.0f * relativePower * relativePower;
            }
        }
        else
        {
            lr_beam.SetPosition(1, rb.position);
            ps.enableEmission = false;
            if (dragable.held)
            {
                //Position aiming line
                RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * 2.75f, transform.right, 500.0f);
                if (hit.point != null)
                {
                    hitPoint = hit.point;
                    lr_aim.SetPosition(1, hitPoint);
                }
                lr_aim.enabled = true;
            }
            else
            {
                lr_aim.enabled = false;
            }
        }
        lr_beam.SetWidth(0.15f * relativePower, 0.15f * relativePower);
        audioSource.pitch = relativePower;
        audioSource.volume = 0.2f * relativePower;
        if (audioSource.volume > 0.01f && !audioSource.isPlaying) audioSource.Play();
        if (audioSource.volume < 0.01f && audioSource.isPlaying) audioSource.Stop();
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.right * 2.75f, 0.125f);
        Gizmos.DrawLine(transform.position, hitPoint);
    }
}
