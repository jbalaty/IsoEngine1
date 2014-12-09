﻿using UnityEngine;
using System.Collections;

namespace IsoEngine1
{
	public class CharacterController : MonoBehaviour
	{

		public float speed = 0.3f;
		public GameController gameController;
		bool IsMoving = false;
		IPathFinding PathFinding;
		Vector2Int? TargetTile;
		Path currentPath;

		// Use this for initialization
		void Start ()
		{
			Debug.Log("CharacterController start");
		}
	
		public void SetTargetTile (Vector2Int? targettile)
		{
			this.TargetTile = targettile;
			currentPath = gameController.TilesGrid.FindPath (new Vector2Int (this.transform.position, EVectorComponents.XZ), targettile.Value);
		}

		// Update is called once per frame
		void FixedUpdate ()
		{
			if (!this.IsMoving && currentPath != null) {
				if (currentPath.IsEmpty ()){
					if(this.TargetTile != null && this.GetTilePosition () != TargetTile) {
						StartCoroutine (JumpJump (.5f));
						currentPath = null;
					} 
				} else {
					// follow the path, but check everytime if the tile is really empty
					var nextposition = currentPath.PopFirst ();
                    // DEBUG CODE
                    var prev = nextposition;
					foreach(var n in currentPath){
                        Debug.DrawLine(prev.Vector3(EVectorComponents.XZ) + new Vector3(.5f, 0f, .5f), n.Vector3(EVectorComponents.XZ) + new Vector3(.5f, 0f, .5f), Color.yellow, 0.2f);
						prev = n;
					} 
                    // DEBUG END
					if (currentPath != null && gameController.TilesGrid.GetIsWalkable (nextposition)) {
						//start movement to next tile
//						Debug.Log ("Moving to next position " + nextposition);
						StartCoroutine (MoveToPosition (nextposition.Vector3 (EVectorComponents.XZ)));
					} else {
						StartCoroutine (JumpJump (.5f));
						currentPath = null;
					}
				}
			}
		}

		public IEnumerator MoveToPosition (Vector3 nexttcoords)
		{
			this.IsMoving = true;
			var diff = (nexttcoords - transform.position);
			while (diff.magnitude>.1f) {
				transform.Translate (diff.normalized * speed * Time.deltaTime);
				diff = (nexttcoords - transform.position);
				yield return  null;
			}
			transform.position = nexttcoords;
			this.IsMoving = false;
		}

		public IEnumerator JumpJump (float height)
		{
			this.IsMoving = true;
			while (transform.position.y<height) {
				transform.Translate (0f, speed * Time.deltaTime, 0f);
				yield return  null;
			}
			while (transform.position.y>0f) {
				transform.Translate (0f, -speed * Time.deltaTime, 0f);
				yield return  null;
			}
//			yield return new WaitForSeconds (.05f);
			while (transform.position.y<height) {
				transform.Translate (0f, speed * Time.deltaTime, 0f);
				yield return  null;
			}
			while (transform.position.y>0f) {
				transform.Translate (0f, -speed * Time.deltaTime, 0f);
				yield return  null;
			}
			var p = transform.position;
			p.y = 0f;
			transform.position = p;
			this.IsMoving = false;
		}

		public Vector2Int GetTilePosition ()
		{
			return new Vector2Int (transform.position, EVectorComponents.XZ);
		}
	}
}
