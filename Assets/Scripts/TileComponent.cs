using UnityEngine;
using System.Collections;

public enum EPrefabType {
    Tile, Object, Decoration
}

public class TileComponent : MonoBehaviour {

    public EPrefabType PrefabType = EPrefabType.Tile;
    public bool IsWalkable = true;
    public bool IsFlyable = true;
    

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
