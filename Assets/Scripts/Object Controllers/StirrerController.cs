using UnityEngine;
using System.Collections;

public class StirrerController : MonoBehaviour {

    public float angularVelocity = 120.0f;
    float lastAngularVelocity;

	void Start()
    {
        lastAngularVelocity = angularVelocity;
        GetComponent<Rigidbody2D>().angularVelocity = angularVelocity;
	}

    void FixedUpdate()
    {
        if (lastAngularVelocity != angularVelocity)
        {
            GetComponent<Rigidbody2D>().angularVelocity = angularVelocity;
            lastAngularVelocity = angularVelocity;
        }
    }
}
