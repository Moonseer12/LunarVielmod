using Stellamod.Content.CommonMaterials;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.MagicCauldron;

public class VanillaCauldronEditions : ModSystem
{
    public override void PostAddRecipes()
    {
        base.PostAddRecipes();
        BrewExtension.RegisterVanillaBrew(ItemID.Aglet, ModContent.ItemType<Mushroom>());
        BrewExtension.RegisterVanillaBrew(ItemID.JellyfishNecklace, ModContent.ItemType<Mushroom>());

        BrewExtension.RegisterVanillaBrew(ItemID.HermesBoots, ModContent.ItemType<Ivythorn>());
        BrewExtension.RegisterVanillaBrew(ItemID.ShinyRedBalloon, ModContent.ItemType<Ivythorn>());
        BrewExtension.RegisterVanillaBrew(ItemID.CloudinaBottle, ModContent.ItemType<Ivythorn>());

        BrewExtension.RegisterVanillaBrew(ItemID.PortableStool, ModContent.ItemType<AlcadizScrap>());
        BrewExtension.RegisterVanillaBrew(ItemID.SunStone, ModContent.ItemType<AlcadizScrap>());

        BrewExtension.RegisterVanillaBrew(ItemID.IceSkates, ModContent.ItemType<WinterbornShard>());
        BrewExtension.RegisterVanillaBrew(ItemID.BlizzardinaBottle, ModContent.ItemType<WinterbornShard>());

        BrewExtension.RegisterVanillaBrew(ItemID.RocketBoots, ModContent.ItemType<MinersGold>());
        BrewExtension.RegisterVanillaBrew(ItemID.ClimbingClaws, ModContent.ItemType<MinersGold>());
        BrewExtension.RegisterVanillaBrew(ItemID.LuckyHorseshoe, ModContent.ItemType<MinersGold>());
        BrewExtension.RegisterVanillaBrew(ItemID.ShoeSpikes, ModContent.ItemType<MinersGold>());

        BrewExtension.RegisterVanillaBrew(ItemID.BandofRegeneration, ModContent.ItemType<TerrorFragments>());
        BrewExtension.RegisterVanillaBrew(ItemID.PhilosophersStone, ModContent.ItemType<TerrorFragments>());
        BrewExtension.RegisterVanillaBrew(ItemID.FleshKnuckles, ModContent.ItemType<TerrorFragments>());
        BrewExtension.RegisterVanillaBrew(ItemID.PutridScent, ModContent.ItemType<TerrorFragments>());
        BrewExtension.RegisterVanillaBrew(ItemID.PanicNecklace, ModContent.ItemType<TerrorFragments>());

        BrewExtension.RegisterVanillaBrew(ItemID.AnkletoftheWind, ModContent.ItemType<GintzlMetal>());
        BrewExtension.RegisterVanillaBrew(ItemID.SandBoots, ModContent.ItemType<GintzlMetal>());
        BrewExtension.RegisterVanillaBrew(ItemID.SharkToothNecklace, ModContent.ItemType<GintzlMetal>());
        BrewExtension.RegisterVanillaBrew(ItemID.SandstorminaBottle, ModContent.ItemType<GintzlMetal>());

        BrewExtension.RegisterVanillaBrew(ItemID.HellfireTreads, ModContent.ItemType<Cinderscrap>());
        BrewExtension.RegisterVanillaBrew(ItemID.LavaCharm, ModContent.ItemType<Cinderscrap>());
        BrewExtension.RegisterVanillaBrew(ItemID.EyeoftheGolem, ModContent.ItemType<Cinderscrap>());
        BrewExtension.RegisterVanillaBrew(ItemID.ObsidianRose, ModContent.ItemType<Cinderscrap>());
        BrewExtension.RegisterVanillaBrew(ItemID.ObsidianSkull, ModContent.ItemType<Cinderscrap>());

        BrewExtension.RegisterVanillaBrew(ItemID.BandofStarpower, ModContent.ItemType<HypnotizedSoul>());

        BrewExtension.RegisterVanillaBrew(ItemID.CelestialMagnet, ModContent.ItemType<ConvulgingMater>());
        BrewExtension.RegisterVanillaBrew(ItemID.RifleScope, ModContent.ItemType<ConvulgingMater>());

        BrewExtension.RegisterVanillaBrew(ItemID.CobaltShield, ModContent.ItemType<PearlescentScrap>());
        BrewExtension.RegisterVanillaBrew(ItemID.StarCloak, ModContent.ItemType<PearlescentScrap>());
        BrewExtension.RegisterVanillaBrew(ItemID.PaladinsShield, ModContent.ItemType<PearlescentScrap>());
        BrewExtension.RegisterVanillaBrew(ItemID.MagicQuiver, ModContent.ItemType<PearlescentScrap>());

        BrewExtension.RegisterVanillaBrew(ItemID.FlowerBoots, ModContent.ItemType<MarshScrap>());
        BrewExtension.RegisterVanillaBrew(ItemID.NaturesGift, ModContent.ItemType<MarshScrap>());
        BrewExtension.RegisterVanillaBrew(ItemID.StaffofRegrowth, ModContent.ItemType<MarshScrap>());

        BrewExtension.RegisterVanillaBrew(ItemID.YoyoBag, ModContent.ItemType<MechanizedSoul>());
        BrewExtension.RegisterVanillaBrew(ItemID.DiscountCard, ModContent.ItemType<MechanizedSoul>());
        BrewExtension.RegisterVanillaBrew(ItemID.LuckyCoin, ModContent.ItemType<MechanizedSoul>());
        BrewExtension.RegisterVanillaBrew(ItemID.Tabi, ModContent.ItemType<MechanizedSoul>());

        BrewExtension.RegisterVanillaBrew(ItemID.RainbowString, ModContent.ItemType<KaleidoscopicInk>());

        BrewExtension.RegisterVanillaBrew(ItemID.AngelWings, ModContent.ItemType<IllurineScale>());
        BrewExtension.RegisterVanillaBrew(ItemID.FrozenWings, ModContent.ItemType<IllurineScale>());

        BrewExtension.RegisterVanillaBrew(ItemID.BlackBelt, ModContent.ItemType<MiracleThread>());
        BrewExtension.RegisterVanillaBrew(ItemID.AnkhCharm, ModContent.ItemType<MiracleThread>());

        BrewExtension.RegisterVanillaBrew(ItemID.NecromanticScroll, ModContent.ItemType<EreshkinCandle>());
        BrewExtension.RegisterVanillaBrew(ItemID.DemonWings, ModContent.ItemType<EreshkinCandle>());

        BrewExtension.RegisterVanillaBrew(ItemID.LeafWings, ModContent.ItemType<RadiantNectar>());
        BrewExtension.RegisterVanillaBrew(ItemID.BeeWings, ModContent.ItemType<RadiantNectar>());

        BrewExtension.RegisterVanillaBrew(ItemID.TatteredFairyWings, ModContent.ItemType<AlcaricMush>());
        BrewExtension.RegisterVanillaBrew(ItemID.GhostWings, ModContent.ItemType<AlcaricMush>());

        BrewExtension.RegisterVanillaBrew(ItemID.FishronWings, ModContent.ItemType<FallenEyes>());
        BrewExtension.RegisterVanillaBrew(ItemID.MothronWings, ModContent.ItemType<FallenEyes>());
        BrewExtension.RegisterVanillaBrew(ItemID.BoneWings, ModContent.ItemType<FallenEyes>());

        BrewExtension.RegisterVanillaBrew(ItemID.RodofDiscord, ModContent.ItemType<MothlightWing>());

        BrewExtension.RegisterVanillaBrew(ItemID.AmphibianBoots, ModContent.ItemType<MusicalHarmonise>());
        BrewExtension.RegisterVanillaBrew(ItemID.FrogLeg, ModContent.ItemType<MusicalHarmonise>());
        BrewExtension.RegisterVanillaBrew(ItemID.Flipper, ModContent.ItemType<MusicalHarmonise>());
        BrewExtension.RegisterVanillaBrew(ItemID.DivingGear, ModContent.ItemType<MusicalHarmonise>());
        BrewExtension.RegisterVanillaBrew(ItemID.FloatingTube, ModContent.ItemType<MusicalHarmonise>());
        BrewExtension.RegisterVanillaBrew(ItemID.WaterWalkingBoots, ModContent.ItemType<MusicalHarmonise>());
        BrewExtension.RegisterVanillaBrew(ItemID.TsunamiInABottle, ModContent.ItemType<MusicalHarmonise>());
        BrewExtension.RegisterVanillaBrew(ItemID.BalloonPufferfish, ModContent.ItemType<MusicalHarmonise>());
        BrewExtension.RegisterVanillaBrew(ItemID.SailfishBoots, ModContent.ItemType<MusicalHarmonise>());
    }
}