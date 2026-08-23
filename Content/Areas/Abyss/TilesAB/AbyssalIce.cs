using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Abyss.TilesAB
{
    public class AbyssalIce : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.IceBlock][Type] = true;
            Main.tileMerge[TileID.SnowBlock][Type] = true;
            Main.tileMerge[ModContent.TileType<AbyssalDirt>()][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(57, 55, 172));
        }
    }
}