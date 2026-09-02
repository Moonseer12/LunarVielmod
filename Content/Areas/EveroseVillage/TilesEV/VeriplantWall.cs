using Stellamod.Content.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.EveroseVillage.TilesEV
{
    public class VeriplantWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            DustType = ModContent.DustType<Solution>();
            AddMapEntry(new Color(69, 85, 37));
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class VeriplantGrassWall : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<VeriplantWall>());
        }
    }
}