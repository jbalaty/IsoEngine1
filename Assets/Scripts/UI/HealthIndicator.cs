using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Dungeon
{
    public class HealthIndicator : MonoBehaviour
    {
        public Slider HealthSlider;
        public Text HealtText;
        public Image HeartImage;
        int _LastCurrentHitpoints = 0;
        bool _AnimatingHeart;
        bool _firstRun = true;

        // Use this for initialization
        void Awake()
        {
        }

        // Update is called once per frame
        //void Update()
        //{
        //    this.HealthIndicatorSlider.maxValue = Combat.MaxHitPoints;
        //    this.HealthIndicatorSlider.value = Combat.CurrentHitPoints;
        //}
        public void SetHealth(int currentHitpoints, int maxHitpoints)
        {
            if (currentHitpoints != _LastCurrentHitpoints && !_AnimatingHeart && !_firstRun)
            {

                //HeartImage.transform.ScaleTo(new Vector3(2f, 2f, 2f), 1.5f);
                var scaleSize = Mathf.Max(Mathf.Abs(currentHitpoints - _LastCurrentHitpoints) / maxHitpoints, 0.11f);
                StartCoroutine(AnimHeartCoroutine(2.0f, scaleSize));
            }
            HealthSlider.maxValue = maxHitpoints;
            HealthSlider.value = currentHitpoints;
            HealtText.text = currentHitpoints + "/" + maxHitpoints;
            _LastCurrentHitpoints = currentHitpoints;
            _firstRun = false;
        }

        public IEnumerator AnimHeartCoroutine(float speed, float size)
        {
            this._AnimatingHeart = true;
            var startTime = Time.time;
            while (HeartImage.transform.localScale.x <= 1 + size)
            {
                var ls = HeartImage.transform.localScale;
                HeartImage.transform.localScale = new Vector3(ls.x + speed * Time.deltaTime, ls.y + speed * Time.deltaTime, 0f);
                yield return new WaitForFixedUpdate();
            }
            while (HeartImage.transform.localScale.x > 1f)
            {
                var ls = HeartImage.transform.localScale;
                HeartImage.transform.localScale = new Vector3(ls.x - speed * Time.deltaTime, ls.y - speed * Time.deltaTime, 0f);
                yield return new WaitForFixedUpdate();
            }
            this._AnimatingHeart = false;
        }
    }
}