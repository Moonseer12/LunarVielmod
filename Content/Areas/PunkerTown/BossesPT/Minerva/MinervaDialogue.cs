
using Stellamod.Core.DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.Minerva
{
    public class MinervaStartDialogue : BaseDialogue
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CloseOnComplete = true;
        }

        public override int GetLength()
        {
            return 4;
        }

        public override void OnComplete()
        {
            base.OnComplete();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int index = NPC.FindFirstNPC(ModContent.NPCType<MinervaIdle>());
                if (index == -1)
                    return;
                Vector2 position = Main.npc[index].position;
                int x = (int)position.X;
                int y = (int)position.Y;
                int npcID = NPC.NewNPC(new EntitySource_TileBreak(x, y), x, y, ModContent.NPCType<Minerva>());
                Main.npc[npcID].netUpdate = true;
            }
            else
            {
                int index = NPC.FindFirstNPC(ModContent.NPCType<MinervaIdle>());
                if (index == -1)
                    return;

                Vector2 position = Main.npc[index].position;
                MultiplayerHelper.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI,
                    ModContent.NPCType<Minerva>(), (int)position.X, (int)position.Y);
            }
        }
    }
}
