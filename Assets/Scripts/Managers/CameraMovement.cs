using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    public Vector2 min; // The bottomleft and topright corners of the rectangle binding the scene.
    public Vector2 max; // The cameras rendering box cannot escape these bounds.  
    public float maxSize;   // The max and min orthographic width of the main camera
    public float minSize;

    private Camera cam;
    private float targetOrtho;

    // Use this for initialization
    void Start () {
        cam = GetComponent<Camera>();
        targetOrtho = cam.orthographicSize;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        setZoom();

        fixBounds();
    }

    private void setZoom() {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        cam.transform.Translate(move * 5 * Time.deltaTime);

        if (Input.GetKey(KeyCode.O)) {
            targetOrtho++;
            targetOrtho = Mathf.Clamp(targetOrtho, minSize, maxSize);
            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetOrtho, 2 * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.P)) {
            targetOrtho--;
            targetOrtho = Mathf.Clamp(targetOrtho, minSize, maxSize);
            cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, targetOrtho, 2 * Time.deltaTime);
        }
#endif

#if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            // If the camera is orthographic...
            // ... change the orthographic size based on the change in distance between the touches.
            float newOrtho = cam.orthographicSize + deltaMagnitudeDiff * 0.005f;  // 0.5f is the zoom speed, may get changed

            cam.orthographicSize = Mathf.Clamp(newOrtho, minSize, maxSize);

            // 2 finger touch to move camera
            cam.transform.Translate(-touchZero.deltaPosition * 0.01f);

        }
#endif

    }

    private void fixBounds() {
        Vector3 newCenter = cam.transform.position;
        Vector3 extent = new Vector3(cam.orthographicSize, cam.orthographicSize / cam.aspect, 0);

        if (newCenter.x + extent.x > max.x)
            newCenter.x = max.x - extent.x;
        else if (newCenter.x - extent.x < min.x)
            newCenter.x = min.x + extent.x;
        if (newCenter.y + extent.y > max.y)
            newCenter.y = max.y - extent.y;
        else if (newCenter.y - extent.y < min.y)
            newCenter.y = min.y + extent.y;

        cam.transform.position = newCenter;
    }

}
