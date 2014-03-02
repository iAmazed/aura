using Aura.Channel.Database;
using Aura.Channel.Network;
using Aura.Shared.Util;
using System;

namespace Aura.Channel.Util
{
	public class Autoban
	{
		private readonly ChannelClient _client;

		public int Score
		{
			get
			{
				return _client.Account.AutobanScore;
			}
			set
			{
				if (value < 0)
					value = 0;

				_client.Account.AutobanScore = value;
			}
		}

		public int BanCount
		{
			get
			{
				return _client.Account.AutobanCount;
			}
			set
			{
				if (value < 0)
					value = 0;

				_client.Account.AutobanCount = 0;
			}
		}

		public Autoban(ChannelClient client)
		{
			_client = client;
		}

		public void Incident(IncidentSeverityLevel level, string report)
		{
			if (!ChannelServer.Instance.Conf.Autoban.Enabled)
				return;

			switch (level)
			{
				case IncidentSeverityLevel.Mild: Score += ChannelServer.Instance.Conf.Autoban.MildAmount; break;
				case IncidentSeverityLevel.Moderate: Score += ChannelServer.Instance.Conf.Autoban.ModerateAmount; break;
				case IncidentSeverityLevel.Severe: Score += ChannelServer.Instance.Conf.Autoban.SevereAmount; break;
			}

			Log.Info("Account '{0}' just committed a {1} offense. Total ban score: {2}. Incident report: {3}",
				_client.Account.Id, level, Score, report);

			ChannelDb.Instance.LogAutobanIncident(_client.Account, level, report);

			_client.Account.LastAutobanReduction = DateTime.Now;

			if (Score >= ChannelServer.Instance.Conf.Autoban.BanAt)
				this.Ban();

			_client.Kill();
		}

		private void Ban()
		{
			this.BanCount++;

			TimeSpan banLength;

			switch (ChannelServer.Instance.Conf.Autoban.LengthIncrease)
			{
				case AutobanLengthIncrease.None:
					banLength = ChannelServer.Instance.Conf.Autoban.InitialBanTime;
					break;
				case AutobanLengthIncrease.Linear:
					banLength = TimeSpan.FromTicks(ChannelServer.Instance.Conf.Autoban.InitialBanTime.Ticks * BanCount);
					break;
				default:
					banLength = TimeSpan.FromTicks(ChannelServer.Instance.Conf.Autoban.InitialBanTime.Ticks * (long)Math.Pow(2, BanCount - 1));
					break;
			}

			Log.Info("Autobanning account '{0}'. Total times they've been banned: {1}. Length of this ban: {2}.",
				_client.Account.Id, BanCount, banLength);

			_client.Account.BanExpiration = DateTime.Now + banLength;

			_client.Account.BanReason = "Automatic ban triggered.";

			if (ChannelServer.Instance.Conf.Autoban.ResetScoreOnBan)
				Score = 0;
		}
	}

	/// <summary>
	/// The exception to throw whever you want to invoke the autoban system
	/// </summary>
	public abstract class AutobanTriggeredException : Exception
	{
		public IncidentSeverityLevel Level { get; private set; }
		public Autoban Autoban { get; private set; }
		public string Report { get; private set; }

		public AutobanTriggeredException(Autoban ab, IncidentSeverityLevel lvl, string report)
		{
			this.Autoban = ab;
			this.Level = lvl;
			this.Report = report;
		}
	}

	/// <summary>
	/// Used to indicate something suspicious happened, but it
	/// *could* be nothing. This setting should be rarely used...
	/// </summary>
	public class MildAutoban: AutobanTriggeredException
	{
		public MildAutoban(ChannelClient client, string report, params object[] args)
			: base(client.Autoban,  IncidentSeverityLevel.Mild,
				new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + ": " + string.Format(report, args))
		{

		}
	}

	/// <summary>
	/// Something that is a strong indicator for a hack, but not certain.
	/// </summary>
	public class ModerateAutoban: AutobanTriggeredException
	{
		public ModerateAutoban(ChannelClient client, string report, params object[] args)
			: base(client.Autoban,  IncidentSeverityLevel.Moderate,
				new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + ": " + string.Format(report, args))
		{

		}
	}

	/// <summary>
	/// Something happened that could really only be caused by a hack tool
	/// </summary>
	public class SevereAutoban: AutobanTriggeredException
	{
		public SevereAutoban(ChannelClient client, string report, params object[] args)
			: base(client.Autoban,  IncidentSeverityLevel.Severe,
				new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + ": " + string.Format(report, args))
		{

		}
	}

	public enum IncidentSeverityLevel
	{
		Mild = 0,
		Moderate = 1,
		Severe = 2,
	}

	public enum AutobanLengthIncrease
	{
		None,
		Linear,
		Exponential
	}
}
