using UnityEngine;
using System.Collections;

namespace Dungeon
{


    public class HealthBar : MonoBehaviour
    {
        public Transform HealthBarPrefab;
        Transform _HealthBar;
        float _StartXScale;
        Combat Combat;

        void Awake()
        {
            Combat = this.GetComponent<Combat>();
        }

        // Use this for initialization
        void Start()
        {
            _HealthBar = GameObject.Instantiate(HealthBarPrefab) as Transform;
            _HealthBar.name = "HealthBar";
            _HealthBar.SetParent(this.transform, false);
            _HealthBar.renderer.sortingLayerID = 8;
            _StartXScale = _HealthBar.localScale.x;
            //var ls = HealthBar.localScale;
            //ls.y = 0.2f;
            //_HealthBar.localScale = ls;
        }

        // Update is called once per frame
        void Update()
        {
            if (Combat.CurrentHitPoints > 0 && Combat.CurrentHitPoints < Combat.MaxHitPoints)
            {
                _HealthBar.gameObject.SetActive(true);
                var ls = _HealthBar.localScale;
                var h = Combat.CurrentHitPoints / (float)Combat.MaxHitPoints;
                var newColor = Color.Lerp(Color.red, Color.green, h);
                ls.x = h * _StartXScale;
                _HealthBar.GetComponent<SpriteRenderer>().color = newColor;
                _HealthBar.localScale = ls;
            }
            else
            {
                _HealthBar.gameObject.SetActive(false);
            }
        }
    }
}