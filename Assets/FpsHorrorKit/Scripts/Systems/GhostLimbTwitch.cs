namespace FpsHorrorKit
{
    using UnityEngine;

    public class GhostLimbTwitch : MonoBehaviour
    {
        [Header("Dönme Eksenleri")]
        public bool twitchX = true;
        public bool twitchY = false;
        public bool twitchZ = true;

        [Header("Açı Aralığı (Derece)")]
        public float minAngle = 15f;
        public float maxAngle = 45f;

        [Header("Hız Aralığı")]
        public float minSpeed = 0.8f;
        public float maxSpeed = 2.5f;

        [Header("Ani Sıçrama")]
        [Tooltip("Arada bir aniden farklı bir hedefe zıplar, daha ürkütücü görünüm için")]
        public bool enableJitter = true;
        [Range(0f, 1f)] public float jitterChance = 0.02f;

        private Vector3 baseRotation;
        private Vector3 targetRotation;
        private Vector3 currentRotation;

        private float speedX, speedY, speedZ;
        private float angleX, angleY, angleZ;
        private float timeX, timeY, timeZ;

        private void Awake()
        {
            baseRotation = transform.localEulerAngles;
            RandomizeParams();
        }

        private void RandomizeParams()
        {
            speedX = Random.Range(minSpeed, maxSpeed);
            speedY = Random.Range(minSpeed, maxSpeed);
            speedZ = Random.Range(minSpeed, maxSpeed);

            angleX = Random.Range(minAngle, maxAngle);
            angleY = Random.Range(minAngle, maxAngle);
            angleZ = Random.Range(minAngle, maxAngle);

            // Her eksen kendi fazından başlasın — hepsi aynı anda dönmesin
            timeX = Random.Range(0f, Mathf.PI * 2f);
            timeY = Random.Range(0f, Mathf.PI * 2f);
            timeZ = Random.Range(0f, Mathf.PI * 2f);
        }

        private void Update()
        {
            timeX += Time.deltaTime * speedX;
            timeY += Time.deltaTime * speedY;
            timeZ += Time.deltaTime * speedZ;

            float x = twitchX ? Mathf.Sin(timeX) * angleX : 0f;
            float y = twitchY ? Mathf.Sin(timeY) * angleY : 0f;
            float z = twitchZ ? Mathf.Sin(timeZ) * angleZ : 0f;

            // Ani sıçrama: düşük ihtimalle parametreleri yeniden rastgeleleştirir
            if (enableJitter && Random.value < jitterChance)
                RandomizeParams();

            transform.localEulerAngles = baseRotation + new Vector3(x, y, z);
        }
    }
}
