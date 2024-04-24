namespace Jump.Entities
{
    public interface IInteractable
    {
        public string GetInteractMessage();
        public void OnInteract();
    }
}