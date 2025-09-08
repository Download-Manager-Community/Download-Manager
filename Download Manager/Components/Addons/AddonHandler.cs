using System.Reflection;
using static DownloadManager.Logging;

namespace DownloadManager.Components.Addons
{
    internal static class AddonHandler
    {
        /// <summary>
        /// All available addons are stored in this list. The list is populated with the default addon and any other
        /// </summary>
        public static List<IAddon> addons = new List<IAddon>();

        /// <summary>
        /// Load addons from the addons directory.
        /// </summary>
        public static void LoadAddons()
        {
            string addonsPath = Path.Combine(DownloadForm.installationPath, "Addons\\");

            // Check if the addons directory exists
            if (Directory.Exists(addonsPath))
            {
                // Get all files in the addons directory
                foreach (string dll in Directory.GetFiles(addonsPath, "*.dll"))
                {
                    // Load the assembly and get all types that implement IAddon
                    Assembly assembly = Assembly.LoadFrom(dll);
                    var types = assembly.GetTypes()
                        .Where(t => typeof(IAddon).IsAssignableFrom(t) && !t.IsInterface);

                    // Create an instance of each addon and add it to the addons list
                    foreach (var type in types)
                    {
                        IAddon addon = (IAddon)Activator.CreateInstance(type);
                        addons.Add(addon);
                        Logging.Log(LogLevel.Debug, $"Loaded addon: {addon.Name}");
                        Logging.Log(LogLevel.Debug, $"Executing: {addon.Name}");
                        addon.Execute();
                    }
                }
            }
            else
            {
                // If the addons directory does not exist, create it
                Directory.CreateDirectory(addonsPath);
                Logging.Log(LogLevel.Warning, "Addons directory does not exist. Creating it.");
            }

            foreach (var addon in addons)
            {
                // Add the addon name to the addons list in the application settings
                DownloadForm._instance.settings.addonsList.Items.Add(addon.Name);
            }
        }
    }
}
