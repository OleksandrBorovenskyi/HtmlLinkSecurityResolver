using System;
using System.IO;
using System.Threading.Tasks;

namespace HyperlinkFishingAssignment
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Directory.CreateDirectory(Configuration.WatchedDirtectoryPath);
            Directory.CreateDirectory(Configuration.DestinationDirectory);

            SetupDirectoryWatcher(true, Configuration.WatchedDirtectoryPath);
            HtmlFilesConsumer.Run();

            Console.ReadLine();
        }

        private static FileSystemWatcher SetupDirectoryWatcher(bool enabledAfterCreation, string directoryPath)
        {
            var watcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.html",
                Path = directoryPath,
                EnableRaisingEvents = enabledAfterCreation
            };

            watcher.Changed += WatchedDirectoryChangedHandler;

            return watcher;
        }

        private static void WatchedDirectoryChangedHandler(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(e.FullPath)) return;

            var currentFileInfo = new FileInfo(e.FullPath);
            var currentFileKey = GetFileKey(currentFileInfo);

            if (HtmlFilesConsumer.Buffer.TryGetValue(currentFileKey, out var buffer)) return;

            HtmlFilesConsumer.Buffer.TryAdd(currentFileKey, currentFileInfo);
        }

        private static string GetFileKey(FileInfo fileInfo)
        {
            return fileInfo.FullName + fileInfo.CreationTime.ToString("F");
        }
    }
}
