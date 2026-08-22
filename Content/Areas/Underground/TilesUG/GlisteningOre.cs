using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.TilesUG;

public class GlisteningOre : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(ModContent.TileType<GlisteningOreTile>());
    }
}

public class GlisteningOreTile : ModTile
{
    public override void SetStaticDefaults()
    {
        TileID.Sets.Ore[Type] = true;
        Main.tileShine2[Type] = true;
        Main.tileShine[Type] = 300;
        Main.tileMergeDirt[Type] = true;
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        LocalizedText name = CreateMapEntryName();
        AddMapEntry(new Color(0, 200, 187), name);
        DustType = DustID.CoralTorch;
        RegisterItemDrop(ModContent.ItemType<GlisteningOre>());
        HitSound = SoundID.DD2_CrystalCartImpact;
        MineResist = 1f;
    }
    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }
}