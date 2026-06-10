using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Geçiş Ayarları")]
    public GameObject calendarPanel; // Siyah takvim ekranı
    public float waitTime = 3f;      // Takvimin ekranda kalma süresi

    [Header("Paneller")]
    public GameObject mainPanel;
    public GameObject vidPanel;
    public GameObject audioPanel;
    
    [Header("Animatörler")]
    public Animator audioPanelAnimator;
    public Animator vidPanelAnimator;
    public Animator quitPanelAnimator;

    [Header("Ses Sistemi (Audio Mixer)")]
    public AudioMixer mainMixer;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (calendarPanel != null) calendarPanel.SetActive(false);
        mainPanel.SetActive(true);
        vidPanel.SetActive(false);
        audioPanel.SetActive(false);
    }

    // --- TAKVİM VE OYUNA GİRİŞ ---
    public void StartGame()
    {
        StartCoroutine(ShowCalendarAndLoadGame());
    }

    private IEnumerator ShowCalendarAndLoadGame()
    {
        if (calendarPanel != null) calendarPanel.SetActive(true);
        yield return new WaitForSeconds(waitTime); // Takvim ekranında bekle
        SceneManager.LoadScene(2); // 2 numaralı sahneyi (Oyunu) yükle
    }

    // --- ÇIKIŞ KONTROLLERİ ---
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
        Debug.Log("Oyundan Çıkılıyor... (Build'de kapanacak)");
        Application.Quit();
    }

    // --- MENÜ GEÇİŞLERİ VE ANİMASYONLAR ---
    public void Audio()
    {
        mainPanel.SetActive(false);
        vidPanel.SetActive(false);
        audioPanel.SetActive(true);
        audioPanelAnimator.enabled = true;
        audioPanelAnimator.Play("Audio Panel In");
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
    }

    public void apply() { StartCoroutine(TransitionPanel(vidPanelAnimator, "Video Panel Out")); }
    public void cancelVideo() { StartCoroutine(TransitionPanel(vidPanelAnimator, "Video Panel Out")); }

    private IEnumerator TransitionPanel(Animator animator, string animationName)
    {
        animator.Play(animationName);
        yield return new WaitForSecondsRealtime(0.2f); // Animasyon süresini bekle
        mainPanel.SetActive(true);
        vidPanel.SetActive(false);
        audioPanel.SetActive(false);
    }

    // --- VİDEO VE GRAFİK KONTROLLERİ ---
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVSync(bool isVSyncOn)
    {
        QualitySettings.vSyncCount = isVSyncOn ? 1 : 0;
    }

    public void SetFPSLimit(int dropdownIndex)
    {
        switch (dropdownIndex)
        {
            case 0: Application.targetFrameRate = 30; break;
            case 1: Application.targetFrameRate = 60; break;
            case 2: Application.targetFrameRate = 120; break;
            case 3: Application.targetFrameRate = -1; break; 
        }
    }

    // --- BİZİM MİKSER KONTROLLERİ ---
    public void SetMasterVolume(float sliderValue)
    {
        if (mainMixer != null)
        {
            float volume = sliderValue <= 0.001f ? -80f : Mathf.Log10(sliderValue) * 20f;
            mainMixer.SetFloat("MasterVol", volume); 
        }
    }
    
    public void SetPlayerVolume(float sliderValue)
    {
        if (mainMixer != null)
        {
            float volume = sliderValue <= 0.001f ? -80f : Mathf.Log10(sliderValue) * 20f;
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
}