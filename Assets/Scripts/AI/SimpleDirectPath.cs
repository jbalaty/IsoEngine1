using UnityEngine;
using System.Collections;

public interface IPathFinding
{
	void PlanPath (Vector2Int currentTile, Vector2Int targetTile);

	Vector2Int GetNextStep ();
}

public class SimpleDirectPath : IPathFinding
{

	public float speed = 0.3f;
	public Vector2 direction;
	public TileGridManager tilesGrid;
	public Vector2Int? TargetTilePosition;
	public Vector2Int? NextTilePosition;
	public Vector2Int? CurrentTilePosition;

	public void PlanPath (Vector2Int currentTile, Vector2Int targetTile)
	{
		this.TargetTilePosition = targetTile;
		this.CurrentTilePosition = currentTile;
	}
	
	// Update is called once per frame
	public Vector2Int GetNextStep ()
	{
		// if NextTile move to this tile
		// if we are on NextTile but not on TargetTile
		if (TargetTilePosition != null && TargetTilePosition != CurrentTilePosition) {
			
			var dir = TargetTilePosition.Value - CurrentTilePosition.Value;
			if (Mathf.Abs (dir.x) > Mathf.Abs (dir.y))
				dir.y = 0;
			else
				dir.x = 0;
			direction = dir.normalized;//new Vector3 (dir.x, 0f, dir.y).normalized;
			// actual position + 0.5 to center the position to actual tile + half of direction vector
			// this is needed for the algorithm to work in every direction
			var newPosition = CurrentTilePosition.Value.Vector2 + new Vector2 (0.5f, 0.5f) + direction / 2f;
			if (tilesGrid.GetIsWalkable (new Vector2Int (newPosition))) {
//				transform.Translate (direction * speed * Time.deltaTime);
//				var diff = (TargetTilePosition.Value.Vector2 - GetTilePositionVector ());
//				if (diff.magnitude <= 0.1) {
//					transform.position = new Vector3 (TargetTilePosition.Value.x, 0f, TargetTilePosition.Value.y);
//					TargetTilePosition = null;
//					NextTilePosition = null;
//				}
			} else {
				// rotate and try again
//				if (Random.value <= 0.5f) {
//					direction = new Vector3 (direction.z, direction.y, -direction.x);
//				} else {
//					direction = new Vector3 (-direction.z, direction.y, direction.x);
//				}
				
			}
		} else {
			TargetTilePosition = null;
			NextTilePosition = null;
		}
		return new Vector2Int (0, 0);
	}
	
//	public Vector2 GetTilePositionVector(){
//		return new Vector2(transform.position.x, transform.position.z);
//	}
}
