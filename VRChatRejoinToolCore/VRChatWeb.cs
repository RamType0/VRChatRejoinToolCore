using System.Diagnostics;

namespace VRChatRejoinToolCore
{
    internal static class VRChatWeb
    {
        public static void OpenUserDetail(string userId)
        {
            OpenInWebBrowser(UriGenerator.GetUserWebPageUri(userId));
        }

        private static void OpenInWebBrowser(string url)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = url,
                UseShellExecute = true,
            });
        }

        public static void OpenInstanceDetail(Instance instance)
        {
            OpenInWebBrowser(UriGenerator.GetInstanceWebPageUri(instance));
        }
    }
}
