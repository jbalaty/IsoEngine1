using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IsoEngine1;



namespace IsoEngine1.Components
{

    class DraggableComponent : IDraggable
    {
        Vector2Int DragStartPosition;
        GridObject GridObject;
        GridObjectMultiSprite Indicator;
        Transform TileIndicatorPrefab;
        Vector2Int dragOffset;
        public bool IsDragged { get; set; }

        public DraggableComponent(GridObject go, Transform tileidicatorprefab)
        {
            this.GridObject = go;
            this.TileIndicatorPrefab = tileidicatorprefab;
        }

        public void OnDragStart(Vector2Int dragPosition)
        {
            Debug.Log("OnDragStart- " + this.GridObject.Name);
            this.DragStartPosition = GridObject.MainTile.Coordinates;
            this.dragOffset = dragPosition - GridObject.MainTile.Coordinates;
            this.Indicator = new GridObjectMultiSprite("SelectedTileIndicator", this.TileIndicatorPrefab, Vector2.zero);
            this.GridObject.GridManager.SetupObject(this.GridObject.OccupiedTiles[0, 0].Coordinates,
                ETileLayer.Overlay0.Int(), this.Indicator, this.GridObject.Size);
            var c = Color.green; c.a = .7f;
            this.Indicator.SetColor(c);
            if (this.GridObject is GridObjectSpriteSDGameObject)
            {
                (this.GridObject as GridObjectSpriteSDGameObject).SetColor(c);
            }
            this.GridObject.Move(this.GridObject.MainTile.Coordinates, ETileLayer.Overlay1.Int());
        }

        public void OnDragMove(Vector2Int newCoords)
        {
            newCoords = newCoords - this.dragOffset;
            Debug.Log("OnDragMove- " + this.GridObject.Name);
            this.GridObject.Move(newCoords, ETileLayer.Overlay1.Int());
            this.Indicator.Move(newCoords, null);
            Color c;
            c = Color.green;
            c.a = .7f;
            this.Indicator.SetColor(c);
            var coltiles = this.GridObject.GridManager.GetCollisionTiles(newCoords, this.GridObject.Size,
                ETileLayer.Object0.Int(), this.GridObject);
            if (coltiles.Count > 0)
            {
                c = Color.red;
                c.a = .7f;
                foreach (var ct in coltiles)
                {
                    this.Indicator.SetColor(ct.RelativeCoordinates, c);
                }
            }
        }

        public void OnDragEnd()
        {
            Debug.Log("OnDragEnd- " + this.GridObject.Name);
            var coltiles = this.GridObject.GridManager.GetCollisionTiles(this.GridObject.MainTile.Coordinates,
                this.GridObject.Size, ETileLayer.Object0.Int(), this.GridObject);
            if (this.GridObject is GridObjectSpriteSDGameObject)
            {

                var c = (this.GridObject as GridObjectSpriteSDGameObject).SetColor(Color.white);
                c.a = 1f;
                (this.GridObject as GridObjectSpriteSDGameObject).SetColor(c);
            }
            // if there is some collision prevent end of dragging
            if (coltiles.Count == 0)
            {
                this.GridObject.GridManager.DestroyObject(this.Indicator);
                this.GridObject.Move(this.GridObject.MainTile.Coordinates, ETileLayer.Object0.Int());
            }
            else
            {
                // return at start position
                this.GridObject.GridManager.DestroyObject(this.Indicator);
                this.GridObject.Move(this.DragStartPosition, ETileLayer.Object0.Int());
            }
        }
    }
}
