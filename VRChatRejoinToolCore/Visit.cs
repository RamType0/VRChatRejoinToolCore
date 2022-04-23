using System;
using System.Collections.Generic;
using System.IO;

namespace VRChatRejoinToolCore
{
    internal class Visit
    {
        public Visit(Instance instance,DateTime timeStamp)
        {
            Instance = instance;
            TimeStamp = timeStamp;
        }

        public Instance Instance { get; }
        public DateTime TimeStamp { get; }
        public static void LoadVisits(List<Visit> visits)
        {
            var logFileDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"../LocalLow/VRChat/VRChat");
            foreach (var logFilePath in Directory.GetFiles(logFileDir, "output_log_*.txt"))
            {
                LogParser.GetVisits(visits, File.ReadAllBytes(logFilePath));
            }
        } 
    }
}
