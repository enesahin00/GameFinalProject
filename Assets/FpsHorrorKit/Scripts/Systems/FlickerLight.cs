namespace FpsHorrorKit
{
    using UnityEngine;

    public class FlickerLight : MonoBehaviour
    {
        [Header("Titreme Hızı")]
        public float flickerSpeed = 0.05f;

        [Header("Ani Sönme")]
        public bool randomBlackout = true;
        [Range(0f, 1f)] public float blackoutChance = 0.03f;
        public float blackoutDuration = 0.1f;

        private Light _light;
        private float nextFlicker;
        private float blackoutTimer;
        private bool inBlackout;

        private void Awake()
        {
            _light = GetComponent<Light>();
            if (_light == null)
                Debug.LogError("[FlickerLight] Light component bulunamadı!");

            enabled = false;
            if (_light != null) _light.enabled = false;
        }

        public void Activate()
        {
            enabled = true;
            if (_light != null) _light.enabled = true;
        }

        private void Update()
        {
            if (_light == null) return;

            if (inBlackout)
            {
                blackoutTimer -= Time.deltaTime;
                if (blackoutTimer <= 0f)
                {
                    inBlackout = false;
                    _light.enabled = true;
                }
                return;
            }

            if (Time.time >= nextFlicker)
            {
                nextFlicker = Time.time + flickerSpeed + Random.Range(0f, flickerSpeed);

                if (randomBlackout && Random.value < blackoutChance)
                {
                    inBlackout = true;
                    blackoutTimer = blackoutDuration;
                    _light.enabled = false;
                }
                else
                {
                    _light.enabled = !_light.enabled;
                }
            }
        }
    }
}
