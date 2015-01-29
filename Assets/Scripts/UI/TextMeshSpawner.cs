using UnityEngine;
using System.Collections;

namespace Dungeon
{
    public class TextMeshSpawner : MonoBehaviour
    {
        string SortingLayerName = "HelpersOverlay0";
        public static readonly float DefaultFadeOutTime = 1f;
        public float DelayBetweenSpawns = 0.1f;

        float _LastSpawnTime;

        int _TextMeshCounter = 0;

        // Use this for initialization
        void Start()
        {
            _LastSpawnTime = Time.time;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public TextMesh SpawnTextMesh(string text, Color color, float fadeOutSpeed = -1)
        {
            var timediff = Time.time - _LastSpawnTime;
            _LastSpawnTime = Time.time;
            Transform t = Instantiate(EntitiesManager.Instance.TextMesh) as Transform;
            t.name = "TextAboveTheHead_" + _TextMeshCounter++;
            t.parent = this.transform;
            t.localPosition = new Vector3(0f, 0.78f, 0f);
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            //var textmesh = go.AddComponent<TextMesh>();
            var textmesh = t.GetComponent<TextMesh>();
            textmesh.text = text;
            textmesh.color = color;
            textmesh.renderer.sortingLayerName = SortingLayerName;//"HelpersOverlay0";//SortingLayerName;

            //// debug - sorting layer names
            //string[] options = new string[32];
            //for (int i = 0; i < 32; i++)
            //{ // get layer names
            //    options[i] = i + " : " + LayerMask.LayerToName(i);

            //}
            //// debug end

            //textmesh.renderer.sortingLayerID = 8;
            //textmesh.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;//new Font("Arial");


            /* this does not work very well, because it could happen, 
             * that XP message shows before HP message, or last attack message from enemy
             * but better handling of combat events should resolve this
            if (timediff < DelayBetweenSpawns)
            {
                t.gameObject.SetActive(false);
                var wait = DelayBetweenSpawns - timediff;//Mathf.Max(DelayBetweenSpawns, timediff);
                StartCoroutine(IsoEngine1.Utils.WaitForSeconds(wait, () =>
                {
                    t.gameObject.SetActive(true);
                    if (fadeOutSpeed > 0)
                    {
                        StartCoroutine(FadeOutCoroutine(textmesh, fadeOutSpeed));
                    }
                }));
            }
            else*/
            {
                if (fadeOutSpeed > 0)
                {
                    StartCoroutine(FadeOutCoroutine(textmesh, fadeOutSpeed));
                }
            }


            return textmesh;
        }



        IEnumerator FadeOutCoroutine(TextMesh textMesh, float speed)
        {
            var c = textMesh.color;
            while (c.a > 0)
            {
                c.a -= speed * Time.deltaTime;
                textMesh.color = c;
                textMesh.transform.Translate(new Vector3(0f, speed * Time.deltaTime, 0f));
                yield return new WaitForEndOfFrame();
            }
            GameObject.Destroy(textMesh.gameObject);
        }
    }
}