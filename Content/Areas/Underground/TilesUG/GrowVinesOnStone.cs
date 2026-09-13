using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.TilesUG;

public class GrowVinesOnStone : GlobalTile
{
    public override void RandomUpdate(int i, int j, int type)
    {
        if (type == TileID.Stone)
        {
            if (WorldGen.genRand.NextBool(256))
            {
                TileHelper.GrowVine(i, j, ModContent.TileType<IlluriaVines>());
            }
        }
    }
}
