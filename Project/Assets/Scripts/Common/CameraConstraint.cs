using UnityEngine;
using System.Collections;

public class CameraConstraint : MonoBehaviour {
    public Transform leftLimit;
    public Transform rightLimit;
    public Transform topLimit;
    public Transform bottomLimit;

    private Camera cam;
    void Start()
    {
        cam = camera;
    }
	void OnPreCull()
	{
        var bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0));
        var topRight = cam.ViewportToWorldPoint(new Vector3(1, 1));
        var bottomRight = camera.ViewportToWorldPoint(new Vector3(1, 0));
	    var halfWidth = (bottomRight - bottomLeft).magnitude * 0.5f;
	    var halfHeight = (topRight - bottomRight).magnitude * 0.5f;
        
        var bottom = bottomLimit.localPosition.y + halfHeight;
        var left = leftLimit.localPosition.x + halfWidth;
        var top = topLimit.localPosition.y - halfHeight;
        var right = rightLimit.localPosition.x - halfWidth;
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Clamp(pos.x, left, right);
        pos.y = Mathf.Clamp(pos.y, bottom, top);
        transform.localPosition = pos;
	}
}
