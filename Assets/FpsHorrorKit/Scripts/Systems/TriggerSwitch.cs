namespace FpsHorrorKit
{
    using UnityEngine;

    public class TriggerSwitch : MonoBehaviour, IInteractable
    {
        [Header("Bağlı Trigger")]
        [Tooltip("Hangi GeneralTrigger'ı ateşlesin?")]
        public GeneralTrigger target;

        [Header("Metin Ayarları")]
        public string interactText = "Kullan [E]";

        public void Interact()
        {
            if (target != null)
                target.FireFromSwitch();
        }

        public void Highlight()
        {
            PlayerInteract.Instance.ChangeInteractText(interactText);
        }

        public void HoldInteract() { }
        public void UnHighlight() { }
    }
}
