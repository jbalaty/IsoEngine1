using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public interface IPathFinding
{
//	void Init(System.Func<Vector2Int, bool> walkableCallback);
	Path PlanPath (Vector2Int startTile, Vector2Int? endTile);
}

public class DirectPathfinding : IPathFinding
{

	public Vector2 direction;
	public Vector2Int? TargetTilePosition;
//	public Vector2Int? NextTilePosition;
	public Vector2Int CurrentTilePosition;
	System.Func<Vector2Int, bool> WalkableCallback;

	public DirectPathfinding(System.Func<Vector2Int, bool> walkableCallback){
		this.WalkableCallback = walkableCallback;
	}

	public Path PlanPath (Vector2Int currentTile, Vector2Int? targetTile)
	{
		Path path = new Path();
		if(currentTile == targetTile){
			return null;
		}
		this.TargetTilePosition = targetTile;
		this.CurrentTilePosition = currentTile;
		var step = GetNextStep();
		while(step.HasValue){
			path.AddLast(step.Value);
			step = GetNextStep ();
		}
		// if path is empty, there is no way to target point
		return path;
	}
	
	// Update is called once per frame
	public Vector2Int? GetNextStep ()
	{
		if (TargetTilePosition != null && TargetTilePosition.Value != CurrentTilePosition) {
			var dir = TargetTilePosition.Value - CurrentTilePosition;
			if (Mathf.Abs (dir.x) > Mathf.Abs (dir.y))
				dir.y = 0;
			else
				dir.x = 0;
			direction = dir.normalized;
			// actual position + 0.5 to center the position to actual tile + half of direction vector
			// this is needed for the algorithm to work in every direction
			var newPosition = CurrentTilePosition.Vector2 + new Vector2 (0.5f, 0.5f) + direction;
			var nextposition = new Vector2Int (newPosition);
			if ( this.WalkableCallback(nextposition)) {
				CurrentTilePosition = nextposition;
				return nextposition;
			} else {

				return null;
			}
		} else {
			TargetTilePosition = null;
			return null;
		}
	}


}
