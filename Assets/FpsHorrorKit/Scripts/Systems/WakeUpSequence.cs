using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace FpsHorrorKit
{
    public class WakeUpSequence : MonoBehaviour
    {
        [Header("Zamanlama Ayarları")]
        public float fadeDuration = 3f;

        [Header("UI Referansları")]
        // public yaptığımız için artık Unity'de Inspector panelinde görünecek
        public Image blackPanel; 

        void Start()
        {
            // Eğer paneli sürüklemeyi unutursan oyunun çökmemesi için küçük bir uyarı
            if (blackPanel == null)
            {
                Debug.LogError("WakeUpSequence: Lütfen Inspector üzerinden Black Panel'i atayın!");
                return;
            }
                
            StartCoroutine(WakeUp());
        }

        IEnumerator WakeUp()
        {
            // Ekranı tam siyah yap ve 1 saniye bekle
            blackPanel.color = new Color(0, 0, 0, 1f);
            yield return new WaitForSeconds(1f);

            // Belirlenen süre boyunca siyahlığı yavaşça saydama (0 alpha) çevir
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                blackPanel.color = new Color(0, 0, 0, alpha);
                yield return null;
            }

            // İşlem bitince tam saydam olduğundan emin ol
            blackPanel.color = new Color(0, 0, 0, 0f);
        }
    }
}