using UnityEngine;
using System.Collections;

public class ObjectFollow : MonoBehaviour
{
    public Transform TargetObject;
    public Vector3 OffsetVector = new Vector3(0, 0, -4);

    // Update is called once per frame
    void Update()
    {
        if (TargetObject != null)
        {
            //this.transform.LookAt(TargetObject);
            this.transform.position = TargetObject.transform.position;
            this.transform.Translate(OffsetVector);
        }
    }
}
