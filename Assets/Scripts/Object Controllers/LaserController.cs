using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour {

    Rigidbody2D rb;
    LineRenderer lr;
    //Transform pivot;
    AudioSource audioSource;

    public BatteryController battery;

    public bool on = true;
    public float maxPower = 250.0f, power;
    public float minAngle = -60.0f, maxAngle = 60.0f;

    Vector2 hitPoint;
    ParticleController hitParticle;

	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = transform.FindChild("Beam").GetComponent<LineRenderer>();
        //pivot = transform.FindChild("Pivot");
        audioSource = GetComponent<AudioSource>();
        lr.sortingLayerName = "Laser";
        lr.sortingOrder = 0;
        HingeJoint2D hinge = GetComponent<HingeJoint2D>();
        hinge.connectedAnchor = transform.TransformPoint(GetComponent<HingeJoint2D>().anchor);
        JointAngleLimits2D newLimits = new JointAngleLimits2D();
        newLimits.min = minAngle;
        newLimits.max = maxAngle;
        hinge.limits = newLimits;

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
        lr.SetPosition(0, rb.position);
        if (relativePower > 0.01f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right * 2.75f, transform.right, 100.0f);
            /*RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + transform.forward * 5.0f, transform.forward, 100.0f);
            float distanceSquared;
            RaycastHit2D closestHit;
            foreach (RaycastHit2D hit in hits)
            {
                if ()
            }*/
            Debug.Log(hit.point);
            Debug.Log(hit.collider.tag);
            if (hit.point != null)
            {
                hitPoint = hit.point;
                lr.SetPosition(1, hitPoint);//Vector3.right * 10.0f);//
                hitParticle = hit.collider.GetComponent<ParticleController>();
                if (hitParticle != null)
                {
                    hitParticle.thermalEnergy += power * Time.deltaTime;
                }
            }
        }
        else
        {
            lr.SetPosition(1, rb.position);
        }
        lr.SetWidth(0.15f * relativePower, 0.15f * relativePower);
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
