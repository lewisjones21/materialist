using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleHandler : MonoBehaviour
{

    public static ParticleHandler instance;

    public static List<ParticleController> selection;

    public bool canGrab = true, canHeat = true, canCool = true;

    public Vector2 mousePoint, mousePointDelta, mouseVelocity;
    public float selectionRadius = 0.25f;

    

	void Start()
    {
        if (instance == null)
        {
            instance = this;
            selection = new List<ParticleController>();
            transform.localScale = Vector3.one * selectionRadius * 0.5f;
        }
        else
        {
            Destroy(gameObject);
        }
	}
	
	void Update()
    {
        Vector2 tempMousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * Camera.main.transform.position.z);
        mousePointDelta = tempMousePoint - mousePoint;
        mouseVelocity = mouseVelocity * 0.5f + Vector2.ClampMagnitude(mousePointDelta / Time.deltaTime, 20.0f) * 0.5f;
        mousePoint = tempMousePoint;
        Debug.DrawRay(mousePoint, mousePointDelta);
        transform.position = mousePoint;
        if (canGrab && Input.GetMouseButtonDown(0))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePoint, selectionRadius);
            if (colliders != null)
            {
                ParticleController particle;
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "Particle")
                    {
                        particle = collider.GetComponent<ParticleController>();
                        AddSelection(particle);
                        particle.selectionOffset = mousePoint - particle.rb.position;
                    }
                }
            }
        }
        if (!canGrab || Input.GetMouseButtonUp(0))
        {
            foreach (ParticleController particle in selection)
            {
            }
            ResetSelection();
        }

        if (selection.Count > 0)
        {
            foreach (ParticleController particle in selection)
            {
                particle.rb.position = mousePoint - particle.selectionOffset;
                particle.rb.velocity = mouseVelocity;
                if (mouseVelocity.sqrMagnitude > 25.0f)
                {
                    particle.DisconnectAll();
                    if (particle.thermalEnergy < 0.0f) particle.thermalEnergy = 0.0f;
                }
            }
        }

        if (canGrab || canHeat || canCool)
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                selectionRadius = Mathf.Clamp(selectionRadius + Input.mouseScrollDelta.y * 0.25f, 0.25f, 4.0f);
                transform.localScale = Vector3.one * selectionRadius * 0.5f;
            }
        }
        else
        {
            selectionRadius = 0.25f;
            transform.localScale = Vector3.one * selectionRadius * 0.5f;
        }


        if (canHeat && Input.GetMouseButton(1))
        {
            //Debug.Log("Right clicked");
            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePoint, selectionRadius * 2.0f);
            if (colliders != null)
            {
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "Particle")
                    {
                        collider.GetComponent<ParticleController>().thermalEnergy += 5000.0f * Time.fixedDeltaTime;
                    }
                }
            }
        }
        if (canCool && Input.GetMouseButton(2))
        {
            Debug.Log("Right clicked");
            ParticleController particleController;
            Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePoint, selectionRadius * 2.0f);
            if (colliders != null)
            {
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "Particle")
                    {
                        particleController = collider.GetComponent<ParticleController>();
                        particleController.thermalEnergy -= 5000.0f * Time.fixedDeltaTime;
                        if (particleController.thermalEnergy < 0.0f) particleController.thermalEnergy = 0.0f;
                    }
                }
            }
        }
	}

    public static bool Exists()
    {
        return (instance != null);
    }

    public static void SetSelection(ParticleController particle)
    {
        Debug.Log("Setting selection to " + particle.name);
        ResetSelection();
        AddSelection(particle);
    }
    public static void ResetSelection()
    {
        foreach (ParticleController oldParticle in selection)
        {
            oldParticle.Select(false);
        }
        selection.Clear();
    }
    public static bool AddSelection(ParticleController particle)//Returns whether the particle was in the selection (and adds it anyway)
    {
        if (!selection.Contains(particle))
        {
            selection.Add(particle);
            particle.Select();
            return false;
        }
        return true;
    }
    public static bool RemoveSelection(ParticleController particle)//Returns whether the particle was in the selection (and removes it)
    {
        if (selection.Remove(particle))
        {
            particle.Select(false);
            return true;
        }
        return false;
    }
    public static bool ToggleSelection(ParticleController particle)//Returns whether the particle is now in the selection
    {
        Debug.Log("Toggling selection of " + particle.name);
        if (!RemoveSelection(particle))
        {
            AddSelection(particle);
            return true;
        }
        return false;
    }
    public static void DeleteSelection(ParticleController particle)//Returns whether the particle was in the selection (and removes it)
    {
        if (Exists())
        {
            selection.Remove(particle);
        }
    }
}
