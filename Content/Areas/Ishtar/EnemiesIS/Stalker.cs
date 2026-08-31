using Stellamod.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.EnemiesIS
{
    public class Stalker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 16;
            this.AddToIshtar();
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 14;
            NPC.lifeMax = 450;
            NPC.damage = 90;
            AIType = NPCID.BoneLee;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            NPC.HitSound = SoundID.NPCHit29;
            NPC.DeathSound = SoundID.NPCDeath32;
            SpawnModBiomes = [ModContent.GetInstance<IshtarBiome>().Type];
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.6f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override int SpawnNPC(int tileX, int tileY)
		{
            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X + 30, (int)NPC.Center.Y, NPC.type);
            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X - 30, (int)NPC.Center.Y, NPC.type);
			return NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X - 45, (int)NPC.Center.Y, NPC.type);
		}
    }
}
