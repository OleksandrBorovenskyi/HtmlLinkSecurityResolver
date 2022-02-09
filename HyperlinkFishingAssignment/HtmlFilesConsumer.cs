using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HyperlinkFishingAssignment
{
    class HtmlFilesConsumer
    {
        public static ConcurrentDictionary<string, FileInfo> Buffer { get; } = new ConcurrentDictionary<string, FileInfo>();

        public static async Task Run()
        {
            while (true)
            {
                if (Buffer.Any())
                {
                    var filesToProcess = Buffer.Values;
                    Parallel.ForEach(filesToProcess, file =>
                    {
                        HtmlSecurityService.ProcessHtmlFileWithStreams(file.FullName);
                    });

                    Buffer.Clear();
                }

                await Task.Delay(3000);
            }
        }
    }
}
