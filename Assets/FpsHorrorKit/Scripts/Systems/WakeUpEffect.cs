namespace FpsHorrorKit
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.HighDefinition;

    public class WakeUpEffect : MonoBehaviour
    {
        [Header("Volume")]
        public Volume volume;

        [Header("Ayarlar")]
        public float distortionStart = -0.8f;
        public float clearDuration = 6f;
        public AnimationCurve clearCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        private LensDistortion lensDistortion;
        private ChromaticAberration chromaticAberration;
        private MotionBlur motionBlur;

        private void Awake()
        {
            if (volume == null) return;
            volume.weight = 0f;
            volume.profile.TryGet(out lensDistortion);
            volume.profile.TryGet(out chromaticAberration);
            volume.profile.TryGet(out motionBlur);
        }

        public void Play()
        {
            StartCoroutine(WakeUpRoutine());
        }

        private IEnumerator WakeUpRoutine()
        {
            if (volume != null) volume.weight = 1f;

            if (lensDistortion != null)
            {
                lensDistortion.active = true;
                lensDistortion.intensity.overrideState = true;
                lensDistortion.intensity.value = distortionStart;
            }
            if (chromaticAberration != null)
            {
                chromaticAberration.active = true;
                chromaticAberration.intensity.overrideState = true;
                chromaticAberration.intensity.value = 1f;
            }
            if (motionBlur != null)
            {
                motionBlur.active = true;
                motionBlur.intensity.overrideState = true;
                motionBlur.intensity.value = 1f;
            }

            float elapsed = 0f;
            while (elapsed < clearDuration)
            {
                elapsed += Time.deltaTime;
                float t = clearCurve.Evaluate(elapsed / clearDuration);

                if (lensDistortion != null)
                    lensDistortion.intensity.value = Mathf.Lerp(distortionStart, 0f, t);

                if (chromaticAberration != null)
                    chromaticAberration.intensity.value = Mathf.Lerp(1f, 0f, t);

                if (motionBlur != null)
                    motionBlur.intensity.value = Mathf.Lerp(1f, 0f, t);

                yield return null;
            }

            if (lensDistortion != null) lensDistortion.intensity.value = 0f;
            if (chromaticAberration != null) chromaticAberration.intensity.value = 0f;
            if (motionBlur != null) motionBlur.intensity.value = 0f;
        }
    }
}
