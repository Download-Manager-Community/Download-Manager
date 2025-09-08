namespace DownloadManager.Components.Addons
{
    public interface IAddon
    {
        string Name { get; }
        void Execute();
    }
}
