using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TextMeshZOrder : MonoBehaviour
{
    public string SortingLayerName = "Overlay 0";
    public float FadeOutTime = 1f;
    

    // Use this for initialization
    void Start()
    {
        this.renderer.sortingLayerName = SortingLayerName;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
