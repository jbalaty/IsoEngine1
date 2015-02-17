using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using IsoEngine1;


public class CameraController : MonoBehaviour
{

    public float MinZoom = 1;
    public float MaxZoom = 10;
    public float CameraMovementSpeed = 0.5f;
    [HideInInspector]
    public float CameraStartOrthoSize;
    public Dungeon.GameController GameController;


    // Use this for initialization
    void Start()
    {
        this.CameraStartOrthoSize = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        var uihit = EventSystem.current.IsPointerOverGameObject();
        // ZOOMING
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && !uihit)
        { // forward
            Camera.main.orthographicSize = Mathf.Max(MinZoom, Camera.main.orthographicSize - 0.25f);

        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && !uihit)
        { // back
            Camera.main.orthographicSize = Mathf.Min(MaxZoom, Camera.main.orthographicSize + 0.25f);
        }

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
            Camera.main.orthographicSize += deltaMagnitudeDiff * 0.03f;
            // Make sure the orthographic size never drops below zero.
            Camera.main.orthographicSize = Mathf.Clamp(camera.orthographicSize, MinZoom, MaxZoom);
        }
    }

    public void PanCamera(Vector2 delta)
    {
        this.PanCamera(delta, this.CameraMovementSpeed);
    }
    public void PanCamera(Vector2 delta, float speed)
    {
        //Camera.main.transform.Translate(
        //    new Vector3(-Input.GetAxis("Mouse X") * CameraMovementSpeed,
        //            -Input.GetAxis("Mouse Y") * CameraMovementSpeed, 0));
        Camera.main.transform.Translate(
            new Vector3(delta.x * speed,
                    delta.y * speed, 0));
    }

    public void MoveCameraIfWeAreOnEdgeOfScreen(Vector2Int? gridCoords, Vector2Int mousePosition)
    {
        // try to move camera to stay in view
        // Clash of Clans does not solve this, so maybe it is pointless
        if (gridCoords != null)
        {
            int margin = 40;
            var movevec = new Vector3();
            var dx = Screen.width / 2f - mousePosition.x;
            var dy = Screen.height / 2f - mousePosition.y;
            movevec.x = Mathf.Abs(dx) > Screen.width / 2f - margin ? -1 * Mathf.Sign(dx) : 0f;
            movevec.y = Mathf.Abs(dy) > Screen.height / 2f - margin ? -1 * Mathf.Sign(dy) : 0f;
            Camera.main.transform.Translate(movevec * 0.5f);
        }
    }

    public float GetCameraScale()
    {
        return this.CameraStartOrthoSize / Camera.main.orthographicSize;
    }
}
