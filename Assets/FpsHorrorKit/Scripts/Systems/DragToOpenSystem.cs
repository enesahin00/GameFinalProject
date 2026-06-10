namespace FpsHorrorKit
{
    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class DragToOpenSystem : MonoBehaviour, IInteractable
    {
        public enum DoorDirection { left, right, front, back }

        [Header("Rotation Settings")]
        [Tooltip("Mouse hareket hassasiyeti")]
        [SerializeField] private float rotationSpeed = 5f;

        [Tooltip("Kapının kapalı konumdaki açı değeri (örn. 0)")]
        [SerializeField] private float minAngle = 0f;

        [Tooltip("Kapının tamamen açıldığı açı (örn. 90)")]
        [SerializeField] private float maxAngle = 90f;

        [Tooltip("Kapı'nın player ile etkileşim durumunu kontrol eden yön")]
        [SerializeField] private DoorDirection doorDirection = DoorDirection.left;

        [Header("Collider Settings")]
        [SerializeField] private bool colliderDisabledDuringInteraction = false;

        [Header("Interact Text")]
        [SerializeField] private string interactText = "Drag";

        [Header("Audio Settings")]
        [Tooltip("Sürüklerken çıkacak gıcırtı/sürtünme sesi")]
        [SerializeField] private AudioClip dragSound;
        [Tooltip("Tamamen kapandığında çıkacak çarpma/kilit sesi")]
        [SerializeField] private AudioClip closeSound;
        
        [Range(0f, 1f)] public float dragVolume = 0.5f;
        [Range(0f, 1f)] public float closeVolume = 1f;

        private float currentAngle = 0f;
        private float initialAngle;
        private Vector3 initialForward;
        private Collider _collider;
        private Transform player;

        // Ses sistemi için gerekli değişkenler
        private AudioSource audioSource;
        private bool isDraggingThisFrame = false;
        private bool hasPlayedCloseSound = true; // Oyun başlarken kapalıysa tekrar çalmasın diye

        void Start()
        {
            _collider = GetComponent<Collider>();
            player = GameObject.FindGameObjectWithTag("Player").transform;

            initialAngle = transform.localEulerAngles.y;

            // AudioSource ayarları (Kodla otomatik yapıyoruz)
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // Sesi 3D yapar
            audioSource.loop = true;       // Sürükleme sesi sürekli dönecek
            audioSource.clip = dragSound;
            audioSource.volume = 0f;       // Başlangıçta sessiz
            
            if (dragSound != null) 
            {
                audioSource.Play();
            }

            // Kapının başlangıçtaki sağ yönünü saklıyoruz.
            switch (doorDirection)
            {
                case DoorDirection.left:
                    initialForward = -transform.right;
                    break;
                case DoorDirection.right:
                    initialForward = transform.right;
                    break;
                case DoorDirection.front:
                    initialForward = transform.forward;
                    break;
                case DoorDirection.back:
                    initialForward = -transform.forward;
                    break;
            }
        }

        void Update()
        {
            // Sürükleme Sesini Yumuşatma (Fade In / Fade Out)
            if (isDraggingThisFrame)
            {
                // Sürükleniyorsa sesi hızla aç
                audioSource.volume = Mathf.Lerp(audioSource.volume, dragVolume, Time.deltaTime * 15f);
            }
            else
            {
                // Sürüklenme bittiyse sesi yavaşça kıs (Kulak tırmalamayı önler)
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * 10f);
            }

            // Her frame sonunda sürükleme durumunu sıfırla. 
            // Eğer HoldInteract çalışıyorsa aşağıda tekrar true olacak.
            isDraggingThisFrame = false;
        }

        public void Interact()
        {
        }

        public void HoldInteract()
        {
            if (colliderDisabledDuringInteraction && _collider != null)
            {
                _collider.enabled = false;
            }
            
            float mouseX = Input.GetAxis("Mouse X");
            float sideMultiplier = 1f; 
            
            if (player != null)
            {
                Vector3 doorToPlayer = player.position - transform.position;
                float dot = Vector3.Dot(initialForward, doorToPlayer);
                sideMultiplier = (dot > 0) ? -1f : 1f;
            }

            // Açı değişmeden önceki hali
            float oldAngle = currentAngle;
            
            currentAngle += mouseX * rotationSpeed * sideMultiplier;
            currentAngle = Mathf.Clamp(currentAngle, minAngle, maxAngle);
            
            // Eğer açı gerçekten değişiyorsa (fare hareket ediyorsa ve kapı sınırlara dayanmadıysa)
            if (Mathf.Abs(currentAngle - oldAngle) > 0.01f)
            {
                isDraggingThisFrame = true;
            }

            // KAPANMA SESİ KONTROLÜ
            if (currentAngle <= minAngle + 0.1f && !hasPlayedCloseSound)
            {
                // Kapı minAngle'a ulaştı ve ses henüz çalmadıysa çal
                hasPlayedCloseSound = true;
                if (closeSound != null)
                {
                    audioSource.PlayOneShot(closeSound, closeVolume);
                }
            }
            // Kapı biraz aralandığında kilidi aç ki bir dahaki kapanışta tekrar ses çıkarabilsin
            else if (currentAngle > minAngle + 1f)
            {
                hasPlayedCloseSound = false;
            }

            float targetAngle = initialAngle + currentAngle;
            transform.localEulerAngles = new Vector3(0, targetAngle, 0);
        }

        public void Highlight()
        {
            PlayerInteract.Instance.ChangeInteractText(interactText);
        }

        public void UnHighlight()
        {
            if (_collider != null)
            {
                _collider.enabled = true;
            }
        }
    }
}