namespace FpsHorrorKit
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class DialogueTrigger : MonoBehaviour
    {
        [Tooltip("Gösterilecek diyalog verisi")]
        public DialogueData dialogueData;

        [Header("Dialog Bitince Sahne Yükle (isteğe bağlı)")]
        public bool loadSceneAfter = false;
        public string sceneToLoad;

        private bool queued = false;

        public void Play()
        {
            if (dialogueData == null)
            {
                Debug.LogWarning("[DialogueTrigger] DialogueData atanmamış!");
                return;
            }

            if (DialogueSystem.Instance == null) return;

            if (!DialogueSystem.Instance.isDialogueFinished)
            {
                if (!queued)
                {
                    queued = true;
                    DialogueSystem.Instance.onDialogueEnd += PlayQueued;
                }
                return;
            }

            StartDialogue();
        }

        private void PlayQueued()
        {
            queued = false;
            DialogueSystem.Instance.onDialogueEnd -= PlayQueued;
            StartDialogue();
        }

        private void StartDialogue()
        {
            System.Action onEnd = loadSceneAfter && !string.IsNullOrEmpty(sceneToLoad)
                ? () =>
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    SceneManager.LoadScene(sceneToLoad);
                }
                : null;

            DialogueSystem.Instance.StartDialogue(0, new DialogueData[] { dialogueData }, onEnd);
        }
    }
}
