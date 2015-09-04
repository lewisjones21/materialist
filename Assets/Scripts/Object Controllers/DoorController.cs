using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour {

	void Start()
    {
        GetComponent<SliderJoint2D>().enabled = true;
        GetComponent<SliderJoint2D>().connectedAnchor = transform.position;

        transform.FindChild("Slot").SetParent(transform.parent);
	}

}
