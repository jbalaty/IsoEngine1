using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{

	//float speed = 1f;
	public float MinZoom = 1;
	public float MaxZoom = 10;
	public float CameraMovementSpeed = 0.5f;
	public GameController GameController;
	private Vector3? MouseDownPoint;
    private bool LastMouseDownWasUIHit = false;

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		//var camera = this.GetComponent<Camera>();
		//camera.orthographicSize -= speed * Time.deltaTime;

		if (Input.GetAxis ("Mouse ScrollWheel") > 0) { // forward
			Camera.main.orthographicSize = Mathf.Max (MinZoom, Camera.main.orthographicSize - 0.25f);

		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) { // back
			Camera.main.orthographicSize = Mathf.Min (MaxZoom, Camera.main.orthographicSize + 0.25f);
		}

		if (Input.GetMouseButton(0) && Input.GetAxis ("Mouse X") != 0 
			&& Input.GetAxis ("Mouse Y") != 0) {
			Camera.main.transform.Translate (
				new Vector3 (-Input.GetAxis ("Mouse X") * CameraMovementSpeed,
			            -Input.GetAxis ("Mouse Y") * CameraMovementSpeed, 0));
		}
		if(Input.GetMouseButtonDown(0)){
            if (!IsUIHit())
            {
                this.MouseDownPoint = Input.mousePosition;
                this.LastMouseDownWasUIHit = false;
            }
            else
            {
                this.LastMouseDownWasUIHit = true;
            }
		}
		if (Input.GetMouseButtonUp (0)) {
            if (!this.LastMouseDownWasUIHit)
            {
                if (this.MouseDownPoint == null || (this.MouseDownPoint.Value - Input.mousePosition).magnitude <= 5f)
                {
                    RaycastHit hit;
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
                    if (Physics.Raycast(ray, out hit, 100f))
                    {
                        int x = Mathf.FloorToInt(hit.point.x);
                        int y = Mathf.FloorToInt(hit.point.z);
                        //					Debug.Log ("Hit at coords" + hit.point + "(" + x + "," + y + ")");

                        GameController.HighlightTile(x, y);
                    }
                }
                this.MouseDownPoint = null;
            }
		}

		//Camera.main.orthographicSize = Mathf.Clamp (Camera.main.orthographicSize, orthographicSizeMin, orthographicSizeMax);
	}

    bool IsUIHit()
    {
        PointerEventData pe = new PointerEventData(EventSystem.current);
        pe.position = Input.mousePosition;
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, hits);
        if (hits.Count > 0) Debug.Log("UI Hit!!!");
        return hits.Count > 0;
    }
    
}
