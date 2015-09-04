using UnityEngine;
using System.Collections;

public class LaserController : MonoBehaviour {

    Rigidbody2D rb;
    LineRenderer lr;

    public bool on = true;
    public float power = 250.0f;
    Vector2 hitPoint;
    ParticleController hitParticle;

	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = transform.FindChild("Beam").GetComponent<LineRenderer>();
        lr.sortingLayerName = "Laser";
        lr.sortingOrder = 0;
        GetComponent<HingeJoint2D>().connectedAnchor = transform.TransformPoint(GetComponent<HingeJoint2D>().anchor);
	}
	
	void Update()
    {
        lr.SetPosition(0, rb.position);
        if (on)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.forward, transform.forward, 100.0f);
            Debug.DrawRay(transform.position + transform.forward, transform.forward);
            hitPoint = hit.point;
            lr.SetPosition(1, hitPoint);
            hitParticle = hit.collider.GetComponent<ParticleController>();
            if (hitParticle != null)
            {
                hitParticle.thermalEnergy += power * Time.deltaTime;
            }
        }
        else
        {
            lr.SetPosition(1, rb.position);
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.25f);
        Gizmos.DrawLine(transform.position, hitPoint);
    }
}
