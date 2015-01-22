using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IsoEngine1;
using System;

namespace Dungeon
{
    public class GameLightManager : MonoBehaviour
    {

        public DungeonMapManager MapManager;
        float[,] CurrentLightModifiers;
        float[,] NextLightModifiers;
        public bool Shadows = false;

        // Use this for initialization
        void Start()
        {
            UpdateLight(new Vector2(this.transform.position.x, this.transform.position.z));

        }

        #region Light/Shadow procedures
        public void UpdateLight(Vector2 currentPosition)
        {
            //Debug.Log("Update Light");
            this.CurrentLightModifiers = ComputeLightModifiers(currentPosition);
            ApplyLightModifiers(this.CurrentLightModifiers);
        }
        public void StartLightBlending(Vector2Int nextPosition)
        {
            //Debug.Log("Start Light Blending");
            StopCoroutine("LightBlendingCouroutine");
            this.NextLightModifiers = ComputeLightModifiers(nextPosition.Vector2);
            //StartCoroutine(LightBlendingCouroutine(CharacterController.GetComponent<CharacterControllerScript>(),
            //    nextPosition));
            StartCoroutine("LightBlendingCouroutine", nextPosition);
        }
        IEnumerator LightBlendingCouroutine(Vector2Int nextPosition)
        {
            GameObject character = null;// needs some info about the player;
            //float[,] templm = new float[MapManager.SizeX, MapManager.SizeY];
            float[,] templm = new float[MapManager.SizeX, MapManager.SizeY];//this.CurrentLightModifiers.Clone() as float[,];
            Vector3 np = nextPosition.Vector3(EVectorComponents.XZ);
            float diff = (np - character.transform.position).magnitude;
            while (diff > 0.02f)
            {
                //if (this.CurrentLightModifiers != null && this.NextLightModifiers != null)
                {
                    for (var x = 0; x < templm.GetLength(0); x++)
                        for (var y = 0; y < templm.GetLength(1); y++)
                        //templm.ForEach((coords, val) =>
                        {
                            var from = this.CurrentLightModifiers[x, y];
                            var to = this.NextLightModifiers[x, y];
                            var lerp = Mathf.Lerp(from, to, 1f - diff);
                            //var lerp = Mathf.InverseLerp(from, to, 0.5f);
                            templm[x, y] = lerp;
                        };
                    this.CurrentLightModifiers = templm;
                    ApplyLightModifiers(templm);
                }
                diff = (np - character.transform.position).magnitude;
                yield return null;
            }
            this.NextLightModifiers = null;
        }
        public float[,] ComputeLightModifiers(Vector2 light)
        {
            float[,] result = new float[MapManager.SizeX, MapManager.SizeY];
            MapManager.ForEach((tile) =>
            {
                var m = (light - tile.Coordinates.Vector2).magnitude;
                var lightModifier = 1f;
                if (m < 12)
                {
                    var rayCastTiles = GetIntersectionTiles(new Vector2Int(light), tile.Coordinates);
                    lightModifier = ProcessIntersectionTiles(rayCastTiles) ? 4.6f : 0.8f;
                }
                else
                {
                    lightModifier = 1.8f;
                }
                //result[tile.Coordinates.x, tile.Coordinates.y] = lightModifier;
                //var v = Mathf.Sqrt(m) / 12f * lightModifier;
                var v = m / 36f * lightModifier;
                result[tile.Coordinates.x, tile.Coordinates.y] = v;
            });
            return result;
        }
        bool ProcessIntersectionTiles(List<Vector2Int> rayCastTiles)
        {
            bool hit = false;
            var counter = 0;
            foreach (var c in rayCastTiles)
            {
                var t = MapManager.GetTile(c);
                if (t != null)
                {
                    foreach (var go in t.GridObjectReferences)
                    {
                        if (go != null)
                        {
                            /*if (go.GameObject.tag == "Floor")
                            {
                                var sprites = go.GameObject.GetComponentsInChildren<SpriteRenderer>();
                                foreach (var sprite in sprites)
                                {
                                    sprite.color = hit ? Color.black: Color.white;
                                }
                            }*/
                            //lightModifiers[t.Coordinates.x, t.Coordinates.y] = hit ? 2f : 1f;
                            if (go.GameObject.tag == "Wall"
                                && counter < rayCastTiles.Count - 1) // last tile cannot cast shadow on itself
                            {
                                hit = true;
                                return true;
                            }
                        }
                    }
                }
                counter++;
            }
            return hit;
        }
        List<Vector2Int> GetIntersectionTiles(Vector2Int start, Vector2Int end)
        {
            var result = new List<Vector2Int>();

            var x0 = start.x;
            var y0 = start.y;
            var x1 = end.x;
            var y1 = end.y;
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int x = x0;
            int y = y0;
            int n = 1 + dx + dy;
            int x_inc = (x1 > x0) ? 1 : -1;
            int y_inc = (y1 > y0) ? 1 : -1;
            int error = dx - dy;
            dx *= 2;
            dy *= 2;

            for (; n > 0; --n)
            {
                //visit(x, y);
                result.Add(new Vector2Int(x, y));
                if (error > 0)
                {
                    x += x_inc;
                    error -= dy;
                }
                else
                {
                    y += y_inc;
                    error += dx;
                }
            }

            return result;
        }
        void ApplyLightModifiers(float[,] lightModifiers)
        {
            if (this.Shadows)
            {
                MapManager.ForEach((tile) =>
                {
                    var lm = lightModifiers[tile.Coordinates.x, tile.Coordinates.y];
                    foreach (var go in tile.GridObjectReferences)
                    {
                        if (go != null)
                        {
                            var sprites = go.GameObject.GetComponentsInChildren<SpriteRenderer>();
                            foreach (var sprite in sprites)
                            {

                                sprite.color = Color.Lerp(Color.white, Color.black, lm);
                            }
                        }
                    }
                });

                // apply to entities too
                var entities = GameObject.FindGameObjectsWithTag("Entity");
                foreach (var entity in entities)
                {
                    var coord = new Vector2Int(entity.transform.position, EVectorComponents.XZ);
                    var lm = lightModifiers[coord.x, coord.y];
                    var sprites = entity.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var sprite in sprites)
                    {
                        //lm = lm > 0.25f ? Mathf.Pow(lm, 1f / 3f) : lm;
                        lm = Mathf.Pow(lm, 0.7f);
                        sprite.color = Color.Lerp(Color.white, Color.black, lm);
                        // hide entity
                    }

                }
            }
        }
        #endregion
    }
}