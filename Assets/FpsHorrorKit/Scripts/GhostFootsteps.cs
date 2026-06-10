using UnityEngine;
using System.Collections;

namespace FpsHorrorKit
{
    public class GhostFootsteps : MonoBehaviour
    {
        public AudioClip[] footstepSounds;
        public AudioSource audioSource;
        public float minInterval = 3f;
        public float maxInterval = 8f;
        public float minDistance = 2f;
        public float maxDistance = 8f;

        Transform player;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            StartCoroutine(PlayRandomFootsteps());
        }

        IEnumerator PlayRandomFootsteps()
        {
            while (true)
            {
                float wait = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(wait);

                if (footstepSounds.Length == 0) continue;

                float distance = Random.Range(minDistance, maxDistance);
                float volume = Mathf.Lerp(0.7f, 0.4f, (distance - minDistance) / (maxDistance - minDistance));

                int stepCount = Random.Range(3, 6);
                for (int i = 0; i < stepCount; i++)
                {
                    AudioClip clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
                    audioSource.volume = volume;
                    audioSource.PlayOneShot(clip);
                    yield return new WaitForSeconds(0.7f);
                }
            }
        }
    }
}