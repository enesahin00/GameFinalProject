namespace FpsHorrorKit
{
    using UnityEngine;

    public class PieceTrigger : MonoBehaviour, IInteractable
    {
        [SerializeField] private string interactText = "İncele [E]";
        private bool hasTriggered = false;

        public void Interact()
        {
            if (hasTriggered) return;
            hasTriggered = true;
            PieceManager.Instance.RegisterPiece();
        }

        public void Highlight() => PlayerInteract.Instance.ChangeInteractText(interactText);
        public void HoldInteract() { }
        public void UnHighlight() { }
    }
}
