using UnityEngine;
using System.Collections;

namespace IsoEngine1
{
	public class CharacterController : MonoBehaviour
	{

		public float speed = 0.3f;
		public Vector3 direction;
		public GameController gameController;
		public TileGridManager tilesGrid;
		public Vector2? TargetTilePosition;
		bool IsMoving = false;
		// Use this for initialization
		void Start ()
		{
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
			// if NextTile move to this tile
			// if we are on NextTile but not on TargetTile

			if (this.IsMoving)
				return;

			if (TargetTilePosition != null && TargetTilePosition != GetTilePositionVector ()) {
				var dir = TargetTilePosition.Value - GetTilePositionVector ();
				if (Mathf.Abs (dir.x) > Mathf.Abs (dir.y))
					dir.y = 0f;
				else
					dir.x = 0f;
				direction = new Vector3 (dir.x, 0f, dir.y).normalized;
				// actual position + 0.5 to center the position to actual tile + half of direction vector
				// this is needed for the algorithm to work in every direction
				var newPosition = transform.position + new Vector3 (0.5f, 0f, 0.5f) + direction;
				var nextposition = new Vector3 ((int)newPosition.x, 0f, (int)newPosition.z);
				if (gameController.tilesGrid.GetIsWalkable (new Vector2 (nextposition.x, nextposition.z))) {
					// start movement to next tile
					Debug.Log ("Moving to next position " + nextposition);
					StartCoroutine (MoveToPosition (nextposition));
				} else {
					// try to find some other way
					StartCoroutine (JumpJump(.5f));
					TargetTilePosition = null;
				}
			} else {
				TargetTilePosition = null;
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
				transform.Translate (0f,speed * Time.deltaTime,0f);
				yield return  null;
			}
			while (transform.position.y>0f) {
				transform.Translate (0f,-speed * Time.deltaTime,0f);
				yield return  null;
			}
//			yield return new WaitForSeconds (.05f);
			while (transform.position.y<height) {
				transform.Translate (0f,speed * Time.deltaTime,0f);
				yield return  null;
			}
			while (transform.position.y>0f) {
				transform.Translate (0f,-speed * Time.deltaTime,0f);
				yield return  null;
			}
			var p = transform.position;
			p.y = 0f;
			transform.position = p;
			this.IsMoving = false;
		}

		public Vector2 GetTilePositionVector ()
		{
			return new Vector2 (transform.position.x, transform.position.z);
		}
	}
}
