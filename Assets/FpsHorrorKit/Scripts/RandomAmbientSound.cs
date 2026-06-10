using UnityEngine;
using System.Collections;

namespace FpsHorrorKit
{
    public class RandomAmbientSound : MonoBehaviour
    {
        [Header("Ses Dosyaları")]
        public AudioClip[] ambientSounds;
        
        [Header("Zamanlama (Saniye)")]
        public float minWaitTime = 10f;
        public float maxWaitTime = 40f;

        [Header("Ses Ayarları")]
        public AudioSource audioSource;
        [Range(0f, 1f)] public float volumeLevel = 1f;
        public bool randomizePitch = true;

        private void Start()
        {
            audioSource.spatialBlend = 1f;
            audioSource.playOnAwake = false;
            StartCoroutine(PlaySoundRoutine());
        }

        private IEnumerator PlaySoundRoutine()
        {
            while (true)
            {
                float waitTime = Random.Range(minWaitTime, maxWaitTime);
                yield return new WaitForSeconds(waitTime);

                if (ambientSounds != null && ambientSounds.Length > 0)
                {
                    int index = Random.Range(0, ambientSounds.Length);
                    audioSource.pitch = randomizePitch ? Random.Range(0.85f, 1.15f) : 1f;
                    audioSource.PlayOneShot(ambientSounds[index], volumeLevel);
                }
            }
        }
    }
}