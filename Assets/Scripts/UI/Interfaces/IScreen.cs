namespace UI.Interfaces
{
    /// <summary>
    /// Interface For Views that need a header name such as the Screens and Modals
    /// </summary>
    public interface IScreen
    {
        public string HeaderName { get; }
    }
}