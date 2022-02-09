using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HyperlinkFishingAssignment
{
    public static class HtmlSecurityService
    {
        public static async ValueTask ProcessHtmlFileWithStreams(string htmlFile)
        {
            if (!File.Exists(htmlFile)) return;

            var destinationFilePath = Path.Combine(Configuration.DestinationDirectory, new FileInfo(htmlFile).Name);

            using (var sourceFileStream = new FileStream(htmlFile, FileMode.Open, FileAccess.Read, FileShare.None))
            using (var destinationFileStream = new FileStream(destinationFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            using (var streamReader = new StreamReader(sourceFileStream, Encoding.UTF8))
            {
                string currentLine;
                while ((currentLine = await streamReader.ReadLineAsync()) != null)
                {
                    var processedLine = Configuration.HyperlinkRegex.Replace(currentLine, CheckAndReplaceHyperlink);
                    var processedLineBytes = Encoding.UTF8.GetBytes(processedLine);

                    destinationFileStream.Write(processedLineBytes, 0, processedLineBytes.Length);
                }
            }

            File.Delete(htmlFile);
        }

        public static async ValueTask ProcessHtmlFile(string htmlFile)
        {
            if (!File.Exists(htmlFile)) return;

            var content = await File.ReadAllTextAsync(htmlFile);

            var processedContent = Configuration.HyperlinkRegex.Replace(content, CheckAndReplaceHyperlink);

            var destinationFilePath = Path.Combine(Configuration.DestinationDirectory, new FileInfo(htmlFile).Name);
            await File.WriteAllTextAsync(destinationFilePath, processedContent);

            File.Delete(htmlFile);
        }

        private static string CheckAndReplaceHyperlink(Match regexMatch)
        {
            var href = "href=";
            var linkStartIndex = regexMatch.Value.IndexOf(href) + href.Length + 1;
            var linkEndIndex = regexMatch.Value.IndexOf("\"", linkStartIndex);
            var link = regexMatch.Value.Substring(linkStartIndex, linkEndIndex - linkStartIndex);

            var textStartIndex = regexMatch.Value.IndexOf(">") + 1;
            var textEndIndex = regexMatch.Value.IndexOf("</a>");
            var text = regexMatch.Value.Substring(textStartIndex, textEndIndex - textStartIndex);

            if (text.Equals(link, StringComparison.InvariantCultureIgnoreCase))
            {
                return regexMatch.Value;
            }
            else
            {
                var replacementResult = regexMatch.Value.Replace(text, link);
                return replacementResult;
            }
        }
    }
}
