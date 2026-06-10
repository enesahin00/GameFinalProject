namespace FpsHorrorKit
{
    using System;
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class MindQuestionSystem : MonoBehaviour
    {
        public static MindQuestionSystem Instance { get; private set; }
        public bool IsQuestionActive { get; private set; }

        [Serializable]
        public class Answer
        {
            [TextArea(1, 2)] public string text;
            public int sanityChange;
            [TextArea(2, 4)] public string responseText;
        }

        [Serializable]
        public class Question
        {
            [TextArea(2, 4)] public string questionText;
            [Tooltip("Kaçıncı parça bulununca bu soru tetiklensin")]
            public int triggerAtPiece;
            public Answer[] answers;
        }

        [Header("Sorular (Inspector'dan Düzenle)")]
        [SerializeField] private Question[] questions;

        [Header("Soru UI Referansları")]
        [SerializeField] private GameObject questionPanel;
        [SerializeField] private TextMeshProUGUI questionText;
        [SerializeField] private TextMeshProUGUI[] answerTexts;

        [Header("Cevap Sonrası UI")]
        [SerializeField] private GameObject responsePanel;
        [SerializeField] private TextMeshProUGUI responseText;
        [SerializeField] private float responseDisplayTime = 3f;

        private Answer[] currentAnswers;
        private int nextQuestionIndex = 0;
        private FpsController fpsController;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void Start()
        {
            fpsController = FindAnyObjectByType<FpsController>();
            if (questionPanel != null) questionPanel.SetActive(false);
            if (responsePanel != null) responsePanel.SetActive(false);
            PieceManager.OnPieceFound += OnPieceFound;
        }

        private void OnDestroy()
        {
            PieceManager.OnPieceFound -= OnPieceFound;
        }

        private void Update()
        {
            if (!IsQuestionActive || currentAnswers == null) return;

            if (Input.GetKeyDown(KeyCode.Alpha1) && currentAnswers.Length > 0) SelectAnswer(currentAnswers[0]);
            if (Input.GetKeyDown(KeyCode.Alpha2) && currentAnswers.Length > 1) SelectAnswer(currentAnswers[1]);
            if (Input.GetKeyDown(KeyCode.Alpha3) && currentAnswers.Length > 2) SelectAnswer(currentAnswers[2]);
            if (Input.GetKeyDown(KeyCode.Alpha4) && currentAnswers.Length > 3) SelectAnswer(currentAnswers[3]);
        }

        private void OnPieceFound(int count)
        {
            if (nextQuestionIndex >= questions.Length) return;
            if (count == questions[nextQuestionIndex].triggerAtPiece)
            {
                ShowQuestion(questions[nextQuestionIndex]);
                nextQuestionIndex++;
            }
        }

        private void ShowQuestion(Question q)
        {
            currentAnswers = q.answers;
            questionPanel.SetActive(true);
            questionText.text = q.questionText;

            for (int i = 0; i < answerTexts.Length; i++)
            {
                if (i < q.answers.Length)
                    answerTexts[i].text = $"[{i + 1}]  {q.answers[i].text}";
                else
                    answerTexts[i].text = "";
            }

            LockPlayer();
        }

        private void SelectAnswer(Answer answer)
        {
            if (!IsQuestionActive) return;
            currentAnswers = null;
            SanityManager.Instance.ChangeSanity(answer.sanityChange);
            questionPanel.SetActive(false);
            UnlockPlayer();
            StartCoroutine(ShowResponse(answer.responseText));
        }

        private IEnumerator ShowResponse(string response)
        {
            responsePanel.SetActive(true);
            responseText.text = response;
            yield return new WaitForSecondsRealtime(responseDisplayTime);
            responsePanel.SetActive(false);
        }

        private void LockPlayer()
        {
            IsQuestionActive = true;
            if (PlayerInteract.Instance != null) PlayerInteract.Instance.sendRaycast = false;
            if (fpsController != null) fpsController.isInteracting = true;
            if (FpsAssetsInputs.Instance != null) FpsAssetsInputs.Instance.cursorLocked = false;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }

        private void UnlockPlayer()
        {
            IsQuestionActive = false;
            if (PlayerInteract.Instance != null) PlayerInteract.Instance.sendRaycast = true;
            if (fpsController != null) fpsController.isInteracting = false;
            if (FpsAssetsInputs.Instance != null) FpsAssetsInputs.Instance.cursorLocked = true;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
