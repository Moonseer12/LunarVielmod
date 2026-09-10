using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas;

public class VeilGenTester : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.useTime = 1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.useAnimation = 1;
    }

    public override bool? UseItem(Player player)
    {
        IlluriaUndergroundTest();
        return true;
    }

    private static void IlluriaUndergroundTest()
    {
        Point mouseTile = Main.MouseWorld.ToTileCoordinates();
        for(int x = mouseTile.X; x < mouseTile.X + 1300; x++)
        {
            for(int y = mouseTile.Y; y < mouseTile.Y + 2000; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.HasTile = true;
                tile.TileType = TileID.Stone;
            }
        }

        void SetTile(int tileX, int tileY, Color c)
        {
            Tile tile = Main.tile[tileX, tileY];
            if(c.R == 255 && c.G == 0 && c.B == 0)
            {
                tile.ClearEverything();
            }
            if (c.R == 255 && c.G == 255 && c.B == 255)
            {
                tile.ClearEverything();
            }
            if (c.R == 0 && c.G == 0 && c.B == 255)
            {
                tile.ClearEverything();
                tile.LiquidAmount = 255;
                tile.LiquidType = LiquidID.Water;
            }
        }

        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("IlluriaBase");
        prefab.PasteErase(mouseTile.X, mouseTile.Y, PrefabPlacementType.FromTopLeft, SetTile);

    }

}
