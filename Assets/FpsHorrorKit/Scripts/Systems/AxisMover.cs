namespace FpsHorrorKit
{
    using System.Collections;
    using UnityEngine;

    public class AxisMover : MonoBehaviour
    {
        public enum MoveAxis { X, Y, Z, XZ }

        [Header("Hedef")]
        public Transform target;

        [Header("Hareket Ayarları")]
        public MoveAxis axis = MoveAxis.X;
        public float duration = 2f;
        public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Header("Bitiş")]
        [Tooltip("Hareket bitince başlangıç konumuna dönsün mü?")]
        public bool resetOnFinish = false;

        private Vector3 startPosition;
        private Coroutine moveRoutine;

        private void Awake()
        {
            startPosition = transform.position;
        }

        public void StartMoving()
        {
            if (target == null)
            {
                Debug.LogWarning("AxisMover: Hedef atanmamış!");
                return;
            }

            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(MoveRoutine());
        }

        public void ResetPosition()
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            transform.position = startPosition;
        }

        private IEnumerator MoveRoutine()
        {
            Vector3 from = transform.position;
            Vector3 to = BuildTargetPosition(from);

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = moveCurve.Evaluate(elapsed / duration);
                transform.position = Vector3.Lerp(from, to, t);
                yield return null;
            }

            transform.position = to;

            if (resetOnFinish)
            {
                transform.position = startPosition;
            }

            moveRoutine = null;
        }

        private Vector3 BuildTargetPosition(Vector3 from)
        {
            Vector3 t = target.position;

            return axis switch
            {
                MoveAxis.X  => new Vector3(t.x, from.y, from.z),
                MoveAxis.Y  => new Vector3(from.x, t.y, from.z),
                MoveAxis.Z  => new Vector3(from.x, from.y, t.z),
                MoveAxis.XZ => new Vector3(t.x, from.y, t.z),
                _           => t
            };
        }
    }
}
