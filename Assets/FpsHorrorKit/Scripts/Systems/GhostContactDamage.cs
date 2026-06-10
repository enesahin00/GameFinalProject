namespace FpsHorrorKit
{
    using UnityEngine;

    public class GhostContactDamage : MonoBehaviour
    {
        [Header("Sanity Hasarı")]
        public float sanityDamage = 15f;

        [Header("Algılama")]
        [Tooltip("Oyuncuyu bu yarıçapta algılar (BoxCollider varsa otomatik kullanır)")]
        public float detectionRadius = 1f;

        [Header("Tekrar Hasar")]
        public bool damageOnce = false;
        [Tooltip("damageOnce kapalıysa bu kadar saniyede bir hasar tekrar verir")]
        public float damageInterval = 1.5f;

        private Transform player;
        private BoxCollider box;
        private bool hasDamaged;
        private float damageTimer;
        private bool playerInside;

        private void Awake()
        {
            enabled = false;
        }

        public void Activate()
        {
            enabled = true;
        }

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log($"[GhostDamage] Player bulundu: {playerObj.name}");
            }
            else
            {
                Debug.LogError("[GhostDamage] Player bulunamadı! Tag 'Player' olduğundan emin ol.");
            }

            box = GetComponent<BoxCollider>();
            Debug.Log($"[GhostDamage] BoxCollider: {(box != null ? "bulundu" : "YOK!")}");
        }

        private void Update()
        {
            if (player == null) return;
            if (damageOnce && hasDamaged) return;

            Debug.Log($"[GhostDamage] Mesafe: {Vector3.Distance(transform.position, player.position):F2}");

            bool isInside = IsPlayerInside();

            if (isInside && !playerInside)
            {
                playerInside = true;
                damageTimer = 0f;
                ApplyDamage();
            }
            else if (isInside && !damageOnce)
            {
                damageTimer += Time.deltaTime;
                if (damageTimer >= damageInterval)
                {
                    damageTimer = 0f;
                    ApplyDamage();
                }
            }
            else if (!isInside)
            {
                playerInside = false;
                damageTimer = 0f;
            }
        }

        private bool IsPlayerInside()
        {
            if (box != null)
            {
                Vector3 localPos = transform.InverseTransformPoint(player.position);
                Vector3 half = box.size * 0.5f;
                Vector3 c = box.center;
                return localPos.x > c.x - half.x && localPos.x < c.x + half.x &&
                       localPos.y > c.y - half.y && localPos.y < c.y + half.y &&
                       localPos.z > c.z - half.z && localPos.z < c.z + half.z;
            }

            return Vector3.Distance(transform.position, player.position) <= detectionRadius;
        }

        private void ApplyDamage()
        {
            hasDamaged = true;
            SanityManager.Instance?.ChangeSanity(-sanityDamage);
            Debug.Log($"[GhostDamage] Sanity düştü: -{sanityDamage}");
        }
    }
}
