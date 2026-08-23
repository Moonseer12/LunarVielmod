using Stellamod.Core.DecorativeTileSystem;
using Terraria.ModLoader;
using Terraria;

namespace Stellamod.Content.Areas.Cinderspark.TilesCS
{
    public class ManorPillarItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<ManorPillar>();
        }
    }
    public class ManorPillar : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }

}
