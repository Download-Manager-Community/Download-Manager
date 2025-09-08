using ReleaseToolBuild.Components;
using System.Diagnostics;
using System.Reflection;

namespace ReleaseToolBuild
{
    class Program
    {
        private static string downloadManagerPath = "F:/Download Manager/Download Manager/";
        private static string releasePath = downloadManagerPath + "bin/Release/net8.0-windows10.0.22000.0/";

        private static Assembly thisAssembly = Assembly.GetExecutingAssembly();
        private static Assembly? downloadManagerAssembly = null;

        private static int lastExitCode = -1;

        static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string[] args)
        {
            Logging.WriteColoredLine("Download Manager Build Release Tool", ConsoleColor.Blue);
            Console.WriteLine("==================================");
            if (!CheckBuild())
                await BuildRelease();

            try
            {
                downloadManagerAssembly = Assembly.LoadFile(downloadManagerPath);
            }
            catch (FileNotFoundException) { }

            Console.WriteLine("Tool Version    : "); Logging.WriteColoredLine($"{Assembly.GetExecutingAssembly().GetName().Version}", ConsoleColor.Yellow);
            Console.WriteLine("Current Version : "); Logging.WriteColoredLine($"{downloadManagerAssembly.GetName().Version.ToString() ?? "Unknown"}", ConsoleColor.Yellow);

            if (lastExitCode != 0)
            {
                Logging.WriteColoredLine("Failed to build the release.", ConsoleColor.Red);
            }

            Logging.Pause();
        }

        private async static Task BuildRelease()
        {
            // Build installer with --self-contained true
            Environment.CurrentDirectory = downloadManagerPath;
            await StartProcess("dotnet", "clean \"../Download Manager.sln\" --nologo");

            if (lastExitCode != 0)
            {
                Logging.WriteColoredLine("Project clean failed!", ConsoleColor.Red);
                return;
            }

            await StartProcess("dotnet", "build -c Release --nologo");
            if (!CheckBuild())
            {
                Logging.WriteColoredLine("Build failed!", ConsoleColor.Red);
                return;
            }
        }

        private static bool CheckBuild()
        {
            // Check if the build was successful
            if (!File.Exists(releasePath + "DownloadManager.dll"))
            {
                Logging.WriteColoredLine("Could not find a build!", ConsoleColor.Red);
                return false;
            }
            else
            {
                Logging.WriteColoredLine("Build located!", ConsoleColor.Green);
                return true;
            }
        }

        private async static Task StartProcess(string fileName, string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process process = new Process { StartInfo = startInfo };

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.Error.WriteLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
            lastExitCode = process.ExitCode;
        }
    }
}