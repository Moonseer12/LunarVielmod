using Stellamod.Content.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.TilesMT
{
    public class CathediteWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;

            DustType = ModContent.DustType<Sparkle>();


            AddMapEntry(new Color(200, 200, 200));
        }
        public override bool CanExplode(int i, int j) => false;
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
    
    public class CathediteWall : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<CathediteWallTile>());
        }
    }
}