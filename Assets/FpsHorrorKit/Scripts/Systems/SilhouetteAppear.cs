namespace FpsHorrorKit
{
    using System.Collections;
    using UnityEngine;

    public class SilhouetteAppear : MonoBehaviour
    {
        [Header("Görünürlük Ayarları")]
        public float appearDuration = 2f;

        [Header("Animasyon (İsteğe Bağlı)")]
        public bool fadeInOut = false;
        [Range(0.1f, 1f)] public float fadeDuration = 0.3f;

        [Header("Dönme Ayarları")]
        public bool rotate = true;
        public float rotateSpeed = 60f;
        public Vector3 rotateAxis = Vector3.up;

        private Renderer[] renderers;
        private Light[] lights;
        private AudioSource audioSource;
        private bool isVisible = false;
        private Coroutine activeRoutine;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            lights = GetComponentsInChildren<Light>();
            audioSource = GetComponent<AudioSource>();
            SetVisible(false);
        }

        private void Update()
        {
            if (rotate && isVisible)
                transform.Rotate(rotateAxis, rotateSpeed * Time.deltaTime, Space.Self);
        }

        public void Trigger()
        {
            if (activeRoutine != null)
                StopCoroutine(activeRoutine);

            activeRoutine = StartCoroutine(AppearRoutine());
        }

        private IEnumerator AppearRoutine()
        {
            if (fadeInOut)
            {
                yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
                yield return new WaitForSeconds(appearDuration);
                yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
            }
            else
            {
                SetVisible(true);
                yield return new WaitForSeconds(appearDuration);
                SetVisible(false);
            }

            activeRoutine = null;
        }

        private IEnumerator Fade(float from, float to, float duration)
        {
            SetVisible(true);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(from, to, elapsed / duration);
                SetAlpha(alpha);
                yield return null;
            }

            SetAlpha(to);
            if (to <= 0f) SetVisible(false);
        }

        private void SetVisible(bool visible)
        {
            foreach (var r in renderers)
                r.enabled = visible;

            foreach (var l in lights)
                l.enabled = visible;

            isVisible = visible;

            if (audioSource != null)
            {
                if (visible) audioSource.Play();
                else audioSource.Stop();
            }
        }

        private void SetAlpha(float alpha)
        {
            foreach (var r in renderers)
            {
                foreach (var mat in r.materials)
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
            }
        }
    }
}
