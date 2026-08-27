using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.TilesUG
{
    public class RobedCatastoneBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(120, 125, 37));
        }
    }

    public class RobedCatastone : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RobedCatastoneBlock>());
        }
    }
}