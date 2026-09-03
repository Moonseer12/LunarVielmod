using Stellamod.Common.QuestSystem;
using Stellamod.Content.Quests.ZuiQuest;
using Stellamod.Content.Vanity.Nyxia;
using Stellamod.Content.Vanity.Solarian;
using Stellamod.Core;
using Stellamod.Content.Areas.Jungle.BossesJN.Zui;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH
{
    public class Zui : VeilTownNPC
    {
        public const string ShopName = "Shop";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 4;
        }

        // Current frame
        public int frameCounter;
        // Current frame's progress
        public int frameTick;
        // Current state's timer
        public float timer;

        // AI counter
        public int counter;
        public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {
            spawner.structureToSpawnIn = "Structures/WitchTown";
            spawner.spawnTileOffset = new Point(190, -20 - 38);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 54;
            NPC.height = 130;
            NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 2000;
            NPC.npcSlots = 0;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            SpawnAtPoint = true;
            HasTownDialogue = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.07f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        //This prevents the NPC from despawning
        public override bool CheckActive()
        {
            return false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A traveller of the lands who may hold great power")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Zui the Traveller", "2"))
            });
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Zui The Traveller",
            };
        }

        public override void AI()
        {
            timer++;
            NPC.CheckActive();
            NPC.spriteDirection = NPC.direction;
            if (NPC.AnyNPCs(ModContent.NPCType<ZuiTheTraveller>()))
            {

                NPC.Kill();
            }
        }

        public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
            //Set buttons
            buttons.Add(new Tuple<string, Action>("Talk", Talk));
            buttons.Add(new Tuple<string, Action>("Shop", OpenShop));



            portrait = "ZuiPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "ZuiOpenDialogue1";
        }

        public override void IdleChat(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
        {
            base.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            portrait = "ZuiPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "ZuiIdleChat1";


        }

        public override void SetQuestLine(List<Quest> quests)
        {
            base.SetQuestLine(quests);
            quests.Add(ModContent.GetInstance<CraftAtCauldron>());
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
            .Add<NyxiaHat>()
            .Add<NyxiaRobe>()
            .Add<NyxiaThighs>()
            .Add<SolarianHat>()
            .Add<SolarianChestplate>()
            .Add<SolarianPants>()
            ;
            npcShop.Register(); // Name of this shop tab		
        }


    }
}