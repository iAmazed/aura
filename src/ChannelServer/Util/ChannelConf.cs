﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using Aura.Shared.Util.Configuration;

namespace Aura.Channel.Util
{
	public class ChannelConf : BaseConf
	{
		/// <summary>
		/// channel.conf
		/// </summary>
		public ChannelConfFile Channel { get; protected set; }

		public ChannelConf()
		{
			this.Channel = new ChannelConfFile();
		}

		public override void Load()
		{
			this.LoadDefault();
			this.Channel.Load();
		}
	}

	public class ChannelConfFile : ConfFile
	{
		public string Host { get; protected set; }
		public int Port { get; protected set; }

		public string LoginHost { get; protected set; }
		public int LoginPort { get; protected set; }

		public string ChannelServer { get; protected set; }
		public string ChannelName { get; protected set; }
		public string ChannelHost { get; protected set; }
		public int ChannelPort { get; protected set; }
		public int MaxUsers { get; protected set; }

		public void Load()
		{
			this.RequireAndInclude("{0}/conf/channel.conf", "system", "user");

			this.Host = this.GetString("channel.host", "127.0.0.1");
			this.Port = this.GetInt("channel.port", 11020);

			this.LoginHost = this.GetString("channel.login_host", "127.0.0.1");
			this.LoginPort = this.GetInt("channel.login_port", 11020);

			this.ChannelServer = this.GetString("channel.channel_server", "127.0.0.1");
			this.ChannelName = this.GetString("channel.channel_name", "127.0.0.1");
			this.ChannelHost = this.GetString("channel.channel_host", "127.0.0.1");
			this.ChannelPort = this.GetInt("channel.channel_port", 11020);
			this.MaxUsers = this.GetInt("channel.max_users", 11020);
		}
	}
}