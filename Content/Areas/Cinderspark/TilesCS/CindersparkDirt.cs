using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.TilesCS
{
    public class CindersparkDirt : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(100, 25, 40));
        }

        public override void RandomUpdate(int i, int j)
        {
            TileHelper.GrowVine(i, j, ModContent.TileType<CindersparkVines>());
        }
    }
}