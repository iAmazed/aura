﻿// Copyright (c) Aura development team - Licensed under GNU GPL
// For more information, see license file in the main folder

using System;
using System.Linq;
using Aura.Channel.World.Entities;
using Aura.Data;
using Aura.Shared.Mabi.Const;
using Aura.Shared.Network;
using Aura.Channel.World.Entities.Creatures;
using Aura.Shared.Mabi.Structs;
using Aura.Shared.Util;
using Aura.Channel.World.Quests;

namespace Aura.Channel.Network.Sending
{
	public static partial class Send
	{
		private static Packet AddPublicEntityInfo(this Packet packet, Entity entity)
		{
			switch (entity.DataType)
			{
				case DataType.Creature: packet.AddCreatureInfo(entity as Creature, CreaturePacketType.Public); break;
				case DataType.Item: packet.AddItemInfo(entity as Item, ItemPacketType.Public); break;
				case DataType.Prop: packet.AddPropInfo(entity as Prop); break;
				default:
					throw new Exception("Unknown entity type '" + entity.GetType().ToString() + "', '" + entity.DataType + "'.");
			}

			return packet;
		}

		public static Packet AddCreatureInfo(this Packet packet, Creature creature, CreaturePacketType type)
		{
			// Check for MabiPC for private data.
			var character = creature as PlayerCreature;
			if (type == CreaturePacketType.Private && character == null)
				throw new Exception("PlayerCreature required for private creature packet.");

			var pos = creature.GetPosition();

			// Start
			// --------------------------------------------------------------

			packet.PutLong(creature.EntityId);
			packet.PutByte((byte)type);

			// Looks/Location
			// --------------------------------------------------------------
			packet.PutString(creature.Name);
			packet.PutString("");				 // Title
			packet.PutString("");				 // Eng Title
			packet.PutInt(creature.Race);
			packet.PutByte(creature.SkinColor);
			packet.PutShort(creature.EyeType); // [180600, NA187 (25.06.2014)] Changed from byte to short
			packet.PutByte(creature.EyeColor);
			packet.PutByte(creature.MouthType);
			packet.PutUInt((uint)creature.State);
			if (type == CreaturePacketType.Public)
			{
				packet.PutUInt((uint)creature.StateEx);

				// [180300, NA166 (18.09.2013)
				{
					packet.PutInt(0);
				}
			}
			packet.PutFloat(creature.Height);
			packet.PutFloat(creature.Weight);
			packet.PutFloat(creature.Upper);
			packet.PutFloat(creature.Lower);
			packet.PutInt(creature.RegionId);
			packet.PutInt(pos.X);
			packet.PutInt(pos.Y);
			packet.PutByte(creature.Direction);
			packet.PutInt(Convert.ToInt32(creature.IsInBattleStance));
			packet.PutByte((byte)creature.Inventory.WeaponSet);
			packet.PutUInt(creature.Color1);
			packet.PutUInt(creature.Color2);
			packet.PutUInt(creature.Color3);

			// Stats
			// --------------------------------------------------------------
			packet.PutFloat(creature.CombatPower);
			packet.PutString(creature.StandStyle);

			if (type == CreaturePacketType.Private)
			{
				packet.PutFloat(creature.Life);
				packet.PutFloat(creature.LifeInjured);
				packet.PutFloat(creature.LifeMaxBaseTotal);
				packet.PutFloat(creature.LifeMaxMod);
				packet.PutFloat(creature.Mana);
				packet.PutFloat(creature.ManaMaxBaseTotal);
				packet.PutFloat(creature.ManaMaxMod);
				packet.PutFloat(creature.Stamina);
				packet.PutFloat(creature.StaminaMaxBaseTotal);
				packet.PutFloat(creature.StaminaMaxMod);
				packet.PutFloat(creature.StaminaHunger);
				packet.PutFloat(0.5f);
				packet.PutShort(creature.Level);
				packet.PutInt(creature.LevelTotal);
				packet.PutShort(0);                  // Max Level (reached ever?)
				packet.PutShort(0);					 // Rebirthes
				packet.PutShort(0);
				packet.PutLong(AuraData.ExpDb.CalculateRemaining(creature.Level, creature.Exp) * 1000);
				packet.PutShort(creature.Age);
				packet.PutFloat(creature.StrBaseTotal);
				packet.PutFloat(creature.StrMod);
				packet.PutFloat(creature.DexBaseTotal);
				packet.PutFloat(creature.DexMod);
				packet.PutFloat(creature.IntBaseTotal);
				packet.PutFloat(creature.IntMod);
				packet.PutFloat(creature.WillBaseTotal);
				packet.PutFloat(creature.WillMod);
				packet.PutFloat(creature.LuckBaseTotal);
				packet.PutFloat(creature.LuckMod);
				packet.PutFloat(creature.LifeFoodMod);
				packet.PutFloat(creature.ManaFoodMod);
				packet.PutFloat(creature.StaminaFoodMod);
				packet.PutFloat(creature.StrFoodMod);
				packet.PutFloat(creature.DexFoodMod);
				packet.PutFloat(creature.IntFoodMod);
				packet.PutFloat(creature.WillFoodMod);
				packet.PutFloat(creature.LuckFoodMod);
				packet.PutShort(creature.AbilityPoints);
				packet.PutShort(0);			         // AttackMinBase
				packet.PutShort((short)creature.AttackMinMod);
				packet.PutShort(0);			         // AttackMaxBase
				packet.PutShort((short)creature.AttackMaxMod);
				packet.PutShort(0);			         // WAttackMinBase
				packet.PutShort(0);			         // WAttackMinMod
				packet.PutShort(0);			         // WAttackMaxBase
				packet.PutShort(0);			         // WAttackMaxMod
				packet.PutShort(0);			         // LeftAttackMinMod
				packet.PutShort(0);			         // LeftAttackMaxMod
				packet.PutShort(0);			         // RightAttackMinMod
				packet.PutShort(0);			         // RightAttackMaxMod
				packet.PutShort(0);			         // LeftWAttackMinMod
				packet.PutShort(0);			         // LeftWAttackMaxMod
				packet.PutShort(0);			         // RightWAttackMinMod
				packet.PutShort(0);			         // RightWAttackMaxMod
				packet.PutFloat(0);			         // LeftCriticalMod
				packet.PutFloat(0);			         // RightCriticalMod
				packet.PutShort(0);			         // LeftRateMod
				packet.PutShort(0);			         // RightRateMod
				packet.PutFloat(0);			         // MagicDefenseMod
				// [180300, NA166 (18.09.2013)] Magic Protection
				{
					packet.PutFloat(0);			     // MagicProtectMod
				}
				packet.PutFloat(0);			         // MagicAttackMod
				packet.PutShort(15);		         // MeleeAttackRateMod
				packet.PutShort(15);		         // RangeAttackRateMod
				packet.PutFloat(0);			         // CriticalBase
				packet.PutFloat(0);			         // CriticalMod
				packet.PutFloat((short)creature.ProtectionBase);
				packet.PutFloat(creature.ProtectionMod);
				packet.PutShort((short)creature.DefenseBase);
				packet.PutShort((short)creature.DefenseMod);
				packet.PutShort(0);			         // RateBase
				packet.PutShort(0);			         // RateMod
				packet.PutShort(0);			         // Rank1
				packet.PutShort(0);			         // Rank2
				// [180300, NA166 (18.09.2013)] Armor Pierce
				{
					packet.PutShort(0);			     // ArmorPierceMod
				}
				packet.PutLong(0);			         // Score
				packet.PutShort((short)creature.AttackMinBaseMod);
				packet.PutShort((short)creature.AttackMaxBaseMod);
				packet.PutShort((short)creature.WAttackMinBase);
				packet.PutShort((short)creature.WAttackMaxBase);
				packet.PutFloat(creature.CriticalBase * 100);
				packet.PutFloat(creature.ProtectionBaseMod);
				packet.PutShort((short)creature.DefenseBaseMod);
				packet.PutShort((short)(creature.BalanceBase * 100));

				// In some tests the damage display would be messed up if
				// those two weren't set to something.
				packet.PutShort(0);                  // MeleeAttackMinBaseMod (8 / 3)
				packet.PutShort(0);                  // MeleeAttackMaxBaseMod (18 / 4)

				packet.PutShort(0);                  // MeleeWAttackMinBaseMod
				packet.PutShort(0);                  // MeleeWAttackMaxBaseMod
				packet.PutShort(0);                  // RangeAttackMinBaseMod (10)
				packet.PutShort(0);                  // RangeAttackMaxBaseMod (25)
				packet.PutShort(0);                  // RangeWAttackMinBaseMod
				packet.PutShort(0);                  // RangeWAttackMaxBaseMod
				// [180100] Guns
				{
					packet.PutShort(0);			     // DualgunAttackMinBaseMod
					packet.PutShort(0);			     // DualgunAttackMaxBaseMod
					packet.PutShort(0);			     // DualgunWAttackMinBaseMod
					packet.PutShort(0);			     // DualgunWAttackMaxBaseMod
				}
				// [180800, NA189 - 2014-07-23] Ninja?
				{
					packet.PutShort(0);			     // ? AttackMinBaseMod
					packet.PutShort(0);			     // ? AttackMaxBaseMod
					packet.PutShort(0);			     // ? WAttackMinBaseMod
					packet.PutShort(0);			     // ? WAttackMaxBaseMod
				}
				packet.PutShort(0);			         // PoisonBase
				packet.PutShort(0);			         // PoisonMod
				packet.PutShort(67);		         // PoisonImmuneBase
				packet.PutShort(0);			         // PoisonImmuneMod
				packet.PutFloat(0.5f);		         // PoisonDamageRatio1
				packet.PutFloat(0);			         // PoisonDamageRatio2
				packet.PutFloat(0);			         // toxicStr
				packet.PutFloat(0);			         // toxicInt
				packet.PutFloat(0);			         // toxicDex
				packet.PutFloat(0);			         // toxicWill
				packet.PutFloat(0);			         // toxicLuck
				packet.PutString("Uladh_main/town_TirChonaill/TirChonaill_Spawn_A"); // Last town
				packet.PutShort(1);					 // ExploLevel
				packet.PutShort(0);					 // ExploMaxKeyLevel
				packet.PutInt(0);					 // ExploCumLevel
				packet.PutLong(0);					 // ExploExp
				packet.PutInt(0);					 // DiscoverCount
				packet.PutFloat(0);					 // conditionStr
				packet.PutFloat(0);					 // conditionInt
				packet.PutFloat(0);					 // conditionDex
				packet.PutFloat(0);					 // conditionWill
				packet.PutFloat(0);					 // conditionLuck
				packet.PutByte(9);					 // ElementPhysical
				packet.PutByte(0);					 // ElementLightning
				packet.PutByte(0);					 // ElementFire
				packet.PutByte(0);					 // ElementIce

				var regens = creature.Regens.GetList();
				packet.PutInt(regens.Count);
				foreach (var regen in regens)
					packet.AddRegen(regen);
			}
			else if (type == CreaturePacketType.Public || type == CreaturePacketType.Minimal)
			{
				packet.PutFloat(creature.Life);
				packet.PutFloat(creature.LifeMaxBaseTotal);
				packet.PutFloat(creature.LifeMaxMod);
				packet.PutFloat(creature.LifeInjured);

				var regens = creature.Regens.GetPublicList();
				packet.PutInt(regens.Count);
				foreach (var regen in regens)
					packet.AddRegen(regen);

				// Another 6 elements list?
				packet.PutInt(0);
			}

			// Titles
			// --------------------------------------------------------------
			packet.PutUShort(creature.Titles.SelectedTitle);
			packet.PutLong(creature.Titles.Applied);
			if (type == CreaturePacketType.Private)
			{
				// List of available titles
				var titles = character.Titles.GetList();
				packet.PutShort((short)titles.Count);
				foreach (var title in titles)
				{
					packet.PutUShort(title.Key);
					packet.PutByte((byte)title.Value);
					packet.PutInt(0);
				}
			}
			if (type == CreaturePacketType.Private || type == CreaturePacketType.Public)
			{
				packet.PutUShort(creature.Titles.SelectedOptionTitle);
			}

			// Items and expiring? (Last part of minimal)
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Minimal)
			{
				packet.PutString("");
				packet.PutByte(0);

				var items = creature.Inventory.Equipment;

				packet.PutInt(items.Count());
				foreach (var item in items)
				{
					packet.PutLong(item.EntityId);
					packet.PutBin(item.Info);
				}

				packet.PutInt(0);  // PetRemainingTime
				packet.PutLong(0); // PetLastTime
				packet.PutLong(0); // PetExpireTime

				return packet;
			}

			// Mate
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutLong(0);					 // MateID
				packet.PutString("");				 // MateName
				packet.PutLong(0);					 // MarriageTime
				packet.PutShort(0);					 // MarriageCount
			}
			else if (type == CreaturePacketType.Public)
			{
				packet.PutString("");				 // MateName
			}

			// Destiny
			// --------------------------------------------------------------
			packet.PutByte(0);			             // (0:Venturer, 1:Knight, 2:Wizard, 3:Bard, 4:Merchant, 5:Alchemist)

			// Inventory
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(creature.RaceData.InventoryWidth);
				packet.PutInt(creature.RaceData.InventoryHeight);

				var items = creature.Inventory.Items;
				packet.PutInt(items.Count());
				foreach (var item in items)
					packet.AddItemInfo(item, ItemPacketType.Private);
			}
			else if (type == CreaturePacketType.Public)
			{
				var items = creature.Inventory.Equipment;

				packet.PutInt(items.Count());
				foreach (var item in items)
				{
					packet.PutLong(item.EntityId);
					packet.PutBin(item.Info);
				}
			}

			// [180300, NA169 (23.10.2013)] ?
			// Strange one, it's in the logs, but stucks the char in
			// casting animation. Dependent on something?
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				//packet.PutInt(2); // Count?
				//packet.PutInt(36);
				//packet.PutInt(8);
				//packet.PutInt(38);
				//packet.PutInt(4);
			}

			// Keywords
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				var keywords = character.Keywords.GetList();
				packet.PutShort((short)keywords.Count);
				foreach (var keyword in keywords)
					packet.PutUShort(keyword);
			}

			// Skills
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				var skills = creature.Skills.GetList();
				packet.PutShort((short)skills.Count);
				foreach (var skill in skills)
					packet.PutBin(skill.Info);
				packet.PutInt(0);			     // SkillVarBufferList
				// loop						         
				//   packet.PutInt
				//   packet.PutFloat
			}
			else if (type == CreaturePacketType.Public)
			{
				packet.PutShort(0);			     // CurrentSkill
				packet.PutByte(0);			     // SkillStackCount
				packet.PutInt(0);			     // SkillProgress
				packet.PutInt(0);			     // SkillSyncList
				// loop						         
				//   packet.PutShort
				//   packet.PutShort
			}

			// [150100] ?
			{
				packet.PutByte(0);			     // {PLGCNT}
			}

			// Party
			// --------------------------------------------------------------
			//if (creature.Party != null)
			//{
			//    packet.PutByte(creature.Party.IsOpen && creature.Party.Leader == creature);
			//    packet.PutString(creature.Party.GetMemberWantedString());
			//}
			//else
			{
				packet.PutByte(0);
				packet.PutString("");
			}

			// PvP
			// --------------------------------------------------------------
			packet.AddPvPInfo(creature);

			// Conditions
			// --------------------------------------------------------------
			packet.AddConditions(creature.Conditions);

			// Guild
			// --------------------------------------------------------------
			//if (creature.Guild != null)
			//{
			//    packet.PutLong(creature.Guild.Id);
			//    packet.PutString(creature.Guild.Name);
			//    packet.PutInt((uint)creature.GuildMember.MemberRank);
			//    packet.PutByte(0);
			//    packet.PutByte(0);
			//    packet.PutByte(0);
			//    packet.PutInt(0);
			//    packet.PutByte(0);
			//    packet.PutByte(0);
			//    packet.PutByte(0);
			//    packet.PutByte(0);
			//    packet.PutString(creature.Guild.Title);
			//}
			//else
			{
				packet.PutLong(0);
				packet.PutString("");
				packet.PutInt(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutInt(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutString("");
			}

			// PTJ
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutLong(0);				     // ArbeitID
				packet.PutInt(0);				     // ArbeitRecordList
				// loop
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutShort
			}

			// Following a master
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				if (creature.Master != null)
				{
					packet.PutLong(creature.Master.EntityId);
					packet.PutByte(2);               // Type (1:RPCharacter, 2:Pet, 3:Transport, 4:PartnerVehicle)
					packet.PutByte(0);				 // SubType
				}
				else
				{
					packet.PutLong(0);
					packet.PutByte(0);
					packet.PutByte(0);
				}
			}

			// [170100] ?
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutFloat(1);
				packet.PutLong(0);
			}

			// Transformation
			// --------------------------------------------------------------
			packet.PutByte(0);				     // Type (1:Paladin, 2:DarkKnight, 3:SubraceTransformed, 4:TransformedElf, 5:TransformedGiant)
			packet.PutShort(0);				     // Level
			packet.PutShort(0);				     // SubType

			// Pet
			// --------------------------------------------------------------
			if (creature.Master != null)
			{
				packet.PutString(creature.Master.Name);

				if (type == CreaturePacketType.Private)
				{
					packet.PutInt(2000000000);			// RemainingSummonTime
					packet.PutLong(0);					// LastSummonTime
					packet.PutLong(0);					// PetExpireTime
					packet.PutByte(0);					// Loyalty
					packet.PutByte(0);					// Favor
					packet.PutLong(DateTime.Now);		// SummonTime
					packet.PutByte(0);					// KeepingMode
					packet.PutLong(0);					// KeepingProp
					packet.PutLong(creature.Master.EntityId);
					packet.PutByte(0);					// PetSealCount {PSCNT}
				}
				else if (type == CreaturePacketType.Public)
				{
					packet.PutLong(creature.Master.EntityId);
					packet.PutByte(0);				 // KeepingMode
					packet.PutLong(0);				 // KeepingProp
				}
			}
			else
			{
				packet.PutString("");

				if (type == CreaturePacketType.Private)
				{
					packet.PutInt(0);
					packet.PutLong(0);
					packet.PutLong(0);
					packet.PutByte(0);
					packet.PutByte(0);
					packet.PutLong(0);
					packet.PutByte(0);
					packet.PutLong(0);
					packet.PutLong(0);
					packet.PutByte(0);
				}
				else if (type == CreaturePacketType.Public)
				{
					packet.PutLong(0);
					packet.PutByte(0);
					packet.PutLong(0);
				}
			}

			// House
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
				packet.PutLong(0);				 // HouseID

			// Taming
			// --------------------------------------------------------------
			packet.PutLong(0);					 // MasterID
			packet.PutByte(0);					 // IsTamed
			packet.PutByte(0);					 // TamedType (1:DarkKnightTamed, 2:InstrumentTamed, 3:AnimalTraining, 4:MercenaryTamed, 5:Recalled, 6:SoulStoneTamed, 7:TamedFriend)
			packet.PutByte(1);					 // IsMasterMode
			packet.PutInt(0);					 // LimitTime

			// Vehicle
			// --------------------------------------------------------------
			packet.PutInt(0);					 // Type
			packet.PutInt(0);					 // TypeFlag (0x1:Driver, 0x4:Owner)
			packet.PutLong(0);					 // VehicleId
			packet.PutInt(0);					 // SeatIndex
			packet.PutByte(0);					 // PassengerList
			// loop
			//   packet.PutLong

			// Showdown
			// --------------------------------------------------------------
			packet.PutInt(0);	                 // unknown at 0x18
			packet.PutLong(0);                   // unknown at 0x08
			packet.PutLong(0);	                 // unknown at 0x10
			packet.PutByte(1);	                 // IsPartyPvpDropout

			// Transport
			// --------------------------------------------------------------
			packet.PutLong(0);					 // TransportID
			packet.PutInt(0);					 // HuntPoint

			// Aviation
			// --------------------------------------------------------------
			packet.PutByte(0); // --v
			//packet.PutByte(creature.IsFlying);
			//if (creature.IsFlying)
			//{
			//    var pos = creature.GetPosition();
			//    packet.PutFloat(pos.X);
			//    packet.PutFloat(pos.H);
			//    packet.PutFloat(pos.Y);
			//    packet.PutFloat(creature.Destination.X);
			//    packet.PutFloat(creature.Destination.H);
			//    packet.PutFloat(creature.Destination.Y);
			//    packet.PutFloat(creature.Direction);
			//}

			// Skiing
			// --------------------------------------------------------------
			packet.PutByte(0);					 // IsSkiing
			// loop
			//   packet.PutFloat
			//   packet.PutFloat
			//   packet.PutFloat
			//   packet.PutFloat
			//   packet.PutInt
			//   packet.PutInt
			//   packet.PutByte
			//   packet.PutByte

			// Farming
			// [150100-170400] Public too
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutLong(0);					 // FarmId
				//   packet.PutLong
				//   packet.PutLong
				//   packet.PutLong
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutShort
				//   packet.PutByte
				//   packet.PutLong
				//   packet.PutByte
				//   packet.PutLong
			}

			// Event (CaptureTheFlag, WaterBalloonBattle)
			// --------------------------------------------------------------
			packet.PutByte(0);				     // EventFullSuitIndex
			packet.PutByte(0);				     // TeamId
			// if?
			//   packet.PutInt					 // HitPoint
			//   packet.PutInt					 // MaxHitPoint

			// [170300] ?
			{
				packet.PutString("");
				packet.PutByte(0);
			}

			// Heartstickers
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutShort(0);
				packet.PutShort(0);
			}

			// Joust
			// --------------------------------------------------------------
			packet.PutInt(0);					 // JoustId
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(0);					 // JoustPoint
				packet.PutByte(0);					 // unknown at 0x1D
				packet.PutByte(0);					 // unknown at 0x1C
				packet.PutByte(0);					 // WeekWinCount
				packet.PutShort(0);					 // DailyWinCount
				packet.PutShort(0);					 // DailyLoseCount
				packet.PutShort(0);					 // ServerWinCount
				packet.PutShort(0);					 // ServerLoseCount
			}
			else if (type == CreaturePacketType.Public)
			{
				packet.PutLong(0);			         // HorseId
				packet.PutFloat(0);	                 // Life
				packet.PutInt(100);		             // LifeMax
				packet.PutByte(9);			         // unknown at 0x6C
				packet.PutByte(0);			         // IsJousting
			}

			// Achievements
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(0);	                 // TotalScore
				packet.PutShort(0);                  // AchievementList
				// loop
				//   packet.PutShort achievementId
			}

			// PrivateFarm
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(0);					 // FavoriteFarmList
				// loop
				//   packet.PutLong                  // FarmId
				//   packet.PutInt                   // ZoneId
				//   packet.PutShort                 // PosX
				//   packet.PutShort                 // PoxY
				//   packet.PutString                // FarmName
				//   packet.PutString                // OwnerName
			}

			// Family
			// --------------------------------------------------------------
			packet.PutLong(0);					 // FamilyId
			// if
			//   packet.PutString				 // FamilyName
			//   packet.PutShort
			//   packet.PutShort
			//   packet.PutShort
			//   packet.PutString				 // FamilyTitle

			// Demigod
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(0);					 // SupportType (0:None, 1:Neamhain, 2:Morrighan)
			}

			// [150100] NPC options
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Public && creature is NPC)
			{
				packet.PutShort(0);		         // OnlyShowFilter
				packet.PutShort(0);		         // HideFilter
			}

			// [150100] Commerce
			// --------------------------------------------------------------
			{
				packet.PutByte(1);               // IsInCommerceCombat
				packet.PutLong(0);               // TransportCharacterId
				packet.PutFloat(1);              // ScaleHeight
			}

			// [170100] Talents
			// --------------------------------------------------------------
			{
				if (type == CreaturePacketType.Public)
				{
					packet.PutLong(0);
					packet.PutByte(0);
					packet.PutByte(0);
					packet.PutFloat(1);
					packet.PutLong(0);

					packet.PutShort(0); // --v
					packet.PutByte(0);  // --v
					//packet.PutShort((ushort)creature.Talents.SelectedTitle);
					//packet.PutByte((byte)creature.Talents.Grandmaster);
				}
				else if (type == CreaturePacketType.Private)
				{
					packet.AddPrivateTalentInfo(creature);
				}
			}

			// [170300] Shamala
			// --------------------------------------------------------------
			{
				if (type == CreaturePacketType.Private)
				{
					// Transformation Diary
					packet.PutInt(0); // --v
					//packet.PutSInt(character.Shamalas.Count);
					//foreach (var trans in character.Shamalas)
					//{
					//    packet.PutInt(trans.Id);
					//    packet.PutByte(trans.Counter);
					//    packet.PutByte((byte)trans.State);
					//}
				}
				else if (type == CreaturePacketType.Public)
				{
					// Current transformation info
					//if (creature.Shamala != null)
					//{
					//    packet.PutInt(creature.Shamala.Id);
					//    packet.PutByte(0);
					//    packet.PutInt(creature.ShamalaRace.Id);
					//    packet.PutFloat(creature.Shamala.Size);
					//    packet.PutInt(creature.Shamala.Color1);
					//    packet.PutInt(creature.Shamala.Color2);
					//    packet.PutInt(creature.Shamala.Color3);
					//}
					//else
					{
						packet.PutInt(0);
						packet.PutByte(0);
						packet.PutInt(0);
						packet.PutFloat(1);
						packet.PutInt(0x808080);
						packet.PutInt(0x808080);
						packet.PutInt(0x808080);
					}
					packet.PutByte(0);
					packet.PutByte(0);
				}
			}

			// [180100] ?
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Private)
			{
				packet.PutInt(0);
				packet.PutInt(0);
			}

			// [NA170403, TW170300] ?
			// --------------------------------------------------------------
			{
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutLong(0);

				// Rock/Paper/Scissors?
				packet.PutString(""); // Banner text?
				packet.PutByte(0);    // Banner enabled?
			}

			// [180300, NA166 (18.09.2013)] ?
			// Required, even though it looks like a list.
			// --------------------------------------------------------------
			{
				packet.PutInt(10); // Count?
				packet.PutLong(4194304);
				packet.PutInt(1347950097);
				packet.PutLong(34359771136);
				packet.PutInt(1346340501);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
				packet.PutLong(0);
				packet.PutInt(0);
			}

			// [180500, NA181 (12.02.2014)] ?
			// Without this the "me" creature in the Smash cutscene had a
			// red aura.
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Public)
			{
				packet.PutByte(0);
			}

			// Character
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Public)
			{
				packet.PutLong(0);			         // AimingTarget
				packet.PutLong(0);			         // Executor
				packet.PutShort(0);			         // ReviveTypeList
				// loop						         
				//   packet.PutInt	

				// < int g18 monsters?
			}

			packet.PutByte(0);					 // IsGhost

			// SittingProp
			if (creature.Temp.SittingProp == null)
				packet.PutLong(0);
			else
				packet.PutLong(creature.Temp.SittingProp.EntityId);

			packet.PutInt(-1);					 // SittedSocialMotionId

			// ? (Last Part of public)
			// --------------------------------------------------------------
			if (type == CreaturePacketType.Public)
			{
				packet.PutLong(0);			         // DoubleGoreTarget (Doppelganger condition)
				packet.PutInt(0);			         // DoubleGoreTargetType

				// [180300, NA169 (23.10.2013)] ?
				{
					packet.PutLong(0);
				}

				if (!creature.IsMoving)
				{
					packet.PutByte(0);
				}
				else
				{
					var dest = creature.GetDestination();

					packet.PutByte((byte)(!creature.IsWalking ? 2 : 1));
					packet.PutInt(dest.X);
					packet.PutInt(dest.Y);
				}

				if (creature is NPC)
				{
					packet.PutString(creature.StandStyleTalking);
				}

				// [150100] Bomb Event
				{
					packet.PutByte(0);			     // BombEventState
				}

				// [170400] ?
				{
					packet.PutByte(0);
				}

				// [180?00] ?
				{
					packet.PutByte(1);
				}

				// [180500, NA181 (12.02.2014)] ?
				{
					packet.PutByte(1);
				}

				return packet;
			}

			// private:

			// [JP] ?
			// This int is needed in the JP client (1704 log),
			// but doesn't appear in the NA 1704 or KR test 1801 log.
			{
				//packet.PutInt(4);
			}

			// Premium stuff
			// --------------------------------------------------------------
			// [180600, NA187 (25.06.2014)] ?
			{
				packet.PutByte(0);
			}
			packet.PutByte(0);                   // IsUsingExtraStorage (old service)
			packet.PutByte(1);                   // IsUsingNaosSupport (old service) (Style tab in 1803?)
			packet.PutByte(0);                   // IsUsingAdvancedPlay (old service)
			packet.PutByte(0);                   // PremiumService 0
			packet.PutByte(0);                   // PremiumService 1
			packet.PutByte(1);                   // Premium Gestures
			packet.PutByte(1);					 // ? (Default 1 on NA?)
			packet.PutByte(0);
			// [170402, TW170300] New premium thing
			{
				packet.PutByte(1); // VIP inv? (since 1803?)
			}
			// [180300, NA166 (18.09.2013)] ?
			{
				packet.PutByte(0);
				packet.PutByte(0);
			}
			packet.PutInt(0);
			packet.PutByte(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);

			// Quests
			// --------------------------------------------------------------
			var quests = character.Quests.GetIncompleteList();
			packet.PutInt(quests.Count);
			foreach (var quest in quests)
				packet.AddQuest(quest);

			// Char
			// --------------------------------------------------------------
			packet.PutByte(0);					 // NaoDress (0:normal, 12:??, 13:??)
			packet.PutLong(character.CreationTime);
			packet.PutLong(character.LastRebirth);
			packet.PutString("");
			packet.PutByte(0); // "true" makes character lie on floor?
			packet.PutByte(2);

			// [150100] Pocket ExpireTime List
			// Apperantly a list of "pockets"?, incl expiration time.
			// Ends with a long 0?
			// --------------------------------------------------------------
			{
				// Style
				packet.PutLong(DateTime.Now.AddMonths(1));
				packet.PutShort(72);

				// ?
				//packet.PutLong(0);
				//packet.PutShort(73);

				packet.PutLong(0);
			}

			return packet;
		}

		private static Packet AddPvPInfo(this Packet packet, Creature creature)
		{
			packet.PutByte(0);
			packet.PutInt(0);
			packet.PutByte(0);
			packet.PutInt(0);
			packet.PutByte(0);
			packet.PutByte(0);
			packet.PutByte(1);
			packet.PutLong(0);
			packet.PutLong(0);
			packet.PutInt(0);
			packet.PutByte(1);
			// --v

			//var arena = creature.ArenaPvPManager != null;

			//packet.PutByte(arena); // ArenaPvP
			//packet.PutInt(arena ? creature.ArenaPvPManager.GetTeam(creature) : (uint)0);
			//packet.PutByte(creature.TransPvPEnabled);
			//packet.PutInt(arena ? creature.ArenaPvPManager.GetStars(creature) : 0);
			//packet.PutByte(creature.EvGEnabled);
			//packet.PutByte(creature.EvGSupportRace);
			//packet.PutByte(1); // IsPvPMode
			//packet.PutLong(creature.PvPWins);
			//packet.PutLong(creature.PvPLosses);
			//packet.PutInt(0);// PenaltyPoints
			//packet.PutByte(1);  // unk

			// [170300] ?
			{
				packet.PutByte(0);
				packet.PutInt(0);
				packet.PutInt(0);
				packet.PutInt(0);
				packet.PutInt(0);
			}

			return packet;
		}

		private static Packet AddPrivateTalentInfo(this Packet packet, Creature creature)
		{
			packet.PutShort(0);
			packet.PutByte(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			packet.PutInt(0);
			// --v
			//packet.PutShort((short)creature.Talents.SelectedTitle);
			//packet.PutByte((byte)creature.Talents.Grandmaster);
			//packet.PutInt(creature.Talents.GetExp(TalentId.Adventure));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Warrior));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Mage));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Archer));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Merchant));
			//packet.PutInt(creature.Talents.GetExp(TalentId.BattleAlchemy));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Fighter));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Bard));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Puppeteer));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Knight));
			//packet.PutInt(creature.Talents.GetExp(TalentId.HolyArts));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Transmutaion));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Cooking));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Blacksmith));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Tailoring));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Medicine));
			//packet.PutInt(creature.Talents.GetExp(TalentId.Carpentry));
			// [180100] Zero Talent
			{
				packet.PutInt(0);
			}
			// [180800, NA189 - 2014-07-23] Ninja?
			{
				packet.PutInt(0);
			}

			// Talent titles
			// ----------------------------------------------------------
			//var titles = creature.Talents.GetTitles();

			packet.PutByte(0);// --v
			//packet.PutByte((byte)titles.Count);
			//foreach (var title in titles)
			//    packet.PutShort(title);

			return packet;
		}

		private static Packet AddItemInfo(this Packet packet, Item item, ItemPacketType type)
		{
			packet.PutLong(item.EntityId);
			packet.PutByte((byte)type);
			packet.PutBin(item.Info);

			if (type == ItemPacketType.Public)
			{
				packet.PutByte(1);
				packet.PutByte(0);

				//packet.PutByte(0); // Bitmask
				// if & 1
				//     float
				packet.PutByte(1);
				packet.PutFloat(1); // Size multiplicator *hint: Server side giant key mod*

				packet.PutByte(item.FirstTimeAppear); // 0: No bouncing, 1: Bouncing, 2: Delayed bouncing
			}
			else if (type == ItemPacketType.Private)
			{
				packet.PutBin(item.OptionInfo);
				packet.PutString(item.MetaData1.ToString());
				packet.PutString(item.MetaData2.ToString());

				// Egos (example)
				//0969 [................] String : Navi
				//0970 [..............02] Byte   : 2
				//0971 [..............0E] Byte   : 14
				//0972 [..............0E] Byte   : 14
				//0973 [........00000F50] Int    : 3920
				//0974 [..............11] Byte   : 17
				//0975 [........000013B6] Int    : 5046
				//0976 [..............05] Byte   : 5
				//0977 [........00000092] Int    : 146
				//0978 [..............04] Byte   : 4
				//0979 [........000000C9] Int    : 201
				//0980 [..............03] Byte   : 3
				//0981 [........0000007E] Int    : 126
				//0982 [..............03] Byte   : 3
				//0983 [........000000B5] Int    : 181
				//0984 [..............00] Byte   : 0
				//0985 [........00000000] Int    : 0
				//0986 [0000000000000000] Long   : 0
				//0987 [000039C6DF641228] Long   : 63526314185256 (26.01.2014 06:23:05)
				//0988 [........00000000] Int    : 0

				// Upgrades?
				packet.PutByte(0); // Count
				// for upgrades
				//     Bin    : 01 00 00 00 68 21 11 00 00 00 00 00 05 00 1E 00 00 00 00 00 0A 00 00 00 D3 E4 90 65 0A 00 00 00 F0 18 9E 65

				// Special upgrades? (example)
				//0608 [0000000000000000] Long   : 0
				//0609 [........00000002] Int    : 2
				//0610 [........00000024] Int    : 36
				//0611 [........00000008] Int    : 8
				//0612 [........00000026] Int    : 38
				//0613 [........00000004] Int    : 4

				packet.PutLong(item.QuestId);
			}

			return packet;
		}

		private static Packet AddPropInfo(this Packet packet, Prop prop)
		{
			packet.PutLong(prop.EntityId);
			packet.PutInt(prop.Info.Id);

			// Client side props (A0 range, instead of A1)
			// look a bit different.
			if (prop.ServerSide)
			{
				packet.PutString(prop.Name);
				packet.PutString(prop.Title);
				packet.PutBin(prop.Info);
				packet.PutString(prop.State);
				packet.PutLong(0);

				if (prop.HasXml)
				{
					packet.PutByte(true);
					packet.PutString(prop.Xml.ToString());
				}
				else
				{
					packet.PutByte(false);
				}

				packet.PutInt(0);
				packet.PutShort(0);
			}
			else
			{
				packet.PutString(prop.State);
				packet.PutLong(DateTime.Now);

				if (prop.HasXml)
				{
					packet.PutByte(true);
					packet.PutString(prop.Xml.ToString());
				}
				else
				{
					packet.PutByte(false);
				}

				packet.PutFloat(prop.Info.Direction);
			}

			return packet;
		}

		private static Packet AddPropUpdateInfo(this Packet packet, Prop prop)
		{
			packet.PutString(prop.State);
			packet.PutLong(DateTime.Now);

			if (prop.HasXml)
			{
				packet.PutByte(true);
				packet.PutString(prop.Xml.ToString());
			}
			else
			{
				packet.PutByte(false);
			}

			packet.PutFloat(prop.Info.Direction);
			packet.PutShort(0);

			return packet;
		}

		private static Packet AddRegen(this Packet packet, StatRegen regen)
		{
			packet.PutInt(regen.Id);

			// It makes more sense for us to *increase* the hunger, but the
			// client wants to *decrease* the amount of available Stamina.
			packet.PutFloat(regen.Stat != Stat.Hunger ? regen.Change : -regen.Change);

			packet.PutInt(regen.TimeLeft);
			packet.PutInt((int)regen.Stat);
			packet.PutByte(0); // ?
			packet.PutFloat(regen.Max);

			return packet;
		}

		private static Packet AddConditions(this Packet packet, CreatureConditions conditions)
		{
			packet.PutULong((ulong)conditions.A);
			packet.PutULong((ulong)conditions.B);
			packet.PutULong((ulong)conditions.C);
			// [150100] New conditions list
			{
				packet.PutULong((ulong)conditions.D);
			}
			// [180300, NA169 (23.10.2013)] New conditions list?
			{
				packet.PutULong((ulong)conditions.E);
			}

			// List of additional values for the conditions
			var extra = conditions.GetExtraList();
			packet.PutInt(extra.Count);
			foreach (var e in extra)
			{
				packet.PutInt(e.Key);
				packet.PutString(e.Value.ToString());
			}

			// [180100] ? (old: Zero Talent?)
			{
				// This might not be part of the conditions, but we need that
				// long in both cases (5209 and update).
				packet.PutLong(0);
			}

			return packet;
		}

		private static void AddQuest(this Packet packet, Quest quest)
		{
			if (quest.Data == null)
				throw new Exception("AddQuest: Missing quest data for '" + quest.Id.ToString() + "'.");

			packet.PutLong(quest.UniqueId);
			packet.PutByte(0);

			packet.PutLong(quest.QuestItem.EntityId);

			packet.PutByte(2); // 0 = blue icon, 2 = normal, 4 = exploration, 7 = shadow (changes structure slightly)
			// Client values that might make sense:
			// Delivery: 1? (id == 506401?)
			// Event: 1? ((this + 80) == 18?)
			// Homework: 1? ((this + 80) >= 10000?)
			// Exploration: 4|5
			// Escort: 6
			// Shadow: 7|8
			// Bingo: 9
			// GameQuest: 2|4|5
			// GuildQuest: 0 (id >= 110001 < 120000?)
			// PartyQuest: 0 (id >= 100000 < 110000?)

			packet.PutInt(quest.Id); // Range is important for the tabs.
			// 201000~201999 : event
			// 202000~209999 : normal
			// 210000~239999 : goddess
			// 240000~289999 : normal
			// 290000~290599 : alchemist
			// 290600~291999 : normal
			// 292000~292599 : alchemist
			// 292600~292999 : normal
			// 293000~293599 : alchemist
			// 293600~293999 : shakespear (default)
			// 294000~294599 : shakespear (hamlet)
			// 294600~294999 : shakespear (default)
			// 295000~295599 : shakespear (romeo and juliet)
			// 295600~295999 : shakespear (default)
			// 296000~296599 : shakespear (merchant)
			// 296600~296999 : shakespear (default)
			// 297000~297599 : shakespear (macbeth)
			// 297600~______ : normal

			packet.PutString(quest.Data.Name);
			packet.PutString(quest.Data.Description);
			packet.PutString(""); // AdditionalInfo?

			packet.PutInt(1);
			packet.PutInt(quest.QuestItem.Info.Id);
			packet.PutByte(quest.Data.Cancelable); // Doesn't seem to work?
			packet.PutByte(0);
			packet.PutByte(0); // 1 = blue icon
			packet.PutByte(0);

			// [180300, NA166 (18.09.2013)] ?
			{
				packet.PutByte(0);
				packet.PutByte(0);
				packet.PutByte(0);
			}

			packet.PutString(""); // data\gfx\image\gui_temporary_quest.dds
			packet.PutInt(0);     // 4, x y ?
			packet.PutInt(0);
			packet.PutString(""); // <xml soundset="4" npc="GUI_NPCportrait_Lanier"/>
			packet.PutString("QMBEXP:f:1.000000;QMBGLD:f:1.000000;QMSMEXP:f:1.000000;QMSMGLD:f:1.000000;QMAMEXP:f:1.000000;QMAMGLD:f:1.000000;QMBHDCTADD:4:0;QMGNRB:f:1.000000;QMGNRB:f:1.000000;");

			packet.PutInt(0);
			packet.PutInt(0);
			// Alternative, PTJ
			//020 [........00000002] Int    : 2
			//021 [........0000000C] Int    : 12
			//022 [........00000010] Int    : 16
			//023 [........00000015] Int    : 21
			//024 [000039BF89671150] Long   : 63494806770000 // Timestamp

			packet.PutInt(quest.Data.Objectives.Count);
			foreach (var objectiveData in quest.Data.Objectives)
			{
				var objective = objectiveData.Value;
				var progress = quest.GetProgress(objectiveData.Key);

				// Objective
				packet.PutByte((byte)objective.Type);
				packet.PutString(objective.Description);
				packet.PutString(objective.MetaData.ToString());

				// 3  - TARGECHAR:s:shamala;TARGETCOUNT:4:1; - Ask Shamala about collecting transformations
				// 14 - TARGETITEM:4:40183;TARGETCOUNT:4:1; - Break a nearby tree
				// 1  - TGTSID:s:/brownphysisfoxkid/;TARGETCOUNT:4:10;TGTCLS:2:0; - Hunt 10 Young Brown Physis Foxes
				// 9  - TGTSKL:2:23002;TGTLVL:2:1;TARGETCOUNT:4:1; - Combat Mastery rank F reached
				// 19 - TGTCLS:2:3906;TARGETCOUNT:4:1; - Win at least one match in the preliminaries or the finals of the Jousting Tournament.
				// 18 - TGTCLS:2:3502;TARGETCOUNT:4:1; - Read the Author's Notebook.
				// 4  - TARGECHAR:s:duncan;TARGETITEM:4:75473;TARGETCOUNT:4:1; - Deliver the Author's Notebook to Duncan.
				// 15 - TGTLVL:2:15;TARGETCOUNT:4:1; - Reach Lv. 15
				// 2  - TARGETITEM:4:52027;TARGETCOUNT:4:10;QO_FLAG:4:1; - Harvest 10 Bundles of Wheat
				// 22 - TGTSID:s:/ski/start/;TARGETITEM:4:0;EXCNT:4:0;TGITM2:4:0;EXCNT2:4:0;TARGETCOUNT:4:1; - Click on the Start Flag.
				// 47 - TARGETCOUNT:4:1;TGTCLS:4:730205; - Clear the Monkey Mash Mission.
				// 52 - QO_FLAG:b:true;TARGETCOUNT:4:1; - Collect for the Transformation Diary
				// 50 - TARGETRACE:4:9;TARGETCOUNT:4:1; - Transform into a Kiwi.
				// 54 - TARGETRACE:4:9;TARGETCOUNT:4:1; - Collect Frail Green Kiwi perfectly.

				// Type theory:
				// 1  : Kill x of y
				// 2  : Collect x of y
				// 3  : Talk to x
				// 4  : Bring x to y
				// 9  : Reach rank x on skill y
				// 14 : ?
				// 15 : Reach lvl x
				// 18 : Do something with item x ?
				// 19 : Clear something, like jousting or a dungeon?

				// Progress
				packet.PutInt(progress.Count);
				// [180600, NA187 (25.06.2014)] ?
				{
					packet.PutFloat(0);
				}
				packet.PutByte(progress.Done);
				packet.PutByte(progress.Unlocked);

				// Target location
				packet.PutByte(objective.RegionId > 0);
				if (objective.RegionId > 0)
				{
					packet.PutInt(objective.RegionId);
					packet.PutInt(objective.X);
					packet.PutInt(objective.Y);
				}
			}

			packet.PutByte(1);
			packet.PutByte(0);
			packet.PutByte(0);
			packet.PutByte(1);

			// Rewards
			packet.PutByte((byte)quest.Data.Rewards.Count);
			foreach (var reward in quest.Data.Rewards)
			{
				packet.PutByte((byte)reward.Type);
				packet.PutString(reward.ToString());
				packet.PutByte(0); // Group?
				packet.PutByte(1);
				packet.PutByte(1);
			}

			packet.PutByte(0);
		}
	}

	public enum CreaturePacketType : byte
	{
		/// <summary>
		/// 1
		/// </summary>
		Minimal = 1,
		/// <summary>
		/// 2
		/// </summary>
		Private = 2,
		/// <summary>
		/// 5
		/// </summary>
		Public = 5,
	}

	public enum ItemPacketType : byte { Public = 1, Private = 2 }

	public enum StatUpdateType : byte { Private = 3, Public = 4 }
}
