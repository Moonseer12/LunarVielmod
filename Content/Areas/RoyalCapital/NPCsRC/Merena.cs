using Stellamod.Content.TODO.Weapons;
using Stellamod.Core;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.NPCsRC
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    //[AutoloadHead]
    public class Merena : VeilTownNPC
    {
        public const string ShopName = "Shop";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 8; 
        }

        // Current frame
        public int frameCounter;
        // Current frame's progress
        public int frameTick;
        // Current state's timer
        public float timer;

        // AI counter
        public int counter;
        public override void SetDefaults()
        {
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 62;
            NPC.height = 90;
            NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 200;
            NPC.npcSlots = 0;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamageFromHostiles = true;
            SpawnAtPoint = true;
        }

        public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {
            spawner.structureToSpawnIn = "Struct/Alcad/RoyalCapital3";
            spawner.spawnTileOffset = new Point(506, -13);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.16f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }


        //This prevents the NPC from despawning
        public override bool CheckActive()
        {
            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Merena the Sorcerer"
            };
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
            .Add(new Item(ModContent.ItemType<BurnedCarianTome>()));
            npcShop.Register(); // Name of this shop tab		
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
        }
    }
}