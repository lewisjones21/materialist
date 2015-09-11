using UnityEngine;
using System.Collections;

public class LaserSwitchController : MonoBehaviour {

    LaserController laser;

	void Start ()
    {
        laser = GetComponentInParent<LaserController>();
        //transform.GetChild(0).SetParent(laser.transform);
        transform.SetParent(laser.transform.parent, true);
        transform.rotation = Quaternion.identity;
	}

    void OnMouseDown()
    {
        laser.on = true;//!laser.on;
    }
    void OnMouseUp()
    {
        laser.on = false;
        laser.dragable.OnMouseUp();
    }
    public void OnMouseEnter()
    {
        laser.dragable.OnMouseEnter();
    }
    void OnMouseExit()
    {
        //laser.on = false;
        if (laser.on)
        {
            laser.dragable.OnMouseDown();
            laser.dragable.SetJointAnchorForButton();
        }
        laser.dragable.OnMouseExit();
    }
}
