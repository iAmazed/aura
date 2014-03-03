// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Channel.Util;
using Aura.Shared.Network;
using Aura.Channel.Network.Sending;

namespace Aura.Channel.Network.Handlers
{
	public partial class ChannelServerHandlers : PacketHandlerManager<ChannelClient>
	{
		/// <summary>
		/// Sent when pressing "Complete Quest".
		/// </summary>
		/// <example>
		/// 001 [0060F00000000004] Long   : 27285480554889220
		/// </example>
		[PacketHandler(Op.CompleteQuest)]
		public void CompleteQuest(ChannelClient client, Packet packet)
		{
			var uniqueQuestId = packet.GetLong();

			var creature = client.GetCreature(packet.Id);

			var quest = creature.Quests.Get(uniqueQuestId);
			if (quest == null || !quest.IsDone)
				throw new SevereAutoban(client, "'{0}' attempted to complete an already-finished quest.", creature.Name);

			if (creature.Quests.Complete(quest))
				Send.CompleteQuestR(creature, true);

			Send.CompleteQuestR(creature, false);
		}

		/// <summary>
		/// Sent when pressing "Give Up".
		/// </summary>
		/// <example>
		/// 001 [0060F00000000004] Long   : 27285480554889220
		/// </example>
		[PacketHandler(Op.GiveUpQuest)]
		public void GiveUpQuest(ChannelClient client, Packet packet)
		{
			var uniqueQuestId = packet.GetLong();

			var creature = client.GetCreature(packet.Id);

			var quest = creature.Quests.Get(uniqueQuestId);
			if (quest == null) goto L_Fail;

			if (!creature.Quests.GiveUp(quest)) goto L_Fail;

			Send.GiveUpQuestR(creature, true);
			return;

		L_Fail:
			Send.GiveUpQuestR(creature, false);
		}
	}
}
