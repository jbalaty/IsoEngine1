using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public enum ELastMouseDownHit
{
    Nothing, UI, GameMap, GameObject
}

public class CameraController : MonoBehaviour
{




    //float speed = 1f;
    public float MinZoom = 1;
    public float MaxZoom = 10;
    public float CameraMovementSpeed = 0.5f;
    public GameController GameController;
    private Vector3? MouseDownPoint;
    private ELastMouseDownHit LastMouseDownHitType = ELastMouseDownHit.Nothing;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // ZOOMING
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        { // forward
            Camera.main.orthographicSize = Mathf.Max(MinZoom, Camera.main.orthographicSize - 0.25f);

        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        { // back
            Camera.main.orthographicSize = Mathf.Min(MaxZoom, Camera.main.orthographicSize + 0.25f);
        }

        // MOUSE MOVE
        if (Input.GetMouseButton(0) && Input.GetAxis("Mouse X") != 0
            && Input.GetAxis("Mouse Y") != 0)
        {
            if (this.LastMouseDownHitType == ELastMouseDownHit.Nothing || this.LastMouseDownHitType == ELastMouseDownHit.GameMap)
            {
                Camera.main.transform.Translate(
                    new Vector3(-Input.GetAxis("Mouse X") * CameraMovementSpeed,
                            -Input.GetAxis("Mouse Y") * CameraMovementSpeed, 0));
            }
            else if (this.LastMouseDownHitType == ELastMouseDownHit.GameObject)
            {
                GameController.MouseMove(new Vector2Int(Input.mousePosition),
                    new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            // store position
            this.MouseDownPoint = Input.mousePosition;
            if (IsUIHit())
            {
                // set UI hit flag for next frame
                this.LastMouseDownHitType = ELastMouseDownHit.UI;
            }
            this.LastMouseDownHitType = GameController.MouseDown(new Vector2Int(Input.mousePosition, EVectorComponents.XY));
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (this.LastMouseDownHitType == ELastMouseDownHit.GameObject || this.LastMouseDownHitType == ELastMouseDownHit.GameMap)
            {
                // if mouse down was near mouse up
                if (this.MouseDownPoint == null || (this.MouseDownPoint.Value - Input.mousePosition).magnitude <= 5f)
                {
                    GameController.MouseUp(new Vector2Int(Input.mousePosition));
                }
                else
                {
                    GameController.MouseUpAfterMove(new Vector2Int(Input.mousePosition));
                }
            }
            this.MouseDownPoint = null;
        }
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
