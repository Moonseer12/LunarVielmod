using Stellamod.Content.Bar.Drinks;
using Stellamod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.NPCsSH;

public class Bulbtrifier : VeilTownNPC
{
    public const string ShopName = "Shop";
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        // Sets NPC to be a Town NPC
        breathe = true;
        NPC.friendly = true; // NPC Will not attack player
        NPC.width = 38;
        NPC.height = 38;
        NPC.aiStyle = -1;
        NPC.damage = 90;
        NPC.defense = 42;
        NPC.lifeMax = 200;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        NPC.dontTakeDamageFromHostiles = true;
        HasTownDialogue = true;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    { // What the chat buttons are when you open up the chat UI
        button2 = Language.GetTextValue("LegacyInterface.28");
        button = LangText.Chat(this, "Button");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
        if (!firstButton)
        {
            shop = ShopName;
        }
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter += 0.50f;
        NPC.frameCounter %= Main.npcFrameCount[NPC.type];
        int frame = (int)NPC.frameCounter;
        NPC.frame.Y = frame * frameHeight;
    }

    public override bool CheckActive()
    {
        return false;
    }

    public override List<string> SetNPCNameList()
    {
        return new List<string>() {
            "Bulbtrifier",
        };
    }

    public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
    {
        base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
        //Set buttons
        buttons.Add(new Tuple<string, Action>("Talk", Talk));
        buttons.Add(new Tuple<string, Action>("Shop", OpenShop));

        portrait = "BulbtrifierPortrait";
        timeBetweenTexts = 0.015f;
        talkingSound = SoundID.Item1;

        //This pulls from the new Dialogue localization
        text = "ZuiOpenDialogue1";
    }

    public override void Talk()
    {
        base.Talk();
        OpenTalkOptions(
            ModContent.GetInstance<BulbtrifierHiDialogue>(),
            ModContent.GetInstance<BulbtrifierWhoDialogue>(),
            ModContent.GetInstance<BulbtrifierHowMuchDialogue>());
    }

    public override void IdleChat(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
    {
        base.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
        portrait = "BulbtrifierPortrait";
        timeBetweenTexts = 0.015f;
        talkingSound = SoundID.Item1;

        //This pulls from the new Dialogue localization
        text = "ZuiIdleChat1";
    }

    public override void ModifyActiveShop(string shopName, Item[] items)
    {
        base.ModifyActiveShop(shopName, items);
        int index = 0;
        for(int i = 0; i < items.Length; i++)
        {
            items[i] = new Item();
            items[i].TurnToAir();
        }

        foreach (Item item in DrinkShopSystem.items)
        {
            items[index++] = item;
        }
    }

    public override void AddShops()
    {
        var npcShop = new NPCShop(Type, ShopName);
        npcShop.Register(); // Name of this shop t
    }
}