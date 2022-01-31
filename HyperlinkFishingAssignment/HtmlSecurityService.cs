using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HyperlinkFishingAssignment
{
    public static class HtmlSecurityService
    {
        public static async Task ProcessHtmlFile(string htmlFile)
        {
            // open and read (better as Stream)
            var content = File.ReadAllText(htmlFile);

            var processedContent = Configuration.HyperlinkRegex.Replace(content, CheckAndReplaceHyperlink);

            // Write content to new file to target 
            var destinationFilePath = Path.Combine(Configuration.DestinationDirectory, new FileInfo(htmlFile).Name);
            await File.WriteAllTextAsync(destinationFilePath, processedContent);

            // File.Delete(htmlFile);
        }

        private static string CheckAndReplaceHyperlink(Match regexMatch)
        {
            var href = "href=";
            var linkStartIndex = regexMatch.Value.IndexOf(href) + href.Length + 1;
            var linkEndIndex = regexMatch.Value.IndexOf("\"", linkStartIndex);
            var link = regexMatch.Value.Substring(linkStartIndex, linkEndIndex-linkStartIndex);

            var textStartIndex = regexMatch.Value.IndexOf(">") + 1;
            var textEndIndex = regexMatch.Value.IndexOf("</a>");
            var text = regexMatch.Value.Substring(textStartIndex, textEndIndex-textStartIndex);

            if(text.Equals(link, StringComparison.InvariantCultureIgnoreCase))
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
