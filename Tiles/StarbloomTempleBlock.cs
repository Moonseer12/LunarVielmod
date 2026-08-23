using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Tiles
{
    public class StarbloomTempleBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.IceBlock][Type] = true;
            Main.tileMerge[TileID.SnowBlock][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(60, 50, 78));
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
        }
    }
}