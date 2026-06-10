namespace FpsHorrorKit
{
    using System;
    using System.Collections;
    using TMPro;
    using UnityEngine;

    public class PieceManager : MonoBehaviour
    {
        public static PieceManager Instance { get; private set; }
        public static Action<int> OnPieceFound;

        [Header("Settings")]
        [SerializeField] private int totalPieces = 12;

        [Header("Notification UI")]
        [SerializeField] private CanvasGroup notificationGroup;
        [SerializeField] private TextMeshProUGUI notificationText;
        [SerializeField] private string notificationMessage = "İlerleme kaydedildi";

        public int FoundPieces { get; private set; } = 0;

        private Coroutine fadeCoroutine;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (notificationGroup != null) notificationGroup.alpha = 0f;
        }

        public void RegisterPiece()
        {
            if (FoundPieces >= totalPieces) return;
            FoundPieces++;
            OnPieceFound?.Invoke(FoundPieces);
            ShowNotification();
        }

        private void ShowNotification()
        {
            if (notificationText != null)
                notificationText.text = $"{notificationMessage}  ({FoundPieces}/{totalPieces})";

            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeNotification());
        }

        private IEnumerator FadeNotification()
        {
            notificationGroup.alpha = 1f;
            yield return new WaitForSeconds(2f);
            float elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                notificationGroup.alpha = Mathf.Lerp(1f, 0f, elapsed);
                yield return null;
            }
            notificationGroup.alpha = 0f;
        }
    }
}
