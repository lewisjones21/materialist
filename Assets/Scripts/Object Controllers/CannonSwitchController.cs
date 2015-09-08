using UnityEngine;
using System.Collections;

public class CannonSwitchController : MonoBehaviour {

    CannonController cannon;

	void Start ()
    {
        cannon = GetComponentInParent<CannonController>();
        //transform.GetChild(0).SetParent(laser.transform);
        transform.SetParent(cannon.transform.parent, true);
        transform.rotation = Quaternion.identity;
	}

    void OnMouseDown()
    {
        cannon.shouldFire = true;
        cannon.Fire();
    }
    void OnMouseUp()
    {
        cannon.shouldFire = false;
    }
    void OnMouseExit()
    {
        cannon.shouldFire = false;
    }
}
