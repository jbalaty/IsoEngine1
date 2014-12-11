using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Tile Controller - Start");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
        Debug.Log("On tile OnMouseDown");
	}
	
}
