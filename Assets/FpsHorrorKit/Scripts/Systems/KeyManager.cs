// KeyManager.cs — GameManager objesine ekle
using UnityEngine;
using System.Collections.Generic;

namespace FpsHorrorKit
{
    public class KeyManager : MonoBehaviour
    {
        public static KeyManager Instance;

        List<string> collectedKeys = new List<string>();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public static System.Action<string> OnKeyCollected;

        public void AddKey(string keyID)
        {
            collectedKeys.Add(keyID);
            OnKeyCollected?.Invoke(keyID);
        }

        public bool HasKey(string keyID) => collectedKeys.Contains(keyID);
    }
}