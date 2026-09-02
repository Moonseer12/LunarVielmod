using Stellamod.Content.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.TilesFB
{
    public class OvermorrowdirtWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            DustType = ModContent.DustType<Solution>();
            AddMapEntry(new Color(11, 13, 17));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
    
    public class OvermorrowdirtwallBlock : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<OvermorrowdirtWall>());
        }
    }
}