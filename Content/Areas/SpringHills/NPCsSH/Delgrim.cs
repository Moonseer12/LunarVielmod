using Stellamod.Common.CellConverterSystem;
using Stellamod.Common.QuestSystem;
using Stellamod.Content.TODO.GunSwapping;
using Stellamod.Content.Quests.DelgrimQuest;
using Stellamod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH
{
    public class Delgrim : VeilTownNPC
    {
        public const string ShopName = "Shop";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 11;
        }

        public override void SetDefaults()
        {
            // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 92;
            NPC.height = 84;
            NPC.aiStyle = -1;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 1;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamage = true;
            HasTownDialogue = true;
        }


        //This prevents the NPC from despawning
        public override bool CheckActive()
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.20f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Magical Engineer Delgrim"
            };
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
            .Add<GunHolster>()
            .Add<Pulsing>()
            ;
            npcShop.Register(); // Name of this shop tab		
        }

        public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
            //Set buttons
            buttons.Add(new Tuple<string, Action>("Shop", OpenShop));
            buttons.Add(new Tuple<string, Action>("CellConverter", OpenCellConverter));

            //Delgrim Portrait
            text = "TestDialogue";
            portrait = "DelgrimPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;
        }

        private void OpenCellConverter()
        {
            Main.CloseNPCChatOrSign();
            Main.playerInventory = true;
            CellConverterUISystem uiSystem = ModContent.GetInstance<CellConverterUISystem>();
            uiSystem.CellConverterPos = NPC.Center;
            uiSystem.OpenUI();

        }
        public override void SetQuestLine(List<Quest> quests)
        {
            base.SetQuestLine(quests);
            quests.Add(ModContent.GetInstance<MysteriousPlacesI>());
        }
    }
}