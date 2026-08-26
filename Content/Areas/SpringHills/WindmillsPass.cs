using Stellamod.Content.Armors.Windmillion;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.WorldG;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.SpringHills;

public class WindmillsPass : GenPass
{
    public WindmillsPass() : base("Windmills", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Adding life to the world!";
        bool placed = false;
        int attempts = 0;
        var genRand = WorldGen.genRand;
        int[] tileBlend = [
            TileID.RubyGemspark
        ];

        Point windmillPlacementTile = new();
        windmillPlacementTile.X = (int)MathHelper.Lerp(ModContent.GetInstance<VeilGen>().MistyHillStartLocation.X, ModContent.GetInstance<VeilGen>().MistyHillEndLocation.X, 0.45f);
        windmillPlacementTile.Y = (int)(Main.worldSurface - 1200);
        windmillPlacementTile = TileUtilities.FallToSolidTile(windmillPlacementTile.X, windmillPlacementTile.Y);
        while (!placed && attempts++ < 10000000)
        {
            string structure = "Structures/Overworld/Windmill";
            int[] ChestIndexs = Structurizer.ReadStruct(windmillPlacementTile, structure, tileBlend);
            Rectangle structureRectangle = Structurizer.ReadRectangle(structure);
            structureRectangle.Location = windmillPlacementTile;
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

            foreach (int chestIndex in ChestIndexs)
            {
                if (chestIndex == -1)
                    continue;
                var chest = Main.chest[chestIndex];
                var itemsToAdd = new List<(int type, int stack)>();

                // Using a switch statement and a random choice to add sets of items.
                switch (Main.rand.Next(4))
                {
                    case 0:
                        itemsToAdd.Add((ModContent.ItemType<WindmillShuriken>(), genRand.Next(1, 1)));
                        break;
                    case 1:
                        itemsToAdd.Add((ModContent.ItemType<WindmillionRobe>(), genRand.Next(1, 1)));
                        itemsToAdd.Add((ModContent.ItemType<WindmillionHat>(), genRand.Next(1, 1)));
                        itemsToAdd.Add((ModContent.ItemType<WindmillionBoots>(), genRand.Next(1, 1)));
                        break;

                    case 2:
                    
                        break;

                    case 3:
                        itemsToAdd.Add((ItemID.BabyBirdStaff, genRand.Next(1, 1)));
                        break;
                }

                itemsToAdd.Add((ItemID.IronOre, genRand.Next(9, 15)));
                if (genRand.NextBool(2))
                {
                    itemsToAdd.Add((ItemID.EndurancePotion, genRand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.WormholePotion, genRand.Next(1, 2)));
                }
                else
                {
                    itemsToAdd.Add((ItemID.SwiftnessPotion, genRand.Next(1, 3)));
                    itemsToAdd.Add((ItemID.WormholePotion, genRand.Next(1, 2)));
                    itemsToAdd.Add((ItemID.SpelunkerPotion, genRand.Next(1, 3)));
                }

                itemsToAdd.Add((ModContent.ItemType<Ivythorn>(), genRand.Next(3, 5)));
                // Finally, iterate through itemsToAdd and actually create the Item instances and add to the chest.item array
                int chestItemIndex = 0;
                foreach (var itemToAdd in itemsToAdd)
                {
                    Item item = new Item();
                    item.SetDefaults(itemToAdd.type);
                    item.stack = itemToAdd.stack;
                    chest.item[chestItemIndex] = item;
                    chestItemIndex++;
                    if (chestItemIndex >= 40)
                        break; // Make sure not to exceed the capacity of the chest
                }
            }

            placed = true;
        }
    }
}