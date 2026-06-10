using UnityEngine;

namespace FpsHorrorKit
{
    public class MirrorInteractable : MonoBehaviour
    {
        public float sanityChange = -15f;
        public float cooldown = 3f;
        public float maxDistance = 3f;
        public AudioClip mirrorSound;

        AudioSource audioSource;
        float lastTriggerTime = -99f;
        Transform playerCam;

        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            playerCam = Camera.main.transform;
        }

        void Update()
        {
            if (Time.time - lastTriggerTime < cooldown) return;
            if (playerCam == null) return;

            // Mesafe kontrolü
            float dist = Vector3.Distance(transform.position, playerCam.position);
            if (dist > maxDistance) return;

            // Oyuncu aynanın ön yüzünde mi?
            Vector3 dirToPlayer = (playerCam.position - transform.position).normalized;
            float mirrorFacing = Vector3.Dot(transform.forward, dirToPlayer);
            if (mirrorFacing < 0.3f) return;

            // Oyuncu aynaya bakıyor mu?
            Vector3 dirToMirror = (transform.position - playerCam.position).normalized;
            float lookingAtMirror = Vector3.Dot(playerCam.forward, dirToMirror);
            if (lookingAtMirror < 0.7f) return;

            // Yansıma açısı — kamera forward ile ayna forward zıt olmalı
            float reflectionDot = Vector3.Dot(playerCam.forward, transform.forward);
            if (reflectionDot > -0.3f) return;

            // Tetikle
            lastTriggerTime = Time.time;
            SanityManager.Instance.ChangeSanity(sanityChange);

            if (mirrorSound != null)
                audioSource.PlayOneShot(mirrorSound);
        }
    }
}