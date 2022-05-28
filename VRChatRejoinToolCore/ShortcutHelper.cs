using IWshRuntimeLibrary;
using System.Reflection;
using System.Runtime.InteropServices;

namespace VRChatRejoinToolCore
{
    internal static class ShortcutHelper
    {
        static IWshShell_Class Shell { get; } = new();
        static string IconLocation { get; } = Assembly.GetExecutingAssembly().Location + ",0";
        public static void CreateShortcut(string filePath,string targetPath,string? arguments = null)
        {
            IWshShortcut? shortCut = null;
            try
            {
                shortCut = (IWshShortcut)Shell.CreateShortcut(filePath);
                shortCut.TargetPath = targetPath;
                shortCut.Arguments = arguments;
                shortCut.IconLocation = IconLocation;
                shortCut.Save();
            }
            finally
            {
                if(shortCut is not null)
                {
                    Marshal.FinalReleaseComObject(shortCut);
                }
            }
        }
    }
}
