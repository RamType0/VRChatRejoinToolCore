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
			var dynShellType = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"))!;
			dynamic shell = Activator.CreateInstance(dynShellType)!;
			var shortcut = shell.CreateShortcut(filepath);

			//shortcut.IconLocation = Application.ExecutablePath + ",0";

			if (!httpLink)
			{
				shortcut.TargetPath = UriGenerator.GetLaunchInstanceUri(i);
			}
			else
			{
				shortcut.TargetPath = UriGenerator.GetInstanceWebPageUri(i);
			}

			shortcut.Save();
			Marshal.FinalReleaseComObject(shortcut);
			Marshal.FinalReleaseComObject(shell);
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
