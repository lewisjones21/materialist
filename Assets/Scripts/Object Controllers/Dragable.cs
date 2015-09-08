using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Dragable : MonoBehaviour {

    LineRenderer lr;
    Rigidbody2D rb;
    SpringJoint2D joint;

    public bool fixOnRelease = false;

    public bool held = false;

	void Start()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();

        lr.enabled = false;
        rb.isKinematic = fixOnRelease;
	}
	
	void Update()
    {
	    if (joint != null)
        {
            Vector2 mousePoint
                = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * Camera.main.transform.position.z);
            joint.connectedAnchor = mousePoint;
            lr.SetPosition(0, mousePoint);
            lr.SetPosition(1, transform.TransformPoint(joint.anchor));
        }
	}

    void OnMouseDown()
    {
        joint = gameObject.AddComponent<SpringJoint2D>();
        Vector2 mousePoint
            = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * Camera.main.transform.position.z);
        joint.anchor = transform.InverseTransformPoint(mousePoint);
        joint.distance = 0.005f;
        joint.frequency = 0.5f;
        joint.dampingRatio = 1;
        joint.enableCollision = true;
        lr.enabled = true;
        rb.isKinematic = false;
        CursorManager.SetHasGrabbed(true);
        held = true;
    }
    void OnMouseUp()
    {
        Destroy(joint);
        joint = null;
        lr.enabled = false;
        rb.isKinematic = fixOnRelease;
        CursorManager.SetHasGrabbed(false);
        held = false;
    }

    void OnMouseEnter()
    {
        CursorManager.SetCanGrab(true);
    }
    void OnMouseExit()
    {
        CursorManager.SetCanGrab(false);
        if (joint == null)
        {
            Cursor.SetCursor(null, new Vector2(95, 38), CursorMode.Auto);
        }
    }
}
