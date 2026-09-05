using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.SpringHills;

public class RysaHousePass : GenPass
{
    public RysaHousePass() : base("Rysa House", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Rysa is Moving In!";
        bool placed = false;
        int attempts = 0;
        int[] tileBlend = [
            TileID.RubyGemspark
        ];
        Point rysaHousePoint = new();
        rysaHousePoint = ModContent.GetInstance<VeilGen>().FableFarEdgeLocation;
        rysaHousePoint.X += 400;
        rysaHousePoint.Y -= 300;
        rysaHousePoint = TileUtilities.FallToSolidTile(rysaHousePoint.X, rysaHousePoint.Y);
        while (!placed && attempts++ < 10000000)
        {
            string structure = "Structures/Rysahouse";
            Rectangle rectangle = Structurizer.ReadRectangle(structure);
            int[] ChestIndexs = Structurizer.ReadStruct(rysaHousePoint, structure, tileBlend);
            GenerateFallingWoodenBeams(rectangle, rysaHousePoint);

            foreach (int chestIndex in ChestIndexs)
            {
                if (chestIndex == -1)
                    continue;
                var chest = Main.chest[chestIndex];
                var itemsToAdd = new List<(int type, int stack)>();
                itemsToAdd.Add((ModContent.ItemType<ZuisGiftedWand>(), 1));
                AddChestLoot(chest, itemsToAdd);
            }

            placed = true;
        }


        Point gilatineHousePoint = new();
        gilatineHousePoint = ModContent.GetInstance<VeilGen>().FableFarEdgeLocation;
        gilatineHousePoint.X += 800;
        gilatineHousePoint.Y -= 330;
        gilatineHousePoint = TileUtilities.FallToSolidTile(gilatineHousePoint.X, gilatineHousePoint.Y);


        string path = "Structures/GilatineCave";
        gilatineHousePoint.X -= 80;
        gilatineHousePoint.Y += 300;

        gilatineHousePoint.X -= 7;
        gilatineHousePoint.Y += 7;
        gilatineHousePoint.X -= 25;
        Structurizer.ReadStruct(gilatineHousePoint, path, tileBlend);
        Structurizer.ProtectStructure(gilatineHousePoint, path);
        progress.Message = "I'm Racist.";
    }

    public static void GenerateFallingWoodenBeams(Rectangle structureRectangle, Point Loc)
    {
        structureRectangle.Location = Loc;
        for (int beamX = structureRectangle.Location.X;
            beamX < structureRectangle.Location.X + structureRectangle.Width; beamX += 4)
        {
            int beamY = structureRectangle.Location.Y;
            int solidCount = 0;
            while (solidCount < 5)
            {
                if (!WorldGen.SolidTile(beamX, beamY))
                {
                    WorldGen.PlaceTile(beamX, beamY, TileID.WoodenBeam);
                }
                else
                {
                    solidCount++;
                }
                beamY++;
            }
        }
    }

    public static void AddChestLoot(Chest chest, List<(int type, int stack)> itemsToAdd)
    {
        // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
        int chestItemIndex = 0;
        foreach (var itemToAdd in itemsToAdd)
        {
            Item item = new();
            item.SetDefaults(itemToAdd.type);
            item.stack = itemToAdd.stack;
            chest.item[chestItemIndex] = item;
            chestItemIndex++;
            if (chestItemIndex >= 40)
                break; // Make sure not to exceed the capacity of the chest
        }
    }
}