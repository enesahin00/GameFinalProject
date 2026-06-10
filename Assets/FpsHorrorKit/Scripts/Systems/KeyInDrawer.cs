using UnityEngine;

namespace FpsHorrorKit
{
    public class KeyInDrawer : MonoBehaviour
    {
        public Transform drawer;
        public float openDistance = 0.05f;

        Collider col;
        Vector3 startPos;

        void Start()
        {
            col = GetComponent<Collider>();
            col.enabled = false;
            startPos = drawer.localPosition;
            Debug.Log("Başlangıç pos: " + startPos);
        }

        void Update()
        {
            if (col.enabled) return;
            float dist = Vector3.Distance(drawer.localPosition, startPos);
            Debug.Log("Mesafe: " + dist);
            if (dist >= openDistance)
                col.enabled = true;
        }
    }
}