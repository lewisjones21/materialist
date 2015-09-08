using UnityEngine;
using System.Collections;

public class ContainmentObjective : LevelObjective {

    public Vector2 topRight, bottomLeft;
    public int type = -1, countRequired = 12, currentCount = 0, completionRelief = 4;
    public float minTemperature = -1.0f, maxTemperature = -1.0f;

	protected override void Start()
    {
        base.Start();
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
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
        foreach (ParticleController particle in ParticleManager.particles)
        {
            if (type >= 0 && particle.type != type) continue;
            if (particle.rb.position.x < bottomLeft.x || particle.rb.position.x > topRight.x
                || particle.rb.position.y < bottomLeft.y || particle.rb.position.y > topRight.y) continue;
            if (minTemperature > 0.0f && particle.temperature < minTemperature) continue;
            if (maxTemperature > 0.0f && particle.temperature > maxTemperature) continue;
            currentCount++;
        }
        isComplete = (currentCount >= countRequired - (isComplete ? completionRelief : 0));
        return base.GetIsComplete();
    }
}
