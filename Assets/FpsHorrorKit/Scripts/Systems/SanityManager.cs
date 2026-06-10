using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Cinemachine;

namespace FpsHorrorKit
{
    public class SanityManager : MonoBehaviour
    {
        public static SanityManager Instance;

        [Header("Core Settings")]
        [Range(0, 100)] public float sanity = 100f;
        public Volume sanityVolume;
        public FpsController fpsController;

        [Header("Sanity Audio")]
        public AudioSource sanityAudioSource;
        public AudioClip panicSound;

        [Header("Dizziness (Baş Dönmesi)")]
        public CinemachineCamera virtualCamera; 

        float panicWeight = 0f;
        bool isPanicking = false;
        float currentTilt = 0f;
        bool badEndingTriggered = false;
        readonly WaitForSeconds badEndingDelay = new(5f);

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            if (sanityVolume != null)
                sanityVolume.weight = 0f;
        }

        void Update()
        {
            // --- 1. KALICI KARARTMA (GÜNCELLENMİŞ MATEMATİK) ---
            float permanentT = 0f;
            if (sanity <= 60f)
            {
                // Geçişin pürüzsüz olması için alt sınırı da 0.6'dan 0.65'e çektik
                permanentT = Mathf.Lerp(0.7f, 1f, 1f - (sanity / 30f));
            }
            else if (sanity <= 80f)
            {
                // 80'a düştüğü an o sevdiğin hafif karartı (0.4) başlar ve 30'a doğru 0.65'e ulaşır
                permanentT = Mathf.Lerp(0.5f, 0.75f, 1f - ((sanity - 30f) / 30f));
            }

            if (sanityVolume != null)
            {
                sanityVolume.weight = Mathf.Clamp01(permanentT + panicWeight);
            }

            // --- 2. SARSINTI (AMPLITUDE) MATEMATİĞİ ---
            if (fpsController != null)
            {
                float permanentShake = sanity < 60f
                    ? Mathf.Lerp(0f, 2f, 1f - (sanity / 60f))
                    : 0f;

                fpsController.sanityAmplitudeOffset = permanentShake + (panicWeight * 10f);
                fpsController.sanityFrequencyOffset = 0.3f + (panicWeight * 0.8f);
            }

            // --- 3. BAŞ DÖNMESİ (UNITY 6 LENS DUTCH) ---
            if (virtualCamera != null)
            {
                float targetTilt = 0f;

                if (sanity < 60f)
                {
                    float dizzySpeed = Mathf.Lerp(0.5f, 1.5f, 1f - (sanity / 60f));
                    float dizzyAngle = Mathf.Lerp(0f, 4f, 1f - (sanity / 60f)); 
                    targetTilt = Mathf.Sin(Time.time * dizzySpeed) * dizzyAngle;
                }

                if (isPanicking)
                {
                    targetTilt += Mathf.Sin(Time.time * 20f) * (panicWeight * 6f);
                }

                currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * 5f);

                var lens = virtualCamera.Lens;
                lens.Dutch = currentTilt;
                virtualCamera.Lens = lens;
            }
        }

        public void ChangeSanity(float amount)
        {
            sanity = Mathf.Clamp(sanity + amount, 0f, 100f);

            if (amount < 0f && !isPanicking)
                StartCoroutine(PanicEffect());

            if (sanity < 50f && !badEndingTriggered)
            {
                badEndingTriggered = true;
                StartCoroutine(LoadBadEnding());
            }
        }

        private IEnumerator LoadBadEnding()
        {
            yield return badEndingDelay;
            SceneManager.LoadScene(3);
        }

        IEnumerator PanicEffect()
        {
            isPanicking = true;

            if (sanityAudioSource != null && panicSound != null)
            {
                sanityAudioSource.PlayOneShot(panicSound);
            }

        
            float duration = 4f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                // İlk 0.8 saniyede (4'ün %20'si) zirveye vurur, kalan 3.2 saniyede yavaşça dağılır
                panicWeight = t < 0.2f
                    ? Mathf.Lerp(0f, 0.5f, t / 0.2f)
                    : Mathf.Lerp(0.5f, 0f, (t - 0.2f) / 0.8f);
                    
                yield return null;
            }

            panicWeight = 0f;
            isPanicking = false;
        }

        public bool IsGoodEnding() => sanity >= 60f;
    }
}