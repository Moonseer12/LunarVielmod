using Stellamod.Content.Areas.Underground.TilesUG;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground
{
    public class GlobalTileEdits : GlobalTile
    {
        public override void RandomUpdate(int i, int j, int type)
        {
            if (type == TileID.Stone)
            {
                TileHelper.GrowVine(i, j, ModContent.TileType<IlluriaVines>());
            }
        }
    }
}