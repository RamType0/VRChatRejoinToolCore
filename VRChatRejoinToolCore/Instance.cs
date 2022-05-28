using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace VRChatRejoinToolCore
{
    public class Instance
	{
		static readonly Regex instanceNameR = new Regex(@"\A[A-Za-z0-9]+\z");
		static readonly Regex nonceR = new Regex(@"\A([0-9A-F]{48}|[0-9A-F]{64}|[a-f0-9]{8}-[a-f0-9]{4}-[0-5][a-f0-9]{3}-[a-b0-9][a-f0-9]{3}-[a-f0-9]{12})\z");
		static readonly Regex userIdR = new Regex(@"\Ausr_[a-f0-9]{8}-[a-f0-9]{4}-4[a-f0-9]{3}-[a-f0-9]{4}-[a-f0-9]{12}\z");
		static readonly Regex worldIdR = new Regex(@"\Awr?ld_[a-f0-9]{8}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{4}-[a-f0-9]{12}\z");
		static readonly Regex regionR = new Regex(@"\A[a-z]{1,3}\z");

		public Permission Permission { get; set; }

		public ServerRegion Region { get; set; }
		public string CustomRegion { get; set; } = "";

		public string? WorldName { get; set; }

		public string? OwnerId { get; set; }

		public string? Nonce { get; set; }

		public string WorldId { get; set; }

		public string InstanceName { get; set; }

		public string Id
		{
			get
			{
				string id = InstanceName;

				switch (Permission)
				{
					case Permission.Unknown:
						return "";

					case Permission.Public:
						break;

					case Permission.FriendsPlus:
						id += "~hidden";
						break;

					case Permission.Friends:
						id += "~friends";
						break;

					case Permission.InvitePlus:
					case Permission.InviteOnly:
						id += "~private";
						break;
				}

				if (Permission != Permission.Public)
					if (OwnerId != null)
						id += "(" + OwnerId + ")";

				if (Permission == Permission.InvitePlus)
					id += "~canRequestInvite";

				switch (Region)
				{
					case ServerRegion.USW:
						id += "";
						break;

					case ServerRegion.USWWithIdentifier:
						id += "~region(us)";
						break;

					case ServerRegion.USE:
						id += "~region(use)";
						break;

					case ServerRegion.JP:
						id += "~region(jp)";
						break;

					case ServerRegion.EU:
						id += "~region(eu)";
						break;

					case ServerRegion.Custom:
						id += "~region(" + CustomRegion + ")";
						break;
				}

				if (Permission != Permission.Public)
					if (Nonce != null)
						id += "~nonce(" + Nonce + ")";

				return id;
			}
		}

		public string Token
		{
			get
			{
				string token = WorldId;
				string id = Id;

				if (id == "") return token;
				token += ":";
				token += id;

				return token;
			}
		}
        public string RegionName => Region switch
        {
            ServerRegion.USWWithIdentifier or ServerRegion.USW => "USW",
            ServerRegion.USE => "USE",
            ServerRegion.EU => "EU",
            ServerRegion.JP => "JP",
            _ => CustomRegion.ToUpper(),
        };

        public bool HasValidCustomRegionName => regionR.Match(CustomRegion).Success;

        public bool HasValidInstanceName => instanceNameR.Match(InstanceName).Success;

        [MemberNotNullWhen(true, nameof(Nonce))]
        public bool HasValidNonceValue => Nonce is not null && nonceR.Match(Nonce).Success;

        public bool HasValidWorldId => worldIdR.Match(WorldId).Success;
        [MemberNotNullWhen(true,nameof(OwnerId))]
		public bool HasValidOwnerId => OwnerId is not null && userIdR.Match(OwnerId).Success;
		

		public Instance(string token, string? worldName)
		{
            // Parse token
            {
				// NOTE:
				//   instanceName isn't contains ':'
				//   nonce, instanceName isn't contains '~'
				//   non-invite+ instances isn't contains
				//	 "canRequestInvite" parameter
				//   all non-home instances has instance-name
				//   Is valid? wrld_xx~aa
				//
				//   ほんまか？

				var instanceNamePrefixIndex = token.IndexOf(':');
				if(instanceNamePrefixIndex < 0)
                {
					WorldId = token;
					InstanceName = "";
					Permission = Permission.Unknown;

					return;
                }
				WorldId = token.Substring(0,instanceNamePrefixIndex);
				var instanceNameStartIndex = instanceNamePrefixIndex + 1;
				var parameterPrefixIndex = token.IndexOf('~');
				var instanceNameEndIndex = parameterPrefixIndex >= 0 ? parameterPrefixIndex : token.Length;
				InstanceName = token.Substring(instanceNameStartIndex,instanceNameEndIndex-instanceNameStartIndex);

				Permission = Permission.Public;
				var canRequestInvite = false;

				var parameters = token.AsSpan(instanceNameEndIndex);
				while (parameters.Length > 0)
				{
					parameters = parameters.Slice(1);
					var nextParameterPrefixIndex = parameters.IndexOf('~');
					var parameterEndIndex = nextParameterPrefixIndex >= 0 ? nextParameterPrefixIndex : parameters.Length;
					var parameter = parameters.Slice(0,parameterEndIndex);
					var keyValueSeparatorIndex = parameter.IndexOf('(');

					string key, value;
					if (keyValueSeparatorIndex < 0)
					{
						key = parameter.ToString();
						value = "";
					}
					else
					{
						key = parameter.Slice(0,keyValueSeparatorIndex).ToString();
						value = parameter.Slice(keyValueSeparatorIndex+1,parameter.Length-key.Length-2).ToString();
					}

					switch (key)
					{
						case "region":
							switch (value)
							{
								case "us":
									Region = ServerRegion.USW;
									break;

								case "use":
									Region = ServerRegion.USE;
									break;

								case "eu":
									Region = ServerRegion.EU;
									break;

								case "jp":
									Region = ServerRegion.JP;
									break;

								default:
									Region = ServerRegion.Custom;
									CustomRegion = value;
									break;
							}

							break;

						case "nonce":

							Nonce = value;
							break;



						case "private":

							Permission = Permission.InviteOnly;
							OwnerId = value;
							break;

						case "friends":

							Permission = Permission.Friends;
							OwnerId = value;
							break;

						case "hidden":

							Permission = Permission.FriendsPlus;
							OwnerId = value;
							break;

						case "canRequestInvite":

							canRequestInvite = true;
							break;

						default:
							break;
					}

					parameters = parameters.Slice(parameter.Length);
				}

				if (canRequestInvite)
				{
					Permission = Permission.InvitePlus;
				}
			}

			WorldName = worldName;
		}
	}
}
