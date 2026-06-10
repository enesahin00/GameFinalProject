namespace FpsHorrorKit
{
    using UnityEngine;
    using UnityEngine.Events;

    public class GeneralTrigger : MonoBehaviour
    {
        public enum SanityComparison { Below, Above }

        [Header("Tetikleyiciler (birden fazla seçilebilir)")]
        public bool onTouch;
        public bool onStart;
        public bool onItemCollected;
        [Tooltip("ITOKey scriptindeki keyID ile eşleşmeli, obje adı değil")]
        public string itemID;
        public bool onSwitch;
        [Tooltip("Sanity eşiği geçilince otomatik tetikler — objeye ihtiyaç yok")]
        public bool onSanityThreshold;

        [Header("Sanity Koşulu / Eşiği")]
        public bool checkSanity;
        public SanityComparison sanityComparison = SanityComparison.Below;
        [Range(0, 100)] public float sanityThreshold = 60f;

        [Header("Parça Koşulu")]
        [Tooltip("Belirli sayıda parça incelenmeden tetiklenmesin")]
        public bool checkPieces;
        [Tooltip("En az bu kadar parça incelenmiş olmalı")]
        public int requiredPieceCount = 1;

        [Header("Genel Ayarlar")]
        public bool triggerOnce = true;

        [Header("Tetiklenince Çağrılacak Fonksiyon(lar)")]
        public UnityEvent onTrigger;

        private bool hasTriggered;
        private bool sanityThresholdCrossed;

        private void OnEnable()
        {
            if (onItemCollected)
                KeyManager.OnKeyCollected += OnKeyCollectedHandler;

            if (checkPieces)
                PieceManager.OnPieceFound += OnPieceFoundHandler;
        }

        private void OnDisable()
        {
            KeyManager.OnKeyCollected -= OnKeyCollectedHandler;
            PieceManager.OnPieceFound -= OnPieceFoundHandler;
        }

        private void Start()
        {
            if (onStart) TryTrigger();
        }

        private void Update()
        {
            if (!onSanityThreshold) return;
            if (triggerOnce && hasTriggered) return;

            var sm = SanityManager.Instance;
            if (sm == null) return;

            float s = sm.sanity;
            bool crossedNow = sanityComparison == SanityComparison.Below
                ? s < sanityThreshold
                : s > sanityThreshold;

            if (crossedNow && !sanityThresholdCrossed)
            {
                sanityThresholdCrossed = true;
                TryTrigger();
            }
            else if (!crossedNow && sanityThresholdCrossed)
            {
                sanityThresholdCrossed = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!onTouch) return;
            if (!other.CompareTag("Player")) return;
            TryTrigger();
        }

        private void OnKeyCollectedHandler(string id)
        {
            if (id != itemID) return;
            TryTrigger();
        }

        private void OnPieceFoundHandler(int foundCount)
        {
            if (!onSanityThreshold && !onStart && !onTouch && !onSwitch && !onItemCollected)
                TryTrigger();
        }

        public void FireFromSwitch()
        {
            if (!onSwitch) return;
            TryTrigger();
        }

        public void ManualTrigger() => TryTrigger();

        private void TryTrigger()
        {
            if (triggerOnce && hasTriggered) return;
            if (!CheckConditions()) return;

            hasTriggered = true;
            onTrigger?.Invoke();
        }

        private bool CheckConditions()
        {
            if (checkSanity)
            {
                if (SanityManager.Instance == null) return false;
                float s = SanityManager.Instance.sanity;
                if (sanityComparison == SanityComparison.Below && s >= sanityThreshold) return false;
                if (sanityComparison == SanityComparison.Above && s <= sanityThreshold) return false;
            }

            if (checkPieces)
            {
                if (PieceManager.Instance == null) return false;
                if (PieceManager.Instance.FoundPieces < requiredPieceCount) return false;
            }

            return true;
        }
    }
}
