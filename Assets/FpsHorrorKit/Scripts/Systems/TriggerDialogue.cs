namespace FpsHorrorKit
{
    using UnityEngine;

    public class TriggerDialogue : MonoBehaviour
    {
        [SerializeField] private DialogueData dialogueData;
        [SerializeField] private bool playOnce = true;
        private bool hasPlayed = false;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (playOnce && hasPlayed) return;

            if (DialogueSystem.Instance.StartDialogue(0, new DialogueData[] { dialogueData }))
            {
                hasPlayed = true;
            }
        }
    }
}
