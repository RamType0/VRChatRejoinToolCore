namespace VRChatRejoinToolCore
{
    public static class UriGenerator
	{
		public static string GetLaunchInstanceUri(Instance i)
		{
			return "vrchat://launch?id=" + i.Token;
		}

		public static string GetInstanceWebPageUri(Instance i)
		{
			return $"https://vrchat.com/home/launch?worldId={i.WorldId}{(i.Id == "" ? "" : "&instanceId=")}{i.Id}";
		}

		public static string GetUserWebPageUri(string userId)
		{
			return $"https://vrchat.com/home/user/{userId}";
		}
	}
}
