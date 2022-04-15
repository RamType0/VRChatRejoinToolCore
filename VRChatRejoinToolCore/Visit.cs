using System;
using System.Collections.Generic;
using System.IO;

namespace VRChatRejoinToolCore
{
    internal record Visit(Instance Instance ,DateTime TimeStamp)
    {
        public static void LoadVisits(List<Visit> visits)
        {
            var logFileDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"../LocalLow/VRChat/VRChat");
            foreach (var logFilePath in Directory.GetFiles(logFileDir, "output_log_*.txt"))
            {
                LogParser.Add(visits, File.ReadAllBytes(logFilePath));
            }
        } 
    }
}
