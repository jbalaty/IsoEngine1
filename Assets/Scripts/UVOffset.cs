using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UVOffset : MonoBehaviour
{
    public Vector2 TextureSize = new Vector2(8f, 11f);
    public Vector2 TextureOffset = new Vector2(0f, 0f);

    // Use this for initialization
    void OnRenderObject()
    {
        renderer.material.mainTextureScale = (new Vector2(1 / TextureSize.x, 1 / TextureSize.y));
        renderer.material.mainTextureOffset = (new Vector2(TextureOffset.x / TextureSize.x, TextureOffset.y / TextureSize.y));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
