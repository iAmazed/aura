// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aura.Channel.Util;
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

		public PlayerCreature GetCharacterOrPetSafe(long entityId)
		{
			var c = this.GetCharacterOrPet(entityId);
			if (c != null)
				return c;

			throw new SevereViolation("Account does not contain an entity {0:X16}", entityId);
		}

		public PlayerCreature GetCharacterOrPet(long entityId)
		{
			return GetCharacter(entityId) ?? GetPet(entityId) as PlayerCreature;
		}

		public Character GetCharacterSafe(long entityId)
		{
			var c = this.GetCharacter(entityId);
			if (c != null)
				return c;

			throw new SevereViolation("Account does not contain a character {0:X16}", entityId);
		}

		public Character GetCharacter(long entityId)
		{
			return this.Characters.FirstOrDefault(a => a.EntityId == entityId);
		}

		public Pet GetPetSafe(long entityId)
		{
			var c = this.GetPet(entityId);
			if (c != null)
				return c;

			throw new SevereViolation("Account does not contain a pet {0:X16}", entityId);
		}

		public Pet GetPet(long entityId)
		{
			return this.Pets.FirstOrDefault(a => a.EntityId == entityId);
		}
	}
}
