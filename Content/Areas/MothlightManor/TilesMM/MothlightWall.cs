using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MothlightManor.TilesMM
{
    public class MothlightWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(200, 200, 200));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class MothlightWallBlock : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<MothlightWall>());
        }
    }
}