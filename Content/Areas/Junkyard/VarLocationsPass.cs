using Stellamod.Content.Areas.Junkyard.TilesJY;
using Stellamod.WorldG;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Junkyard;

public class JunkyardCavesPass : GenPass
{
    public JunkyardCavesPass() : base("Junkyard Caves", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Junkyard Caves";
        int caveOriginX = ModContent.GetInstance<VeilGen>().MarshLocation.X;
        caveOriginX -= 350;

        int caveOriginY = ModContent.GetInstance<VeilGen>().MarshLocation.Y;
        caveOriginY -= 35;

        int width = 500;

        int left = caveOriginX - width / 2;
        int right = caveOriginX + width / 2;
        int bottom = caveOriginY + 1800;
        int tileType = ModContent.TileType<JunkyTile>();
        for (int y = caveOriginY; y < bottom; y++)
        {

            for (int x = left; x < right; x++)
            {
                float ratio = (x - left) / (float)(right - left);
                float ease = EasingFunction.QuadraticBump(ratio);
                int denom = (int)MathHelper.Lerp(1, 8, ease);
                if (ease < 0.5f)
                {
                    if (Main.rand.NextBool(denom))
                        continue;
                }

                if (caveOriginY > bottom - 25)
                {
                    float heightRatio = (caveOriginY - (bottom - 25)) / 25f;
                    int heightDenom = (int)MathHelper.Lerp(1, 16, heightRatio);
                    if (!Main.rand.NextBool(heightDenom))
                        continue;
                }
                Tile tile = Main.tile[x, y];
                if (tile.HasTile)
                {
                    WorldGen.PlaceTile(x, y, tileType, forced: true);
                }
            }
        }

        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("Junkyard");
        prefab.PasteErase(caveOriginX, caveOriginY, PrefabPlacementType.FromTopCenter);
    }
}