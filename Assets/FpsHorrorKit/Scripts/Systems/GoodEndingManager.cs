namespace FpsHorrorKit
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;

    public class GoodEndingManager : MonoBehaviour
    {
        [Header("UI")]
        public Image fadePanel;

        [Header("Diyalog")]
        public DialogueData hospitalDialogue;

        [Header("Audio")]
        public AudioSource ambienceSource;
        public AudioClip ambienceClip;

        [Header("Ayarlar")]
        public float fadeInDuration = 3f;
        public string mainMenuScene = "MainMenu";
        public float endDelay = 4f;

        private void Start()
        {
            StartCoroutine(PlayEnding());
        }

        private IEnumerator PlayEnding()
        {
            if (fadePanel != null)
            {
                fadePanel.color = Color.black;
            }

            if (ambienceSource != null && ambienceClip != null)
            {
                ambienceSource.clip = ambienceClip;
                ambienceSource.loop = true;
                ambienceSource.volume = 0f;
                ambienceSource.Play();
                StartCoroutine(FadeAudio(ambienceSource, 0.4f, fadeInDuration * 2f));
            }

            yield return StartCoroutine(FadePanel(1f, 0f, fadeInDuration));

            yield return new WaitForSeconds(1f);

            if (hospitalDialogue != null && DialogueSystem.Instance != null)
            {
                DialogueSystem.Instance.StartDialogue(0, new DialogueData[] { hospitalDialogue }, OnDialogueEnd);
            }
        }

        private void OnDialogueEnd()
        {
            StartCoroutine(EndSequence());
        }

        private IEnumerator EndSequence()
        {
            yield return new WaitForSeconds(endDelay);
            yield return StartCoroutine(FadePanel(0f, 1f, 2f));
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(mainMenuScene);
        }

        private IEnumerator FadePanel(float from, float to, float duration)
        {
            if (fadePanel == null) yield break;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(from, to, elapsed / duration);
                fadePanel.color = new Color(0f, 0f, 0f, alpha);
                yield return null;
            }
            fadePanel.color = new Color(0f, 0f, 0f, to);
        }

        private IEnumerator FadeAudio(AudioSource source, float targetVolume, float duration)
        {
            float start = source.volume;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(start, targetVolume, elapsed / duration);
                yield return null;
            }
            source.volume = targetVolume;
        }
    }
}
