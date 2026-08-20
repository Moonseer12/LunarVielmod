using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Helpers
{
    public enum DownedBossFlag : byte
    {
        Woodland_Ravager = 0,
        Minerva = 1,
        Jack = 2,
        Daedus = 3,
        Verlian_Singularity = 4,
        Skullrunner = 5,
        Commander_Gintzia = 6,
        EliteCommander = 7,
        Gustbeak = 8,
        StarBomber = 9,
        Bishinine = 10,
        Jiitas = 11,
        SanguineSingularity = 12,
        PunkerPrime = 13,
        CrumblingTowerOfIlluria = 14,
        StoneGolem = 15,
        Steamroller = 16,
        DescendingTwins=17,
        Verlia=18,
        Celestia=19,
        Cariya=20,
        KingJellyfish=21,
        LeviathanEel=22,
        VerliaPrison=23,
        RoyalFox=24,
        Gothivia=25,
        E=26,
        BunnyStorm=27
    }

    public class Flawless : ModBuff
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            if (NPC.AnyDanger())
                return;
            player.DelBuff(buffIndex);
        }
    }

    public class DownedBossRewardPlayer : ModPlayer
    {
        public bool[] claimedRegularRewards = new bool[64];
        public bool[] claimedMasterRewards = new bool[64];
        public bool[] claimedNoHit = new bool[64];
        public bool[] hasNoHit = new bool[64];
        private void InitializeIfNeeded()
        {
            claimedRegularRewards ??= new bool[64];
            claimedMasterRewards ??= new bool[64];
            claimedNoHit ??= new bool[64];
            hasNoHit ??= new bool[64];

            if (hasNoHit.Length < 64)
                hasNoHit = new bool[64];
        }
        public void ResetFlags()
        {
            InitializeIfNeeded();
            for (int i = 0; i < claimedNoHit.Length; i++)
            {
                claimedMasterRewards[i] = false;
                claimedNoHit[i] = false;
                claimedRegularRewards[i] = false;
                hasNoHit[i] = false;
            }
        }


        public override void PostHurt(Player.HurtInfo info)
        {
            base.PostHurt(info);
            Player.ClearBuff(ModContent.BuffType<Flawless>());
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["claimedRegularRewards"] = claimedRegularRewards;
            tag["claimedMasterRewards"] = claimedMasterRewards;
            tag["claimedNoHit"] = claimedNoHit;
            tag["hasNoHit"] = hasNoHit;

        }


        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            claimedRegularRewards = tag.Get<bool[]>("claimedRegularRewards");
            claimedMasterRewards = tag.Get<bool[]>("claimedMasterRewards");
            claimedNoHit = tag.Get<bool[]>("claimedNoHit");
            hasNoHit = tag.Get<bool[]>("hasNoHit");

            InitializeIfNeeded();

        }

        public static void HandleBossDownedMessage(BinaryReader reader, int whoAmI)
        {
            int flag = reader.ReadInt32();
            DisplayNoHit(flag);
        }

        public static void DisplayNoHit(int flag)
        {
            DownedBossRewardPlayer rwardPlayer = Main.LocalPlayer.GetModPlayer<DownedBossRewardPlayer>();
            if (rwardPlayer.Player.HasBuff(ModContent.BuffType<Flawless>()))
            {
                rwardPlayer.hasNoHit[flag] = true;
                string text = LangText.Common("NoHit");
                int c = CombatText.NewText(rwardPlayer.Player.getRect(), Color.White, text, true);
                Main.combatText[c].lifeTime *= 3;
                rwardPlayer.Player.ClearBuff(ModContent.BuffType<Flawless>());
            }
        }
    }

    public class DownedBossTracker : ModSystem
    {
        public static bool[] downedBossFlags = new bool[64];
        public static int TotalBossCount => 52;
        public static int MaxPossiblePoints => 50;
        public static int DownedBossCount
        {
            get
            {
                int count = 0;
                for(int i = 0; i < downedBossFlags.Length; i++)
                {
                    if (downedBossFlags[i])
                    {
                        count++;
                    }
                }
                return count;
            }
        }
        public static void ResetFlags()
        {
            for (int i = 0; i < downedBossFlags.Length; i++)
            {
                downedBossFlags[i] = false;
            }
        }

        public override void ClearWorld()
        {
            base.ClearWorld();
            ResetFlags();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["downedBossFlags"] = downedBossFlags;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            downedBossFlags = tag.Get<bool[]>("downedBossFlags");
        }

        public static void AllBosses()
        {
            for(int i = 0; i < TotalBossCount; i++)
            {
                downedBossFlags[i] = true;
            }
        }
        public static bool IsDowned(DownedBossFlag flag)
        {
            return IsDowned((int)flag);
        }
        public static bool IsNoHit(DownedBossFlag flag)
        {
            return Main.LocalPlayer.GetModPlayer<DownedBossRewardPlayer>().hasNoHit[(int)flag];
        }

        public static bool IsDowned(int id)
        {
            return downedBossFlags[id];
        }
        public static bool IsNoHit(int id)
        {
            return downedBossFlags[id];
        }
        public static void ClearFlag(DownedBossFlag flag)
        {
            ClearFlag((int)flag);
        }

        public static void ClearFlag(int id)
        {
            NPC.SetEventFlagCleared(ref downedBossFlags[id], -1);
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                int clientToIgnore = Main.LocalPlayer.whoAmI;
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(),
                    (byte)MessageType.BossDowned, id).Send(ignoreClient: clientToIgnore);
            }
            else
            {
                DownedBossRewardPlayer.DisplayNoHit(id);
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            int numBytes = downedBossFlags.Length / 8;
            int j = 0;
            for (int i = 0; i < numBytes; i++)
            {
                BitsByte b = new BitsByte
                {
                    [0] = downedBossFlags[j],
                    [1] = downedBossFlags[j + 1],
                    [2] = downedBossFlags[j + 2],
                    [3] = downedBossFlags[j + 3],
                    [4] = downedBossFlags[j + 4],
                    [5] = downedBossFlags[j + 5],
                    [6] = downedBossFlags[j + 6],
                    [7] = downedBossFlags[j + 7]
                };
                writer.Write(b);
                j += 8;
            }
        }
        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            int numBytes = downedBossFlags.Length / 8;
            int j = 0;
            for (int i = 0; i < numBytes; i++)
            {
                BitsByte flags = reader.ReadByte();
                downedBossFlags[j] = flags[0];
                downedBossFlags[j + 1] = flags[1];
                downedBossFlags[j + 2] = flags[2];
                downedBossFlags[j + 3] = flags[3];
                downedBossFlags[j + 4] = flags[4];
                downedBossFlags[j + 5] = flags[5];
                downedBossFlags[j + 6] = flags[6];
                downedBossFlags[j + 7] = flags[7];
                j += 8;
            }
        }
    }
}