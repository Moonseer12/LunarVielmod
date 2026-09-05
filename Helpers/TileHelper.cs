using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Helpers;

public static class TileHelper
{
    public static Vector2 TileAdj => Lighting.Mode == Terraria.Graphics.Light.LightMode.Retro || Lighting.Mode == Terraria.Graphics.Light.LightMode.Trippy ? Vector2.Zero : Vector2.One * 12;
    public static void DrawInvisTile(int i, int j, SpriteBatch spriteBatch)
    {
        Vector2 pos2 = (new Vector2(i, j) + TileAdj) * 16;
        pos2 -= Main.screenPosition;
        Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Content/Tiles/InvisibleTile").Value;
        spriteBatch.Draw(texture, pos2, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
    }
    public static void DrawInvisTileNoAdj(int i, int j, SpriteBatch spriteBatch)
    {
        Vector2 pos2 = new Vector2(i, j) * 16;
        pos2 -= Main.screenPosition;
        Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Content/Tiles/InvisibleTile").Value;
        spriteBatch.Draw(texture, pos2, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
    }
    public static void GrowVine(int i, int j, int vineTile)
    {
        Tile tile = Framing.GetTileSafely(i, j);
        Tile tileBelow = Framing.GetTileSafely(i, j + 1);
        if (WorldGen.genRand.NextBool(1) && !tileBelow.HasTile && !(tileBelow.LiquidType == LiquidID.Lava))
        {
            if (!tile.BottomSlope)
            {
                tileBelow.TileType = (ushort)vineTile;
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