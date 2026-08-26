using Stellamod.Content.Areas.Underground.TilesUG;
using Terraria;
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
                Tile tile = Framing.GetTileSafely(i, j);
                Tile tileBelow = Framing.GetTileSafely(i, j + 1);
                if (WorldGen.genRand.NextBool(2) && !tileBelow.HasTile)
                {
                    if (!tile.BottomSlope)
                    {
                        tileBelow.TileType = (ushort)ModContent.TileType<IlluriaVines>();
                        tileBelow.HasTile = true;
                        WorldGen.SquareTileFrame(i, j + 1, true);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                        }
                    }
                }
                if (WorldGen.genRand.NextBool(2) && !tileBelow.HasTile)
                {
                    if (!tile.BottomSlope)
                    {
                        tileBelow.TileType = (ushort)ModContent.TileType<IlluriaVines>();
                        tileBelow.HasTile = true;
                        WorldGen.SquareTileFrame(i, j + 1, true);
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                        }
                    }
                }
            }
        }
    }
}