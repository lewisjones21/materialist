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
    }
    void OnMouseExit()
    {
        laser.on = false;
    }
}
