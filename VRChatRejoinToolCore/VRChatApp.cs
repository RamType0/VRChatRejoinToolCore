using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VRChatRejoinToolCore
{
    internal static class VRChatApp
    {
		public static void SaveInstanceToShortcut(Instance i, string filepath, bool httpLink = false)
		{
			//shortcut.IconLocation = Application.ExecutablePath + ",0";
			var targetPath = httpLink ? UriGenerator.GetInstanceWebPageUri(i) : UriGenerator.GetLaunchInstanceUri(i);
			ShortcutHelper.CreateShortcut(filepath, targetPath);
		}

		public static void Launch(Instance i, bool killVRC)
		{
			if (killVRC)
				foreach (var p in Process.GetProcessesByName("vrchat"))
					p.Kill();

			Process.Start(new ProcessStartInfo
			{
				FileName = UriGenerator.GetLaunchInstanceUri(i),
				UseShellExecute = true,
			});
		}

		public static int InviteMe(Instance i, string vrcInviteMePath)
		{
			if (!File.Exists(vrcInviteMePath))
			{
				return -1;
			}

			var proc = Process.Start(new ProcessStartInfo
			{
				FileName = vrcInviteMePath,
				Arguments = UriGenerator.GetLaunchInstanceUri(i),
				UseShellExecute = true,
			})!;

			// ReSharper disable once PossibleNullReferenceException
			// アトミックな処理ではないのでそんなに頑張る必要はないとの結論に達した。
			proc.WaitForExit();

			return proc.ExitCode;
		}
	}
}
