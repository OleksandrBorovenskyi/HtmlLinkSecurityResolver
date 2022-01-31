using System;
using System.IO;
using System.Linq;
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


            while (true)
            {
                CreateDestinationDirectory();

                // Take all HTML files of source directory
                var htmlFiles = GetHtmlFilesFromDirectory(Configuration.WatchedDirtectoryPath);
                if (!htmlFiles?.Any() ?? true)
                {
                    Console.WriteLine("No files to process.");
                    continue;
                }

                //Parallel.ForEach(htmlFiles, htmlFile =>
                //{
                //    HtmlSecurityService.ProcessHtmlFile(htmlFile);
                //});

                var tasks = htmlFiles.Select(file => HtmlSecurityService.ProcessHtmlFile(file)).ToArray();
                await Task.WhenAll(tasks);

                foreach (var task in tasks)
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine(task.Exception);
                    }
                    else if (task.IsCompleted)
                    {
                        Console.WriteLine($"Task with ID {task.Id} has finished succesfully.");
                    }
                }

                await Task.Delay(Configuration.WatchDirectoryIntervalInMiliseconds);
            }
        }

        private static void CreateDestinationDirectory()
        {
            if (!Directory.Exists(Configuration.DestinationDirectory))
            {
                Directory.CreateDirectory(Configuration.DestinationDirectory);
            }
        }

        private static string[] GetHtmlFilesFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("Watched directory does not exist.");
                return Array.Empty<string>();
            }

            return Directory.GetFiles(directoryPath, "*.html", SearchOption.TopDirectoryOnly);
        }
    }
}
