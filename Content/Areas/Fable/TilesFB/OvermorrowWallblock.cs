using Stellamod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.TilesFB
{
    public class OvermorrowWallblock : ModWall
    {
        public override void SetStaticDefaults()
        {
            DustType = ModContent.DustType<Solution>();
            AddMapEntry(new Color(200, 200, 200));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
    
    public class OvermorrowWall : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<OvermorrowWallblock>());
        }
    }
}