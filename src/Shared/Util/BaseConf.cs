﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see licence file in the main folder

namespace Aura.Shared.Util
{
	public abstract class BaseConf : ConfFile
	{
		// Log
		public bool Archive;
		public LogLevel Hide;

		// Database
		public string Host;
		public string User;
		public string Pass;
		public string Db;

		public BaseConf()
		{
		}

		public abstract void Load();

		protected void LoadLog(string logFileName)
		{
			this.Archive = this.GetBool("log.archive", true);
			this.Hide = (LogLevel)this.GetInt("log.cmd_hide", 8);

			if (this.Archive)
				Log.Archive = "../../log/archive/";
			Log.LogFile = "../../log/" + logFileName + ".txt";
			Log.Hide |= this.Hide;
		}

		protected void LoadDatabase()
		{
			this.Host = this.GetString("database.host", "127.0.0.1");
			this.User = this.GetString("database.user", "root");
			this.Pass = this.GetString("database.pass", "");
			this.Db = this.GetString("database.db", "aura");
		}
	}
}
