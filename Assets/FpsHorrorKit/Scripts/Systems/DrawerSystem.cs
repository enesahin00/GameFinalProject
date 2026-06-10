 namespace FpsHorrorKit
{
    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class DrawerSystem : MonoBehaviour, IInteractable
    {
        [Header("Interact Settings")]
        [SerializeField] private string interactText = "Drag";

        [Header("Move Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float maxPosition = 0f;

        [Header("Audio Settings")]
        [Tooltip("Sürüklerken çıkacak ahşap sürtünme/tekerlek sesi")]
        [SerializeField] private AudioClip dragSound;
        [Tooltip("Tamamen kapandığında çıkacak çarpma sesi")]
        [SerializeField] private AudioClip closeSound;
        
        [Range(0f, 1f)] public float dragVolume = 0.5f;
        [Range(0f, 1f)] public float closeVolume = 1f;

        private float startPositionZ = 0f;
        private Collider _collider;

        // Ses sistemi için değişkenler
        private AudioSource audioSource;
        private bool isDraggingThisFrame = false;
        private bool hasPlayedCloseSound = true;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            startPositionZ = transform.localPosition.z;

            // AudioSource ayarları
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // Sesi objeye (3D) sabitler
            audioSource.loop = true;       // Sürtünme sesi döngüde kalır
            audioSource.clip = dragSound;
            audioSource.volume = 0f;       // Başlangıçta sessiz
            
            if (dragSound != null) 
            {
                audioSource.Play();
            }
        }

        private void Update()
        {
            // Sürükleme Sesini Yumuşatma (Fade In / Fade Out)
            if (isDraggingThisFrame)
            {
                // Çekmece hareket ediyorsa sesi hızla aç
                audioSource.volume = Mathf.Lerp(audioSource.volume, dragVolume, Time.deltaTime * 15f);
            }
            else
            {
                // Hareket durduysa sesi yavaşça kıs (pürüzsüz duruş)
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, Time.deltaTime * 10f);
            }

            // Her frame sonunda sürükleme durumunu sıfırla
            isDraggingThisFrame = false;
        }

        public void HoldInteract()
        {
            if (_collider != null)
            {
                _collider.enabled = false;
            }

            // Fare hareketine göre pozisyon hesapla
            float rotationInput = Input.GetAxis("Mouse Y") * -moveSpeed * Time.deltaTime;
            float currentPos = transform.localPosition.z;
            
            // Konum değişmeden önceki hali
            float oldPos = currentPos;

            // Çekmecenin limitleri
            float clampedPos = Mathf.Clamp(currentPos + rotationInput, startPositionZ, maxPosition);
            
            // Çekmece gerçekten hareket ediyorsa (sınırlara dayanmadıysa)
            if (Mathf.Abs(clampedPos - oldPos) > 0.001f)
            {
                isDraggingThisFrame = true;
            }

            // KAPANMA SESİ KONTROLÜ
            // Çekmece başlangıç noktasına (kapalı pozisyona) çok yakınsa ve ses çalmadıysa
            if (Mathf.Abs(clampedPos - startPositionZ) <= 0.01f && !hasPlayedCloseSound)
            {
                hasPlayedCloseSound = true;
                if (closeSound != null)
                {
                    audioSource.PlayOneShot(closeSound, closeVolume);
                }
            }
            // Çekmece biraz açıldığında kilidi aç ki tekrar kapanırken ses çıkarsın
            else if (Mathf.Abs(clampedPos - startPositionZ) > 0.05f)
            {
                hasPlayedCloseSound = false;
            }

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, clampedPos);
        }

        public void Highlight()
        {
            PlayerInteract.Instance.ChangeInteractText(interactText);
        }

        public void Interact() { }

        public void UnHighlight()
        {
            if (_collider != null)
            {
                _collider.enabled = true;
            }
        }
    }
}