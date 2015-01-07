using UnityEngine;
using System.Collections;

public class DisableOnStartup : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Disable on startup start");
        this.gameObject.SetActive(false);
	}
}
