namespace FpsHorrorKit
{
    using UnityEngine;

    public class ITODialogue : MonoBehaviour, IInteractable
    {
        [Header("Interact Text")]
        [SerializeField] private string interactText = "Oku [E]";

        [Header("Dialogue")]
        [SerializeField] private DialogueData[] dialogueDatas;
        [SerializeField] private int dialogueIndex = 0;

        [Header("One Shot")]
        [SerializeField] private bool playOnce = true;
        private bool hasPlayed = false;

        public void Interact()
        {
            if (playOnce && hasPlayed) return;

            if (DialogueSystem.Instance.StartDialogue(dialogueIndex, dialogueDatas))
            {
                hasPlayed = true;
            }
        }

        public void Highlight()
        {
            PlayerInteract.Instance.ChangeInteractText(interactText);
        }

        public void HoldInteract() { }
        public void UnHighlight() { }
    }
}
