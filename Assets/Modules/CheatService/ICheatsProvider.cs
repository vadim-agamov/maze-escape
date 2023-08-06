namespace Modules.CheatService
{
    public interface ICheatsProvider
    {
        void OnGUI();
        string Id { get; }
    }
}