namespace FpsHorrorKit
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class DoorSystem : MonoBehaviour, IInteractable
    {
        [Header("Highlight UI")]
        [SerializeField] private string interactText = "Door Open/Close [E]";
        [SerializeField] private string doorLockedText = "Find Key";
        [SerializeField] private string useKeyText = "Use Key";

        [Header("Door Settings")]
        public bool isLocked;
        public List<string> requiredKeyIDs;
        public float rotationSpeed = 100f;
        public float endRotation;
        
        [Header("Ses Ayarları (Audio)")]
        public AudioSource doorAudioSource;
        public AudioClip openSound;    // Kapı AÇILIRKEN çıkan gıcırtı
        public AudioClip closeSound;   // Kapı KAPANIRKEN çıkan gıcırtı (Buna da openSound'u koyabilirsin)
        public AudioClip slamSound;    // Kapı TAM KAPANDIĞINDA çıkacak çarpma (Slam) sesi

        private float startRotation = 0;
        private bool isFinished = false;
        private bool isOpen;

        private void Start()
        {
            isFinished = true;
            startRotation = transform.localEulerAngles.y;
        }

        bool AllKeysCollected()
        {
            foreach (string id in requiredKeyIDs)
                if (!KeyManager.Instance.HasKey(id)) return false;
            return true;
        }

        public void Interact()
        {
            if (isLocked)
            {
                if (AllKeysCollected()) isLocked = false;
                else return;
            }

            if (!isOpen && isFinished)
            {
                // false: Kapı AÇILIYOR
                StartCoroutine(OpenDoor(endRotation, false));
                isOpen = true;
            }
            else if (isOpen && isFinished)
            {
                // true: Kapı KAPANIYOR
                StartCoroutine(OpenDoor(startRotation, true));
                isOpen = false;
            }
        }

        public void Highlight()
        {
            if (isLocked && AllKeysCollected())
                PlayerInteract.Instance.ChangeInteractText(useKeyText);
            else if (isLocked)
                PlayerInteract.Instance.ChangeInteractText(doorLockedText);
            else
                PlayerInteract.Instance.ChangeInteractText(interactText);
        }

        IEnumerator OpenDoor(float targetRotation, bool isClosing)
        {
            isFinished = false;
            
            // Kapı harekete başladığında çalacak sesi seç
            AudioClip moveSound = isClosing ? closeSound : openSound;
            
            if (doorAudioSource != null && moveSound != null)
            {
                doorAudioSource.clip = moveSound;
                doorAudioSource.Play();
            }

            // Kapının dönme animasyonu (Fizik döngüsü)
            while (Mathf.Abs(Mathf.DeltaAngle(transform.localEulerAngles.y, targetRotation)) > 0.1f)
            {
                float step = rotationSpeed * Time.deltaTime;
                float newY = Mathf.MoveTowardsAngle(transform.localEulerAngles.y, targetRotation, step);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newY, transform.localEulerAngles.z);
                yield return null;
            }

            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, targetRotation, transform.localEulerAngles.z);
            isFinished = true;

            // KAPI TAMAMEN KAPANDI: Slam (Çarpma) sesini üstüne ekleyerek çal
            if (isClosing && doorAudioSource != null && slamSound != null)
            {
                // PlayOneShot kullanıyoruz ki, eğer gıcırtı sesi hala bitmediyse onu kesmeden üstüne vuruş sesini bindirsin
                doorAudioSource.PlayOneShot(slamSound);
            }
        }

        public void HoldInteract() { }
        public void UnHighlight() { }
    }
}