using System;
using System.IO;
using UnityEngine;

namespace GreatArcStudios
{
    [System.Serializable]
    public class SaveSettings
    {
        public string fileName = "GameSettings.json";

        [Header("Kaydedilecek Ayarlar")]
        public int targetFPS = 60;
        public float footstepsVolume = 1f;
        public float environmentVolume = 1f;
        public int curQualityLevel = 3;
        public int vsyncINI = 1;
        public bool fullscreenBool = true;
        public int resHeight;
        public int resWidth;

        public static SaveSettings CreateJSONOBJ(string jsonString)
        {
            return JsonUtility.FromJson<SaveSettings>(jsonString);
        }

        public void LoadGameSettings(string readString)
        {
            try
            {
                SaveSettings read = CreateJSONOBJ(readString);
                
                // Okunan ayarları Unity motoruna doğrudan uygula
                QualitySettings.SetQualityLevel(read.curQualityLevel);
                QualitySettings.vSyncCount = read.vsyncINI;
                Application.targetFrameRate = read.targetFPS;
                
                if (read.resWidth > 0 && read.resHeight > 0)
                {
                    Screen.SetResolution(read.resWidth, read.resHeight, read.fullscreenBool);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Ayarlar yüklenirken bir sorun oluştu veya dosya bozuk: " + e.Message);
            }
        }

        public void SaveGameSettings()
        {
            try
            {
                // Unity motorundaki güncel ayarları çek
                curQualityLevel = QualitySettings.GetQualityLevel();
                vsyncINI = QualitySettings.vSyncCount;
                resHeight = Screen.currentResolution.height;
                resWidth = Screen.currentResolution.width;
                fullscreenBool = Screen.fullScreen;
                targetFPS = Application.targetFrameRate;

                // Verileri JSON formatına çevir ve diske yaz
                string jsonString = JsonUtility.ToJson(this, true);
                File.WriteAllText(Application.persistentDataPath + "/" + fileName, jsonString);
                
                Debug.Log("Ayarlar başarıyla kaydedildi: " + Application.persistentDataPath + "/" + fileName);
            }
            catch (Exception e)
            {
                Debug.LogError("Ayarlar kaydedilemedi: " + e.Message);
            }
        }
    }
}