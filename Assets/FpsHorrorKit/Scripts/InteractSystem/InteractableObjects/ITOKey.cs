namespace FpsHorrorKit
{
    using UnityEngine;

    public class ITOKey : MonoBehaviour, IInteractable
    {
        public string keyID;
        [SerializeField] private string interactText = "Take key [E]";

        public void Interact()
        {
            KeyManager.Instance.AddKey(keyID);
            Destroy(gameObject);
        }

        public void Highlight()
        {
            PlayerInteract.Instance.ChangeInteractText(interactText);
        }

        public void HoldInteract() { }
        public void UnHighlight() { }
    }
}