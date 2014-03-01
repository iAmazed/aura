// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Channel.World.Entities;
using Aura.Channel.Scripting;

namespace Aura.Channel.Database
{
	public class Account
	{
		public string Id { get; set; }
		public long SessionKey { get; set; }

		public int Authority { get; set; }

		public DateTime LastLogin { get; set; }

		public string BanReason { get; set; }
		public DateTime BanExpiration { get; set; }

		public List<Character> Characters { get; set; }
		public List<Pet> Pets { get; set; }

		public ScriptVariables Vars { get; protected set; }

		public int AutobanScore { get; set; }
		public int AutobanCount { get; set; }
		public DateTime LastAutobanReduction { get; set; }

		public Account()
		{
			this.Characters = new List<Character>();
			this.Pets = new List<Pet>();
			this.Vars = new ScriptVariables();

			this.LastLogin = DateTime.Now;
		}

		public PlayerCreature GetCharacterOrPet(long entityId)
		{
			return GetCharacter(entityId) ?? GetPet(entityId) as PlayerCreature;
		}

		public Character GetCharacter(long entityId)
		{
			return this.Characters.FirstOrDefault(a => a.EntityId == entityId);
		}

		public Pet GetPet(long entityId)
		{
			return this.Pets.FirstOrDefault(a => a.EntityId == entityId);
		}
	}
}
