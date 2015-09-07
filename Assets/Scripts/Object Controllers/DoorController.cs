using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class DoorController : MonoBehaviour {

    Rigidbody2D rb;
    AudioSource audioSource;

    public AudioClip impactSound;

    float lastVelocitySquared;

	void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        GetComponent<SliderJoint2D>().enabled = true;
        GetComponent<SliderJoint2D>().connectedAnchor = transform.position;

        transform.FindChild("Slot").SetParent(transform.parent);
	}

    void Update()
    {
        float velocitySquared = rb.velocity.sqrMagnitude;
        if (velocitySquared > 0.01f)
        {
            if (!audioSource.isPlaying) audioSource.Play();
            audioSource.volume = velocitySquared * 0.4f;
            audioSource.pitch = 0.25f + velocitySquared * 0.00075f;
        }
        else
        {
            if (audioSource.isPlaying) audioSource.Stop();
            if (lastVelocitySquared > 0.1f) AudioSource.PlayClipAtPoint(impactSound, Vector3.zero, lastVelocitySquared * 0.001f);
        }
        lastVelocitySquared = velocitySquared;
    }
}
