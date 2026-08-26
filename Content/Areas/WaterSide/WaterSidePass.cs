using Stellamod.Content.Areas.WaterSide.TilesWS;
using Stellamod.Core.ZTileSystem;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.WaterSide
{
    public class ReworkedBeachesPass : GenPass
    {
        public ReworkedBeachesPass()
            : base("Beaches", 449.3721923828125)
        {
        }
        private static double TuneOceanDepth(int count, double depth, bool floridaStyle = false)
        {
            var genRand = WorldGen.genRand;
            if (!floridaStyle)
            {
                if (count < 3)
                    depth += genRand.Next(10, 20) * 0.2;
                else if (count < 6)
                    depth += genRand.Next(10, 20) * 0.15;
                else if (count < 9)
                    depth += genRand.Next(10, 20) * 0.1;
                else if (count < 15)
                    depth += genRand.Next(10, 20) * 0.07;
                else if (count < 50)
                    depth += genRand.Next(10, 20) * 0.05;
                else if (count < 75)
                    depth += genRand.Next(10, 20) * 0.04;
                else if (count < 100)
                    depth += genRand.Next(10, 20) * 0.03;
                else if (count < 125)
                    depth += genRand.Next(10, 20) * 0.02;
                else if (count < 150)
                    depth += genRand.Next(10, 20) * 0.01;
                else if (count < 175)
                    depth += genRand.Next(10, 20) * 0.005;
                else if (count < 200)
                    depth += genRand.Next(10, 20) * 0.001;
                else if (count < 230)
                    depth += genRand.Next(10, 20) * 0.01;
                else if (count < 235)
                    depth += genRand.Next(10, 20) * 0.05;
                else if (count < 240)
                    depth += genRand.Next(10, 20) * 0.1;
                else if (count < 245)
                    depth += genRand.Next(10, 20) * 0.05;
                else if (count < 255)
                    depth += genRand.Next(10, 20) * 0.01;
            }
            else if (count < 3)
            {
                depth += genRand.Next(10, 20) * 0.001;
            }
            else if (count < 6)
            {
                depth += genRand.Next(10, 20) * 0.002;
            }
            else if (count < 9)
            {
                depth += genRand.Next(10, 20) * 0.004;
            }
            else if (count < 15)
            {
                depth += genRand.Next(10, 20) * 0.007;
            }
            else if (count < 50)
            {
                depth += genRand.Next(10, 20) * 0.01;
            }
            else if (count < 75)
            {
                depth += genRand.Next(10, 20) * 0.014;
            }
            else if (count < 100)
            {
                depth += genRand.Next(10, 20) * 0.019;
            }
            else if (count < 125)
            {
                depth += genRand.Next(10, 20) * 0.027;
            }
            else if (count < 150)
            {
                depth += genRand.Next(10, 20) * 0.038;
            }
            else if (count < 175)
            {
                depth += genRand.Next(10, 20) * 0.052;
            }
            else if (count < 200)
            {
                depth += genRand.Next(10, 20) * 0.08;
            }
            else if (count < 230)
            {
                depth += genRand.Next(10, 20) * 0.12;
            }
            else if (count < 235)
            {
                depth += genRand.Next(10, 20) * 0.16;
            }
            else if (count < 240)
            {
                depth += genRand.Next(10, 20) * 0.27;
            }
            else if (count < 245)
            {
                depth += genRand.Next(10, 20) * 0.43;
            }
            else if (count < 255)
            {
                depth += genRand.Next(10, 20) * 0.6;
            }

            return depth;
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            int num731 = 50;
            progress.Message = Lang.gen[22].Value;
            bool floridaStyle = false;
            bool floridaStyle2 = false;
            var genRand = WorldGen.genRand;
            if (genRand.Next(4) == 0)
            {
                if (genRand.Next(2) == 0)
                    floridaStyle = true;
                else
                    floridaStyle2 = true;
            }

            for (int num732 = 0; num732 < 2; num732++)
            {
                int num733 = 0;
                int num734 = 0;
                if (num732 == 0)
                {
                    /*
                    num733 = 0;
                    num734 = genRand.Next(GenVars.oceanWaterStartRandomMin, GenVars.oceanWaterStartRandomMax);
                    if (GenVars.dungeonSide == 1)
                        num734 = GenVars.oceanWaterForcedJungleLength;

                    int num735 = GenVars.leftBeachEnd - num731;
                    if (num734 > num735)
                        num734 = num735;

                    int num736 = 0;
                    double num737 = 1.0;
                    int num738;
                    for (num738 = 0; !Main.tile[num734 - 1, num738].active(); num738++)
                    {
                    }

                    GenVars.shellStartYLeft = num738;
                    num738 += genRand.Next(1, 5);
                    for (int num739 = num734 - 1; num739 >= num733; num739--)
                    {
                        if (num739 > 30)
                        {
                            num736++;
                            num737 = TuneOceanDepth(num736, num737, floridaStyle);
                        }
                        else
                        {
                            num737 += 1.0;
                        }

                        int num740 = genRand.Next(15, 20);
                        for (int num741 = 0; (double)num741 < (double)num738 + num737 + (double)num740; num741++)
                        {
                            if ((double)num741 < (double)num738 + num737 * 0.75 - 3.0)
                            {
                                Main.tile[num739, num741].active(active: false);
                                if (num741 > num738)
                                {
                                    Main.tile[num739, num741].liquid = byte.MaxValue;
                                    Main.tile[num739, num741].lava(lava: false);
                                }
                                else if (num741 == num738)
                                {
                                    Main.tile[num739, num741].liquid = 127;
                                    if (GenVars.shellStartXLeft == 0)
                                        GenVars.shellStartXLeft = num739;
                                }
                            }
                            else if (num741 > num738)
                            {
                                Main.tile[num739, num741].TileType = 53;
                                Main.tile[num739, num741].active(active: true);
                            }

                            Main.tile[num739, num741].WallType = 0;
                        }
                    }*/
                }
                else
                {
                    num733 = Main.maxTilesX - genRand.Next(GenVars.oceanWaterStartRandomMin, GenVars.oceanWaterStartRandomMax);
                    num734 = Main.maxTilesX;
                    if (GenVars.dungeonSide == -1)
                        num733 = Main.maxTilesX - GenVars.oceanWaterForcedJungleLength;

                    int num742 = GenVars.rightBeachStart + num731;
                    if (num733 < num742)
                        num733 = num742;

                    double num743 = 1.0;
                    int num744 = 0;
                    int num745;
                    for (num745 = 0; !Main.tile[num733, num745].HasTile; num745++)
                    {
                    }

                    GenVars.shellStartXRight = 0;
                    GenVars.shellStartYRight = num745;
                    num745 += genRand.Next(1, 5);
                    for (int num746 = num733; num746 < num734; num746++)
                    {
                        if (num746 < num734 - 30)
                        {
                            num744++;
                            num743 = TuneOceanDepth(num744, num743, floridaStyle2);
                        }
                        else
                        {
                            num743 += 1.0;
                        }

                        int num747 = genRand.Next(15, 20);
                        for (int num748 = 0; (double)num748 < (double)num745 + num743 + (double)num747; num748++)
                        {
                            if ((double)num748 < (double)num745 + num743 * 0.75 - 3.0)
                            {
                           
                                WorldGen.KillTile(num746, num748);
                                if (num748 > num745)
                                {
                                    WorldGen.PlaceLiquid(num746, num748, (byte)LiquidID.Water, byte.MaxValue);
                                }
                                else if (num748 == num745)
                                {
                                    WorldGen.PlaceLiquid(num746, num748, (byte)LiquidID.Water, 127);
                                    if (GenVars.shellStartXRight == 0)
                                        GenVars.shellStartXRight = num746;
                                }
                            }
                            else if (num748 > num745)
                            {
                                WorldGen.PlaceTile(num746, num748, TileID.Sand);
                            }

                            Main.tile[num746, num748].WallType = 0;
                        }
                    }
                }
            }
        }
    }

    public class ReworkedOceanSandPass : GenPass
    {
        public ReworkedOceanSandPass() : base("Ocean Sand", 449.3721923828125)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            var genRand = WorldGen.genRand;
            progress.Message = Language.GetTextValue("WorldGeneration.OceanSand");

            //by starting this index at 1 instead of 0, we skip doing the left beach
            for (int num1069 = 1; num1069 < 3; num1069++)
            {
                progress.Set((double)num1069 / 3.0);
                int num1070 = genRand.Next(Main.maxTilesX);
                while ((double)num1070 > (double)Main.maxTilesX * 0.4 && (double)num1070 < (double)Main.maxTilesX * 0.6)
                {
                    num1070 = genRand.Next(Main.maxTilesX);
                }

                int num1071 = genRand.Next(35, 90);
                if (num1069 == 1)
                {
                    double num1072 = (double)Main.maxTilesX / 4200.0;
                    num1071 += (int)((double)genRand.Next(20, 40) * num1072);
                }

                if (genRand.Next(3) == 0)
                    num1071 *= 2;

                if (num1069 == 1)
                    num1071 *= 2;

                int num1073 = num1070 - num1071;
                num1071 = genRand.Next(35, 90);
                if (genRand.Next(3) == 0)
                    num1071 *= 2;

                if (num1069 == 1)
                    num1071 *= 2;

                int num1074 = num1070 + num1071;
                if (num1073 < 0)
                    num1073 = 0;

                if (num1074 > Main.maxTilesX)
                    num1074 = Main.maxTilesX;

                if (num1069 == 0)
                {
                    num1073 = 0;
                    num1074 = GenVars.leftBeachEnd;
                }
                else if (num1069 == 2)
                {
                    num1073 = GenVars.rightBeachStart;
                    num1074 = Main.maxTilesX;
                }
                else if (num1069 == 1)
                {
                    continue;
                }

                int num1075 = genRand.Next(50, 100);
                for (int num1076 = num1073; num1076 < num1074; num1076++)
                {
                    if (genRand.Next(2) == 0)
                    {
                        num1075 += genRand.Next(-1, 2);
                        if (num1075 < 50)
                            num1075 = 50;

                        if (num1075 > 200)
                            num1075 = 200;
                    }

                    for (int num1077 = 0; (double)num1077 < (Main.worldSurface + Main.rockLayer) / 2.0; num1077++)
                    {
                        if (Main.tile[num1076, num1077].HasTile)
                        {
                            if (num1076 == (num1073 + num1074) / 2 && genRand.Next(6) == 0)
                            {
                                GenVars.PyrX[GenVars.numPyr] = num1076;
                                GenVars.PyrY[GenVars.numPyr] = num1077;
                                GenVars.numPyr++;
                            }

                            int num1078 = num1075;
                            if (num1076 - num1073 < num1078)
                                num1078 = num1076 - num1073;

                            if (num1074 - num1076 < num1078)
                                num1078 = num1074 - num1076;

                            num1078 += genRand.Next(5);
                            for (int num1079 = num1077; num1079 < num1077 + num1078; num1079++)
                            {
                                if (num1076 > num1073 + genRand.Next(5) && num1076 < num1074 - genRand.Next(5))
                                    Main.tile[num1076, num1079].TileType = TileID.Sand;
                            }

                            break;
                        }
                    }
                }
            }
        }
    }

    public class InitializePyrPass : GenPass
    {
        public InitializePyrPass() : base("Desert Pyr", 449.3721923828125)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            GenVars.PyrX = new int[3];
            GenVars.PyrY = new int[3];
        }
    }

    public class WaterWobbleCavePass : GenPass
    {
        public WaterWobbleCavePass() : base("Water Wobble Cave", 449.3721923828125)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
        progress.Message = "Water Wobble Cave";
        Point caveOrigin = ModContent.GetInstance<VeilGen>().CoralwaysLocation;
        caveOrigin.X += 60;
        caveOrigin.Y += 334;
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("WaterWobbleCave");
        Rectangle bounds = prefab.GetBounds(caveOrigin.X, caveOrigin.Y, PrefabPlacementType.FromTopRight);


        int deepSeaTile = ModContent.TileType<DeepSeaTile>();
        int pinkSandTile = ModContent.TileType<PinkSandTile>();
        int reefTile = ModContent.TileType<ReefTile>();
        int[] tiles = [
            deepSeaTile,
            pinkSandTile,
            reefTile
        ];
        for (int x = bounds.Left; x < bounds.Right; x++)
        {
            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                ModContent.GetInstance<ZTileMap>().KillAnyTile(new Point(x, y));
            }
        }


        //Fill up area with random tiles fr
        for (int x = bounds.Left; x < bounds.Right; x++)
        {
            for (int y = bounds.Top; y < bounds.Bottom; y++)
            {
                if (!Main.rand.NextBool(16))
                    continue;

                int randTile = Main.rand.Next(3);
                int tileToPlace = tiles[randTile];
                WorldGen.TileRunner(x, y, 16, 32, tileToPlace, addTile: true, 1, 1);
            }
        }

        prefab.PasteErase(caveOrigin, PrefabPlacementType.FromTopRight);
        }
    }

    public class RunicaUnderwaterPass : GenPass
    {
        public RunicaUnderwaterPass() : base("Runica Underwater", 449.3721923828125)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
        progress.Message = "Runica Caves";
        var rand = WorldGen.genRand;
        int caveOriginX = GenVars.snowOriginRight + 250;
        int caveOriginY = (int)Main.worldSurface - 100;

        Point point = new(caveOriginX, caveOriginY);
        caveOriginY = TileUtilities.FallToSolidTile(point).Y;

        int width = 500;
        int left = caveOriginX - width / 2;
        int right = caveOriginX + width / 2;
        int bottom = caveOriginY + 1800;

        int deepSeaTile = ModContent.TileType<DeepSeaTile>();
        int pinkSandTile = ModContent.TileType<PinkSandTile>();
        int reefTile = ModContent.TileType<ReefTile>();


        void ScatterBlotch(int numBlotches, int t)
        {
            int attempts = 0;
            int n = 0;
            while (n < numBlotches)
            {
                attempts++;
                if (attempts > 1000000)
                {
                    Console.WriteLine("Failed to generate enough blotches");
                    break;
                }
                int randY = rand.Next(caveOriginY, bottom);
                int randX = rand.Next(left, right);
                if (randX >= Main.maxTilesX)
                    continue;

                Tile tile = Main.tile[randX, randY];
                if (!tile.HasTile)
                    continue;
                if (tile.TileType != deepSeaTile)
                    continue;

                //We have a spot
                float strength = rand.NextFloat(4, 8);
                int steps = rand.Next(5, 10);
                WorldGen.OreRunner(randX, randY, strength, steps, (ushort)t);
                n++;
            }
        }
        void ScatterBlotchEdges(int numBlotches, int t)
        {
            int attempts = 0;
            int n = 0;
            while (n < numBlotches)
            {
                attempts++;
                if (attempts > 1000000)
                {
                    Console.WriteLine("Failed to generate enough blotches");
                    break;
                }
                int randY = rand.Next(caveOriginY, bottom);
                int randX = rand.Next(left, right);
                if (randX >= Main.maxTilesX)
                    continue;

                Tile tile = Main.tile[randX, randY];
                if (!tile.HasTile)
                    continue;
                if (tile.TileType != reefTile)
                    continue;
                if (!WorldGen.TileIsExposedToAir(randX, randY))
                    continue;
                //We have a spot
                float strength = rand.NextFloat(8, 16);
                int steps = rand.Next(10, 20);
                WorldGen.OreRunner(randX, randY, strength, steps, (ushort)t);
                n++;
            }
        }

        void ScatterBlotchWallEdges(int numBlotches, params ushort[] wallIDs)
        {
            int attempts = 0;
            int n = 0;
            while (n < numBlotches)
            {
                attempts++;
                if (attempts > 1000000)
                {
                    Console.WriteLine("Failed to generate enough blotches");
                    break;
                }
                int randY = rand.Next(caveOriginY, bottom);
                int randX = rand.Next(left, right);
                if (randX >= Main.maxTilesX)
                    continue;

                Tile tile = Main.tile[randX, randY];
                if (!tile.HasTile)
                    continue;
                if (!WorldGen.TileIsExposedToAir(randX, randY))
                    continue;

                Point point = new(randX, randY);
                int steps = rand.Next(1, 4);
                Vector2 baseDirection = -Vector2.UnitY;
                int caveWidth = 3;

                byte paint = PaintID.TealPaint;
                switch (rand.Next(4))
                {
                    case 0:
                        break;
                    case 1:
                        paint = PaintID.SkyBluePaint;
                        break;
                    case 2:
                        paint = PaintID.PinkPaint;
                        break;
                    case 3:
                        paint = PaintID.RedPaint;
                        break;
                }
                for (int s = 0; s < steps; s++)
                {
                    if (point.X - caveWidth > 0 && point.X + caveWidth < Main.maxTilesX && point.Y + caveWidth < Main.maxTilesY && point.Y - caveWidth > 0)
                    {
                        ushort wallId = wallIDs[rand.Next(wallIDs.Length)];
                        WorldUtils.Gen(point, new Shapes.Circle(caveWidth, caveWidth),
                            Actions.Chain(
                                new Actions.PlaceWall(wallId),
                                new PaintWall(paint)));
                    }

                    point += (baseDirection * caveWidth).RotatedByRandom(MathHelper.ToRadians(30)).ToPoint();
                }
                n++;
            }
        }

        for (int y = caveOriginY; y < bottom; y++)
        {

            for (int x = left; x < right && x < Main.maxTilesX; x++)
            {
                float ratio = (x - left) / (float)(right - left);
                float ease = EasingFunction.QuadraticBump(ratio);

                if (ease < 0.5f)
                {
                    int denom = (int)MathHelper.Lerp(1, 8, ease);
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
                    int tileToPlace = deepSeaTile;
                    if (y > bottom - 400)
                        tileToPlace = ModContent.TileType<SeavathanBrick>();
                    WorldGen.PlaceTile(x, y, tileToPlace, forced: true);
                }
            }
        }

        ModContent.GetInstance<VeilGen>().CoralwaysLocation = new Point(caveOriginX - 150, caveOriginY);
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab("HarmonicCoralways");
        prefab.PasteErase(caveOriginX, caveOriginY, PrefabPlacementType.FromTopCenter);

        //Set random reef blocks
        ScatterBlotchEdges(200, TileID.ShellPile);
        ScatterBlotch(3500, pinkSandTile);
        ScatterBlotch(3500, reefTile);
        ScatterBlotch(500, TileID.ReefBlock);
        ScatterBlotch(1500, TileID.Coralstone);
        ScatterBlotchWallEdges(15000, WallID.PoopWall, WallID.PoopWall, WallID.PoopWall, WallID.HardenedSandEcho, WallID.SandstoneEcho);


        ZTileMap tileMap = ModContent.GetInstance<ZTileMap>();
        var items = new ZTile[]
        {
            ModContent.GetInstance<RedCoralMedium>(),
            ModContent.GetInstance<BlueCoralLarge>(),
            ModContent.GetInstance<PinkCoralLarge>()
        };


        for (int y = caveOriginY; y < bottom; y++)
        {

            for (int x = left; x < right && x < Main.maxTilesX; x++)
            {
                if (!WorldGen.TileIsExposedToAir(x, y))
                    continue;
                Tile mainTile = Main.tile[x, y];
                if (!mainTile.HasTile)
                    continue;

                if (!rand.NextBool(7))
                    continue;

                ZTile tile = items[rand.Next(items.Length)];
                var templateData = ModContent.GetInstance<ZTileLoader>().InstanceTileData(tile);
                DecorationBuilder.frame = 0;

                ZTileInstanceData instanceData = templateData;
                instanceData.scale = 1;
                instanceData.rotation = 0;
                instanceData.frameNumber = 0;
                instanceData.flipX = false;
                instanceData.value = 0;

                Vector2 position = new Point(x, y + 1).ToWorldCoordinates();
                tileMap.CreateTile(ZRenderLayer.InFrontOfWalls, position, 0, instanceData);
            }
        }

        for (int y = caveOriginY; y < bottom; y++)
        {
            for (int x = left; x < right; x++)
            {
                WorldGen.PlaceLiquid(x, y, (byte)LiquidID.Water, byte.MaxValue);
            }
        }


        for (int y = caveOriginY; y < bottom; y++)
        {
            for (int x = left; x < right; x++)
            {
                WorldGen.PlaceLiquid(x, y, (byte)LiquidID.Water, byte.MaxValue);
            }
        }

        //Just throw a big ass circle of water at the top to fill the empty space
        Point centerPoint = new(caveOriginX, caveOriginY);
        WorldUtils.Gen(centerPoint, new Shapes.Circle(10, 10),
            Actions.Chain([
            new Actions.SetLiquid(LiquidID.Water)
        ]));
        }
    }
}