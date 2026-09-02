using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.IrradiaNHavoc
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    //[AutoloadHead]

    public class IrradiaIdle : ModNPC
    {
        public int NumberOfTimesTalkedTo = 0;
        public const string ShopName = "Shop";
        public const string ShopName2 = "New Shop";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 60;
        }

        // Current state


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
            // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 23;
            NPC.height = 48;
            NPC.aiStyle = -1;
            NPC.damage = 9000;
            NPC.defense = 69;
            NPC.lifeMax = 200000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamage = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
        }


        //This prevents the NPC from despawning
        public override bool CheckActive()
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.04f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }




        public override bool CanChat()
        {
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A suspicious person at the bottom of the Govheil Castle")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "???", "2"))
            });
        }

        // The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
        // Returning false will allow you to manually draw your NPC
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // This code slowly rotates the NPC in the bestiary
            // (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
            if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
            {


                // Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
                NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
                NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            }

            return true;

        }
        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            int partyGirl = NPC.FindFirstNPC(NPCID.Steampunker);

            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add("...");
            chat.Add(LangText.Chat(this, "Basic1"));
            chat.Add(LangText.Chat(this, "Basic2"));
            chat.Add(LangText.Chat(this, "Basic3"), 1.0);
            chat.Add(LangText.Chat(this, "Basic4"), 1.0);


            NumberOfTimesTalkedTo++;
            if (NumberOfTimesTalkedTo >= 10)
            {
                //This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
                chat.Add("...");
            }

            return chat; // chat is implicitly cast to a string.
        }






        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "???",
                "???"

            };
        }




        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
            button = LangText.Chat(this, "Button");

        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {


            //-----------------------------------------------------------------------------------------------

            if (firstButton)
            {

                Player player = Main.LocalPlayer;
                WeightedRandom<string> chat = new WeightedRandom<string>();




                // Reforge/Anvil sound







            }


        }


        public void ResetTimers()
        {
            timer = 0;
            frameCounter = 0;
            frameTick = 0;
        }











        public override void AI()
        {
            timer++;
            NPC.CheckActive();
            NPC.spriteDirection = NPC.direction;



            if (NPC.AnyNPCs(ModContent.NPCType<Irradia.Irradia>()))
            {

                NPC.Kill();
            }



        }












        //	else if (Main.moonPhase < 4) {
        // shop.item[nextSlot++].SetDefaults(ItemType<ExampleGun>());
        //		shop.item[nextSlot].SetDefaults(ItemType<ExampleBullet>());
        //	}
        //	else if (Main.moonPhase < 6) {
        // shop.item[nextSlot++].SetDefaults(ItemType<ExampleStaff>());
        // 	}
        //
        // 	// todo: Here is an example of how your npc can sell items from other mods.
        // 	// var modSummonersAssociation = ModLoader.TryGetMod("SummonersAssociation");
        // 	// if (ModLoader.TryGetMod("SummonersAssociation", out Mod modSummonersAssociation)) {
        // 	// 	shop.item[nextSlot].SetDefaults(modSummonersAssociation.ItemType("BloodTalisman"));
        // 	// 	nextSlot++;
        // 	// }
        //
        // 	// if (!Main.LocalPlayer.GetModPlayer<ExamplePlayer>().examplePersonGiftReceived && GetInstance<ExampleConfigServer>().ExamplePersonFreeGiftList != null) {
        // 	// 	foreach (var item in GetInstance<ExampleConfigServer>().ExamplePersonFreeGiftList) {
        // 	// 		if (Item.IsUnloaded) continue;
        // 	// 		shop.item[nextSlot].SetDefaults(Item.Type);
        // 	// 		shop.item[nextSlot].shopCustomPrice = 0;
        // 	// 		shop.item[nextSlot].GetGlobalItem<ExampleInstancedGlobalItem>().examplePersonFreeGift = true;
        // 	// 		nextSlot++;
        // 	// 		//TODO: Have tModLoader handle index issues.
        // 	// 	}
        // 	// }
        // }







    }





}