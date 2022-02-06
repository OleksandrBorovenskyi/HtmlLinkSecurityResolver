using System;
using System.IO;
using System.Threading.Tasks;

namespace HyperlinkFishingAssignment
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // For each HTML file we need:
            // open and read (better as Stream)
            // if we find Hyperlink (<a href="Muku"> Kuku </a>)
            // check whether link equals to text
            // yes - ok, proceed
            // no - replace link with text

            CreateDestinationDirectory();
            SetupDirectoryWatcher(true, Configuration.WatchedDirtectoryPath);

            while (true)
            {
            }
        }

        private static void CreateDestinationDirectory()
        {
            if (!Directory.Exists(Configuration.DestinationDirectory))
            {
                Directory.CreateDirectory(Configuration.DestinationDirectory);
            }
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

            Console.WriteLine(e.Name);

            HtmlSecurityService.ProcessHtmlFileWithStreams(e.FullPath);
        }
    }
}
