namespace GreatArcStudios
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.SceneManagement;
    using UnityEngine.Audio;
    using System.Collections;

    public class PauseManager : MonoBehaviour
    {
        [Header("Paneller")]
        public GameObject mainPanel;
        public GameObject vidPanel;
        public GameObject audioPanel;
        public GameObject TitleTexts;
        public GameObject mask;
        
        [Header("Animatörler")]
        public Animator audioPanelAnimator;
        public Animator vidPanelAnimator;
        public Animator quitPanelAnimator;
        
        [Header("Ayarlar")]
        public Text pauseMenu;
        public string mainMenu;
        public float timeScale = 1f;
        public GameObject[] otherUIElements;

        [Header("Ses Sistemi (Audio Mixer)")]
        public AudioMixer mainMixer;

        void Start()
        {
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            
        
            if (mask != null) mask.SetActive(false);
            if (TitleTexts != null) TitleTexts.SetActive(true); 

            // --- İŞTE EKLENEN FARE GİZLEME KODU ---
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            // --- DİKTATÖR FARE KORUMASI ---
            // Eğer menülerden herhangi biri açıksa, karakter kontrolcüsü ne derse desin fareyi serbest bırak!
            if (mainPanel.activeSelf || vidPanel.activeSelf || audioPanel.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // --- MEVCUT ESC KONTROLÜN ---
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!mainPanel.activeSelf && !vidPanel.activeSelf && !audioPanel.activeSelf)
                {
                    PauseGame();
                }
                else if (mainPanel.activeSelf)
                {
                    Resume();
                }
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            // Oyuncu pencere dışına tıklayıp sonra oyuna geri tıkladığında (hasFocus = true) çalışır
            if (hasFocus)
            {
                // Eğer Ana, Video veya Ses panellerinden herhangi biri o an ekrandaysa (Oyun duraklatılmışsa)
                if (mainPanel.activeSelf || vidPanel.activeSelf || audioPanel.activeSelf)
                {
                    // Fareyi FPS kontrolcüsünün elinden zorla geri al, menüde kullanıma aç!
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                // (Eğer menüler kapalıysa hiçbir şey yapma, karakter kontrolcüsü işini yapmaya devam etsin)
            }
        }

        public void PauseGame()
        {
            mainPanel.SetActive(true);
            TitleTexts.SetActive(true);
            mask.SetActive(true);
            Time.timeScale = 0f;
            
            // Fareyi serbest bırak ve göster
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            foreach (GameObject ui in otherUIElements)
            {
                if (ui != null) ui.SetActive(false);
            }
        }

        public void Resume()
        {
            Time.timeScale = timeScale;
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            TitleTexts.SetActive(false);
            mask.SetActive(false);
            
            // Fareyi oyuna kilitle ve gizle
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            foreach (GameObject ui in otherUIElements)
            {
                if (ui != null) ui.SetActive(true);
            }
        }

        public void Restart()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void returnToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenu);
        }

        public void quitOptions()
        {
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            quitPanelAnimator.enabled = true;
            quitPanelAnimator.Play("QuitPanelIn");
        }

        public void quitCancel()
        {
            quitPanelAnimator.Play("QuitPanelOut");
        }

        public void quitGame()
        {
            Application.Quit();
        }

        // --- MENÜ GEÇİŞLERİ ---
        
        public void Audio()
        {
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            audioPanel.SetActive(true);
            audioPanelAnimator.enabled = true;
            audioPanelAnimator.Play("Audio Panel In");
            pauseMenu.text = "Audio Menu";
        }

        public void applyAudio() { StartCoroutine(TransitionPanel(audioPanelAnimator, "Audio Panel Out")); }
        public void cancelAudio() { StartCoroutine(TransitionPanel(audioPanelAnimator, "Audio Panel Out")); }

        public void Video()
        {
            mainPanel.SetActive(false);
            vidPanel.SetActive(true);
            audioPanel.SetActive(false);
            vidPanelAnimator.enabled = true;
            vidPanelAnimator.Play("Video Panel In");
            pauseMenu.text = "Video Menu";
        }

        public void apply() { StartCoroutine(TransitionPanel(vidPanelAnimator, "Video Panel Out")); }
        public void cancelVideo() { StartCoroutine(TransitionPanel(vidPanelAnimator, "Video Panel Out")); }

        private IEnumerator TransitionPanel(Animator animator, string animationName)
        {
            animator.Play(animationName);
            yield return new WaitForSecondsRealtime(0.2f); // Animasyon süresi
            mainPanel.SetActive(true);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            pauseMenu.text = "Pause Menu";
        }

        // --- BİZİM MİKSER KONTROLLERİ ---
        // --- VİDEO VE GRAFİK KONTROLLERİ ---

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetVSync(bool isVSyncOn)
        {
            // VSync açıksa 1 (monitör hızına kilitler), kapalıysa 0 yap
            QualitySettings.vSyncCount = isVSyncOn ? 1 : 0;
        }

        public void SetFPSLimit(int dropdownIndex)
        {
            // Dropdown (Açılır Menü) sırasına göre FPS belirliyoruz
            switch (dropdownIndex)
            {
                case 0: Application.targetFrameRate = 30; break;
                case 1: Application.targetFrameRate = 60; break;
                case 2: Application.targetFrameRate = 120; break;
                case 3: Application.targetFrameRate = -1; break; // -1 Unity'de "Sınırsız" demektir
            }
        }
        public void SetMasterVolume(float sliderValue)
        {
            if (mainMixer != null)
            {
                float volume = sliderValue <= 0.001f ? -80f : Mathf.Log10(sliderValue) * 20f;
                // Mikserdeki en tepedeki MasterVol'u hedef alıyoruz
                mainMixer.SetFloat("MasterVol", volume); 
            }
        }
        
        public void SetPlayerVolume(float sliderValue)
        {
            if (mainMixer != null)
            {
                float volume = sliderValue <= 0.001f ? -80f : Mathf.Log10(sliderValue) * 20f;
                // Mikserdeki yeni ismimiz olan PlayerVol'u arıyoruz
                mainMixer.SetFloat("PlayerVol", volume); 
            }
        }

        public void SetEnvironmentVolume(float sliderValue)
        {
            if (mainMixer != null)
            {
                float volume = sliderValue <= 0.001f ? -80f : Mathf.Log10(sliderValue) * 20f;
                mainMixer.SetFloat("EnvironmentVol", volume);
            }
        }
        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f; 
            SceneManager.LoadScene(0); 
        }
    }
}