using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class AbyssalCoarseDirtItem : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<AbyssalCoarseDirt>());
    }
}

public class AbyssalCoarseDirt : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        Main.tileLighted[Type] = true;
        Main.tileMerge[TileID.IceBlock][Type] = true;
        Main.tileMerge[TileID.SnowBlock][Type] = true;
        Main.tileMerge[ModContent.TileType<AbyssalIce>()][Type] = true;
        Main.tileBlendAll[Type] = true;
        RegisterItemDrop(ModContent.ItemType<AbyssalCoarseDirtItem>());
        AddMapEntry(new Color(57, 55, 172));
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, ref r, ref g, ref b);
    }

    public override void RandomUpdate(int i, int j)
    {
        TileHelper.GrowVine(i, j, ModContent.TileType<AbyssalVines>());
        TileHelper.GrowVine(i, j, ModContent.TileType<AbyssalVines2>());
    }
}
public class AbyssalDirtItem : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToPlaceableTile(ModContent.TileType<AbyssalDirt>());
    }
}

public class AbyssalDirt : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMerge[Type][Type] = true;
        Main.tileBlockLight[Type] = true;
        Main.tileLargeFrames[Type] = 2;
        Main.tileLighted[Type] = true;
        Main.tileMerge[TileID.IceBlock][Type] = true;
        Main.tileMerge[TileID.SnowBlock][Type] = true;
        Main.tileMerge[ModContent.TileType<AbyssalIce>()][Type] = true;
        Main.tileBlendAll[Type] = true;
        AddMapEntry(new Color(57, 55, 172));
    }
    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        base.ModifyLight(i, j, ref r, ref g, ref b);
    }

    public override void RandomUpdate(int i, int j)
    {
        TileHelper.GrowVine(i, j, ModContent.TileType<AbyssalVines>());
        TileHelper.GrowVine(i, j, ModContent.TileType<AbyssalVines2>());
    }
}