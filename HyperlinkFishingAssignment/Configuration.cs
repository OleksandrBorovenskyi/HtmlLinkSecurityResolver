using System;
using System.Text.RegularExpressions;

namespace HyperlinkFishingAssignment
{
    public static class Configuration
    {
        public static int WatchDirectoryIntervalInMiliseconds => 10000;

        public static string WatchedDirtectoryPath => AppDomain.CurrentDomain.BaseDirectory + "Html Samples";

        public static string DestinationDirectory => AppDomain.CurrentDomain.BaseDirectory + "Results";

        public static Regex HyperlinkRegex = new Regex(@"(<a href=.+>.+<\/a>)");

    }
}
