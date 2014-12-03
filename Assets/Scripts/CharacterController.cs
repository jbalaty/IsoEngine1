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

		// Use this for initialization
		void Start ()
		{
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
			var newPosition = transform.position + direction * speed * Time.deltaTime;
			if (gameController.tilesGrid.GetIsWalkable (new Vector2 (newPosition.x, newPosition.z))) {
				//transform.Translate (newPosition);
				transform.position = newPosition;
			} else {
				// rotate and try again
				if (Random.value <= 0.5f) {
					direction = new Vector3 (direction.z, direction.y, -direction.x);
				} else {
					direction = new Vector3 (-direction.z, direction.y, direction.x);
				}
			}
		}
	}
}
