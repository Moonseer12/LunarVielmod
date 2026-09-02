using Stellamod.Content.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.TilesDG
{
    public class BlackWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            DustType = ModContent.DustType<Sparkle>();
            AddMapEntry(new Color(1, 1, 1));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
    public class BlackWallBlock : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<BlackWall>());
        }
    }
}