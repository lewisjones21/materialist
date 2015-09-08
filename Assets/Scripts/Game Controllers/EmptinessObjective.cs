using UnityEngine;
using System.Collections;

public class EmptinessObjective : LevelObjective {

    Vector2 bottomLeft, topRight;
    public int typeToRemove = -1, maxCountAllowed = 0, currentCount = 0;

	protected override void Start()
    {
        base.Start();
        bottomLeft = transform.TransformPoint(-0.5f, -0.5f, 0.0f);
        topRight = transform.TransformPoint(0.5f, 0.5f, 0.0f);
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(bottomLeft, 0.2f);
        Gizmos.DrawLine(bottomLeft, new Vector2(bottomLeft.x, topRight.y));
        Gizmos.DrawLine(bottomLeft, new Vector2(topRight.x, bottomLeft.y));
        Gizmos.DrawLine(topRight, new Vector2(bottomLeft.x, topRight.y));
        Gizmos.DrawLine(topRight, new Vector2(topRight.x, bottomLeft.y));
    }

    public override bool GetIsComplete()
    {
        GameObject[] go_particles = GameObject.FindGameObjectsWithTag("Particle");
        currentCount = 0;
        ParticleController particle;
        foreach (GameObject go_particle in go_particles)
        {
            particle = go_particle.GetComponent<ParticleController>();
            if (typeToRemove >= 0 && particle.type != typeToRemove) continue;
            if (particle.rb.position.x + 0.3f < bottomLeft.x || particle.rb.position.x - 0.3f > topRight.x
                || particle.rb.position.y + 0.3f < bottomLeft.y || particle.rb.position.y - 0.3f > topRight.y) continue;
            currentCount++;
        }
        isComplete = (currentCount <= maxCountAllowed + (isComplete ? 2 : 0));
        return base.GetIsComplete();
    }
}
