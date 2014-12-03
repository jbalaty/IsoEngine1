using UnityEngine;
using System.Collections;

public class BasicObstacleAvoidance : MonoBehaviour {

	public float minimumDistAvoidance = 3f;

	private Vector2 targetPoint;

	// Use this for initialization
	void Start () {
		targetPoint = Vector2.zero;
	}
	
	void FixedUpdate () {
	
	}
}
