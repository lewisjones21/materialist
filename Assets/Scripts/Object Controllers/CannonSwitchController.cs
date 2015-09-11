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
        cannon.dragable.OnMouseUp();
    }
    public void OnMouseEnter()
    {
        cannon.dragable.OnMouseEnter();
    }
    void OnMouseExit()
    {
        //cannon.shouldFire = false;
        if (cannon.shouldFire)
        {
            cannon.dragable.OnMouseDown();
            cannon.dragable.SetJointAnchorForButton();
        }
        cannon.dragable.OnMouseExit();
    }
}
