namespace Player
{
    public interface IPlayerView
    {
        public void ShowInteraction(bool status);
        public void SetHintText(string text);
        public void SetInteractionSuppressed(bool suppressed);
    }
}