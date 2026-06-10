namespace FpsHorrorKit
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using TMPro;

    public class LoadingScreen : MonoBehaviour
    {
        [Header("UI Elemanları")]
        public Image backgroundImage;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI loadingText;
        public Slider loadingBar;

        [Header("Ayarlar")]
        [Tooltip("Yükleme bitince geçilecek sahne adı")]
        public string nextSceneName = "MainScene";
        [Tooltip("Çok hızlı yüklenirse bile en az bu kadar saniye göster")]
        public float minimumDisplayTime = 3f;
        [Tooltip("Yükleme tamamlanınca otomatik geçsin mi, yoksa oyuncu tuşa bassın mı?")]
        public bool autoTransition = true;
        public string pressToStartText = "Başlamak için herhangi bir tuşa bas";

        [Header("Yükleme Yazısı")]
        public string[] loadingDots = { "Yükleniyor", "Yükleniyor.", "Yükleniyor..", "Yükleniyor..." };
        public float dotInterval = 0.4f;

        private void Start()
        {
            if (titleText != null) titleText.text = "Rezonans";
            if (loadingBar != null) loadingBar.value = 0f;

            StartCoroutine(LoadRoutine());
            StartCoroutine(AnimateDots());
        }

        private IEnumerator LoadRoutine()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneName);
            operation.allowSceneActivation = false;

            float elapsed = 0f;

            while (operation.progress < 0.9f || elapsed < minimumDisplayTime)
            {
                elapsed += Time.deltaTime;
                float loadProgress = Mathf.Clamp01(operation.progress / 0.9f);
                float timeProgress = Mathf.Clamp01(elapsed / minimumDisplayTime);
                float progress = Mathf.Min(loadProgress, timeProgress);
                if (loadingBar != null) loadingBar.value = progress;
                yield return null;
            }

            if (loadingBar != null) loadingBar.value = 1f;

            if (autoTransition)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }
            else
            {
                if (loadingText != null) loadingText.text = pressToStartText;
                yield return new WaitUntil(() => Input.anyKeyDown);
                operation.allowSceneActivation = true;
            }
        }

        private IEnumerator AnimateDots()
        {
            int index = 0;
            while (true)
            {
                if (loadingText != null)
                    loadingText.text = loadingDots[index % loadingDots.Length];
                index++;
                yield return new WaitForSeconds(dotInterval);
            }
        }
    }
}
