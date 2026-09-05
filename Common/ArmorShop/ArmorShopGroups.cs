using Stellamod.Content.Areas.Tundra.Abyss.ArmorAB;
using Stellamod.Content.Areas.Desert.ArmorCL;
using Stellamod.Content.Areas.Hallowrooms.ArmorHR;
using Stellamod.Content.Areas.Ishtar.ArmorIS;
using Stellamod.Content.Areas.Jungle.ArmorJN;
using Stellamod.Content.Areas.Junkyard.ArmorJY;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.ArmorMT;
using Stellamod.Content.Areas.PunkerTown.ArmorPT;
using Stellamod.Content.Areas.Tundra.Snow.ArmorSN;
using Stellamod.Content.Areas.SpringHills.ArmorSH;
using Stellamod.Content.Areas.Terror.ArmorTR;
using Stellamod.Content.Areas.TheFalling.ArmorTF;
using Stellamod.Content.Areas.Underground.ArmorUG;
using Stellamod.Content.Areas.Underground.TilesUG;
using Stellamod.Content.Areas.WaterSide.ArmorWS;
using Stellamod.Content.Areas.WondrousDarkspace.ArmorWD;
using Stellamod.Content.Armors.Elegant;
using Stellamod.Content.Armors.Jianxin;
using Stellamod.Content.CommonMaterials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.ArmorShop
{
    public class ArmorShopGroups : ModSystem
    {
        public List<ArmorShopSet> Armors;

        public override void PostSetupContent()
        {
            base.PostSetupContent();
            Armors = new();

            ArmorShopSet ivythornSet = new();
            ivythornSet.AddHead(ModContent.ItemType<ForestCoreHead>());
            ivythornSet.AddBody(ModContent.ItemType<ForestCoreBody>());
            ivythornSet.AddLegs(ModContent.ItemType<ForestCoreLegs>());
            ivythornSet.SetMaterial(ModContent.ItemType<Ivythorn>());
            ivythornSet.Register();

            ArmorShopSet leth = new();
            leth.AddHead(ModContent.ItemType<LeatherHead>());
            leth.AddBody(ModContent.ItemType<LeatherBody>());
            leth.AddLegs(ModContent.ItemType<LeatherLegs>());
            leth.SetMaterial(ItemID.Leather);
            leth.Register();

            ArmorShopSet winterbornSet = new();
            winterbornSet.AddHead(ModContent.ItemType<WinterbornHead>());
            winterbornSet.AddBody(ModContent.ItemType<WinterbornBody>());
            winterbornSet.AddLegs(ModContent.ItemType<WinterbornLegs>());
            winterbornSet.SetMaterial(ModContent.ItemType<WinterbornShard>());
            winterbornSet.Register();

            ArmorShopSet celestiaMoonSet = new();
            celestiaMoonSet.AddHead(ModContent.ItemType<CelestiaMoonHelmet>());
            celestiaMoonSet.AddBody(ModContent.ItemType<CelestiaMoonBreastplate>());
            celestiaMoonSet.AddLegs(ModContent.ItemType<CelestiaMoonLegs>());
            celestiaMoonSet.SetMaterial(ModContent.ItemType<GlisteningOre>());
            celestiaMoonSet.Register();

            ArmorShopSet SW = new();
            SW.AddHead(ModContent.ItemType<ShadeWraithHead>());
            SW.AddBody(ModContent.ItemType<ShadeWraithBody>());
            SW.AddLegs(ModContent.ItemType<ShadeWraithLegs>());
            SW.SetMaterial(ItemID.GraniteBlock);
            SW.Register();

            ArmorShopSet astr = new ArmorShopSet();
            astr.AddHead(ModContent.ItemType<AstrasilkHead>());
            astr.AddBody(ModContent.ItemType<AstrasilkBody>());
            astr.AddLegs(ModContent.ItemType<AstrasilkLegs>());
            astr.SetMaterial(ItemID.FallenStar);
            astr.Register();

            ArmorShopSet GintzeSet = new();
            GintzeSet.AddHead(ModContent.ItemType<HeavyMetalHead>());
            GintzeSet.AddBody(ModContent.ItemType<HeavyMetalBody>());
            GintzeSet.AddLegs(ModContent.ItemType<HeavyMetalLegs>());
            GintzeSet.SetMaterial(ModContent.ItemType<GintzlMetal>());
            GintzeSet.Register();

            ArmorShopSet terr = new();
            terr.AddHead(ModContent.ItemType<TerricHead>());
            terr.AddBody(ModContent.ItemType<TerricBody>());
            terr.AddLegs(ModContent.ItemType<TerricLegs>());
            terr.SetMaterial(ModContent.ItemType<TerrorFragments>());
            terr.Register();

            ArmorShopSet Daedia = new();
            Daedia.AddHead(ModContent.ItemType<DaediaMask>());
            Daedia.AddBody(ModContent.ItemType<DaediaBreastplate>());
            Daedia.AddLegs(ModContent.ItemType<DaediaThighs>());
            Daedia.SetMaterial(ModContent.ItemType<HypnotizedSoul>());
            Daedia.Register();

            ArmorShopSet staff = new();
            staff.AddHead(ModContent.ItemType<StaffigyHat>());
            staff.AddBody(ModContent.ItemType<StaffigyRobe>());
            staff.AddLegs(ModContent.ItemType<StaffigyPants>());
            staff.SetMaterial(ModContent.ItemType<HypnotizedSoul>());
            staff.Register();

            ArmorShopSet Vext = new();
            Vext.AddHead(ModContent.ItemType<VextinMask>());
            Vext.AddBody(ModContent.ItemType<VextinRobe>());
            Vext.AddLegs(ModContent.ItemType<VextinBoots>());
            Vext.SetMaterial(ItemID.AntlionMandible);
            Vext.Register();

            ArmorShopSet hunt = new();
            hunt.AddHead(ModContent.ItemType<HuntrianHelmet>());
            hunt.AddBody(ModContent.ItemType<HuntrianChestplate>());
            hunt.AddLegs(ModContent.ItemType<HuntrianBoots>());
            hunt.SetMaterial(ItemID.Stinger);
            hunt.Register();

            ArmorShopSet fishy = new();
            fishy.AddHead(ModContent.ItemType<FishyHead>());
            fishy.AddBody(ModContent.ItemType<FishyBody>());
            fishy.AddLegs(ModContent.ItemType<FishyLegs>());
            fishy.SetMaterial(ModContent.ItemType<MusicalHarmonise>());
            fishy.Register();

            ArmorShopSet Luvo = new();
            Luvo.AddHead(ModContent.ItemType<LunarianVoidHead>());
            Luvo.AddBody(ModContent.ItemType<LunarianVoidBody>());
            Luvo.AddLegs(ModContent.ItemType<LunarianVoidLegs>());
            Luvo.SetMaterial(ModContent.ItemType<ConvulgingMater>());
            Luvo.Register();

            ArmorShopSet Verl = new();
            Verl.AddHead(ModContent.ItemType<VerlMask>());
            Verl.AddBody(ModContent.ItemType<VerlBreastplate>());
            Verl.AddLegs(ModContent.ItemType<VerlLeggings>());
            Verl.SetMaterial(ModContent.ItemType<PearlescentScrap>());
            Verl.Register();

            ArmorShopSet Ele = new();
            Ele.AddHead(ModContent.ItemType<ElagentHead>());
            Ele.AddBody(ModContent.ItemType<ElagentBody>());
            Ele.AddLegs(ModContent.ItemType<ElagentLegs>());
            Ele.SetMaterial(ItemID.Feather);
            Ele.Register();

            ArmorShopSet Vir = new();
            Vir.AddHead(ModContent.ItemType<VirulentHelm>());
            Vir.AddBody(ModContent.ItemType<VirulentArmor>());
            Vir.AddLegs(ModContent.ItemType<VirulentLegs>());
            Vir.SetMaterial(ModContent.ItemType<MechanizedSoul>());
            Vir.Register();

            ArmorShopSet Paint = new();
            Paint.AddHead(ModContent.ItemType<ArtisanMask>());
            Paint.AddBody(ModContent.ItemType<ArtisanBreastplate>());
            Paint.AddLegs(ModContent.ItemType<ArtisanThighs>());
            Paint.SetMaterial(ModContent.ItemType<KaleidoscopicInk>());
            Paint.Register();

            ArmorShopSet SCP = new();
            SCP.AddHead(ModContent.ItemType<ScrappyHead>());
            SCP.AddBody(ModContent.ItemType<ScrappyBody>());
            SCP.AddLegs(ModContent.ItemType<ScrappyLegs>());
            SCP.SetMaterial(ModContent.ItemType<MechanizedSoul>());
            SCP.Register();

            ArmorShopSet Gov1 = new();
            Gov1.AddHead(ModContent.ItemType<GovheilHelmet>());
            Gov1.AddBody(ModContent.ItemType<GovheilChainplate>());
            Gov1.AddLegs(ModContent.ItemType<GovheilThighs>());
            Gov1.SetMaterial(ModContent.ItemType<MarshScrap>());
            Gov1.Register();

            ArmorShopSet Gov2 = new();
            Gov2.AddHead(ModContent.ItemType<GovheilMask>());
            Gov2.AddBody(ModContent.ItemType<GovheilBreastplate>());
            Gov2.AddLegs(ModContent.ItemType<GovheilQueenThighs>());
            Gov2.SetMaterial(ModContent.ItemType<MarshScrap>());
            Gov2.Register();

            ArmorShopSet miracle = new();
            miracle.AddHead(ModContent.ItemType<MiracleHead>());
            miracle.AddBody(ModContent.ItemType<MiracleBody>());
            miracle.SetMaterial(ModContent.ItemType<MiracleThread>());
            miracle.Register();

            ArmorShopSet silk = new();
            silk.AddHead(ModContent.ItemType<CandlelightHood>());
            silk.AddBody(ModContent.ItemType<CandlelightBody>());
            silk.AddLegs(ModContent.ItemType<CandlelightLegs>());
            silk.SetMaterial(ModContent.ItemType<EreshkinCandle>());
            silk.Register();
            

            ArmorShopSet sanc = new();
            sanc.AddHead(ModContent.ItemType<SanctorousHead>());
            sanc.AddBody(ModContent.ItemType<SanctorousBody>());
            sanc.AddLegs(ModContent.ItemType<SanctorousLegs>());
            sanc.SetMaterial(ModContent.ItemType<FallenEyes>());
            sanc.Register();

            ArmorShopSet JianxinSet = new();
            JianxinSet.AddHead(ModContent.ItemType<JianxinMask>());
            JianxinSet.AddBody(ModContent.ItemType<JianxinCoat>());
            JianxinSet.AddLegs(ModContent.ItemType<JianxinPants>());
            JianxinSet.SetMaterial(ItemID.LunarBar);
            JianxinSet.Register();
        }

        public ArmorShopSet FindSet(Item item)
        {
            foreach (var armor in Armors)
            {
                if (armor.IsInSet(item))
                    return armor;
            }
            return null;
        }

        public void AddSet(ArmorShopSet armorShopSet)
        {
            Armors.Add(armorShopSet);
        }
    }
}
