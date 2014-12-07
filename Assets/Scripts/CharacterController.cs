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
		public Vector2Int? TargetTilePosition;
		public Vector2Int? NextTilePosition;

		// Use this for initialization
		void Start ()
		{
		}
	
		// Update is called once per frame
		void FixedUpdate ()
		{
			// if NextTile move to this tile
			// if we are on NextTile but not on TargetTile
			// 
			if(TargetTilePosition != null && TargetTilePosition != new Vector2Int(GetTilePositionVector())){

				var dir = TargetTilePosition.Value.Vector2 - GetTilePositionVector();
				if(Mathf.Abs(dir.x)>Mathf.Abs(dir.y)) dir.y=0f;
				else dir.x = 0f;

				direction = new Vector3(dir.x,0f,dir.y).normalized;
				// actual position + 0.5 to center the position to actual tile + half of direction vector
				// this is needed for the algorithm to work in every direction
				var newPosition = transform.position + new Vector3 (0.5f, 0f, 0.5f) + direction/2f;
				if (gameController.tilesGrid.GetIsWalkable (new Vector2Int (newPosition,EVectorComponents.XZ))) {
					//transform.Translate (newPosition);
					transform.Translate (direction * speed * Time.deltaTime);
					var diff = (TargetTilePosition.Value.Vector2 - GetTilePositionVector());
					if(diff.magnitude<= 0.1){
						transform.position = new Vector3(TargetTilePosition.Value.x,0f,TargetTilePosition.Value.y);
						TargetTilePosition = null;
						NextTilePosition = null;
					}
				} else {
					// rotate and try again
					if (Random.value <= 0.5f) {
						direction = new Vector3 (direction.z, direction.y, -direction.x);
					} else {
						direction = new Vector3 (-direction.z, direction.y, direction.x);
					}
						
				}
			} else {
				TargetTilePosition = null;
				NextTilePosition = null;
			}
		}

		public Vector2 GetTilePositionVector(){
			return new Vector2(transform.position.x, transform.position.z);
		}
	}
}
