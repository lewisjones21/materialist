using UnityEngine;
using System.Collections;

public class CameraScaler : MonoBehaviour {

    public Vector2 topRight, bottomLeft;

	void Start()
    {
        float halfWidth = (topRight.x - bottomLeft.x) * 0.5f;
        float halfHeight = (topRight.y - bottomLeft.y) * 0.5f;
        Camera.main.orthographicSize = Mathf.Max(halfHeight, halfWidth * Screen.height / Screen.width);
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(bottomLeft, 0.2f);
        Gizmos.DrawLine(bottomLeft, new Vector2(bottomLeft.x, topRight.y));
        Gizmos.DrawLine(bottomLeft, new Vector2(topRight.x, bottomLeft.y));
        Gizmos.DrawLine(topRight, new Vector2(bottomLeft.x, topRight.y));
        Gizmos.DrawLine(topRight, new Vector2(topRight.x, bottomLeft.y));
    }
}
