using Stellamod.Content.Areas.Terror.TilesTR;
using Stellamod.WorldG;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Terror;

public class ForceCrimsonPass : GenPass
{
    public ForceCrimsonPass() : base("Crimsoning", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Nothing Lol";
        WorldGen.WorldGenParam_Evil = 1;
        WorldGen.crimson = true;
    }
}

public class AegislavPass : GenPass
{
    public AegislavPass() : base("Aegislav", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Creating an Evil Place...";
        Point startTile = ModContent.GetInstance<VeilGen>().MistyHillEndLocation;
        startTile.X -= 50;
        startTile.Y -= 300;
        startTile = TileUtilities.FallToSolidTile(startTile);


        Point endTile = startTile;
        endTile.X += 850;
        endTile.Y -= 500;
        endTile = TileUtilities.FallToSolidTile(endTile);


        int sandTile = ModContent.TileType<AegislavSandTile>();
        float minDepth = 45;
        float maxDepth = 200;
        int[] heights = new int[endTile.X - startTile.X];
        int length = endTile.X - startTile.X;
        int startHeight = (int)Main.worldSurface - 500;

        //Place all the sand
        for (int x = startTile.X; x < endTile.X; x++)
        {
            int localX = x - startTile.X;
            float ratio = localX / (float)length;
            float bump = EasingFunction.QuadraticBump(ratio);
            float depthAtPosition = MathHelper.Lerp(minDepth, maxDepth, bump);

            Point point = new(x, startHeight);
            point = TileUtilities.FallToSolidTile(point);
            heights[localX] = point.Y;


            //Clear every tile above the ground
            for (int d = 0; d < 50; d++)
            {
                Main.tile[x, point.Y - (1 + d)].ClearEverything();
            }
            for (int depthY = 0; depthY < depthAtPosition; depthY++)
            {
                Point tileToPlaceAt = new(x, point.Y + depthY);
                tileToPlaceAt.Y -= 2;
                if (!Main.tile[tileToPlaceAt].HasTile)
                    continue;
                WorldGen.PlaceTile(tileToPlaceAt.X, tileToPlaceAt.Y, sandTile, mute: true, forced: true);
            }
        }


        Point evilPoint = startTile;
        evilPoint.X = (int)MathHelper.Lerp(evilPoint.X, endTile.X, 0.5f);
        evilPoint = TileUtilities.FallToSolidTile(evilPoint);
        evilPoint.Y += 250;
        WorldGen_EvilCircle(evilPoint);
        ushort uGrassTileType = (ushort)sandTile;
        var genRand = WorldGen.genRand;
        //Generate big trees, mangrove trees
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;
            float ratio = localX / length;
            int heightIndex = x - startTile.X;
            int height = heights[heightIndex];

            int y = height;
            Tile tile = Main.tile[x, y];

            Rectangle scanArea = new Rectangle(x, y, 5, 2);
            Point point = new Point(x - scanArea.Width / 2, y);
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(scanArea.Width, scanArea.Height),
                new Actions.TileScanner(uGrassTileType).Output(dictionary));
            int tileCount = dictionary[uGrassTileType];

            if (tileCount >= 5)
            {
                if (genRand.NextBool(32))
                {
                    int treeHeight = genRand.Next(20, 48);
                    VeilGen.PlaceBigTrees<BigDeadTree, BigDeadTreeTop>(x, y, treeHeight);
                }
            }
        }

        //Now we're going to place acacia trees
        ushort bigTreeTileType = (ushort)ModContent.TileType<BigDeadTree>();
        for (int x = startTile.X; x < endTile.X; x++)
        {
            float localX = x - startTile.X;
            float ratio = localX / length;
            int heightIndex = x - startTile.X;
            int height = heights[heightIndex];

            int y = height;
            Tile tile = Main.tile[x, y];

            Rectangle scanArea = new(x, y, 5, 2);
            Point point = new(x - scanArea.Width / 2, y);
            Dictionary<ushort, int> dictionary = new Dictionary<ushort, int>();
            WorldUtils.Gen(point, new Shapes.Rectangle(scanArea.Width, scanArea.Height),
                new Actions.TileScanner(uGrassTileType, bigTreeTileType).Output(dictionary));
            int tileCount = dictionary[uGrassTileType];
            int mangroveTreeCount = dictionary[bigTreeTileType];

            if (tileCount >= 5 && mangroveTreeCount <= 0)
            {
                if (genRand.NextBool(8))
                {
                    int treeHeight = genRand.Next(6, 20);
                    VeilGen.PlaceTrees<DeadTree, DeadTreeTop>(x, y, treeHeight);
                }
            }
        }

        Point aegislavCastlePoint = new();
        aegislavCastlePoint = endTile;
        aegislavCastlePoint.X -= 300;
        aegislavCastlePoint.Y -= 20;
        aegislavCastlePoint = TileUtilities.FallToSolidTile(aegislavCastlePoint.X, aegislavCastlePoint.Y);

        string path = "Structures/BloodletCastle";
        aegislavCastlePoint.Y += 15;
        Structurizer.ReadStruct(aegislavCastlePoint, path, Structurizer.DefaultTileBlend);
        Structurizer.ProtectStructure(aegislavCastlePoint, path);
    }

    public void WorldGen_EvilCircle(Point evilPoint)
    {
        var genRand = WorldGen.genRand;
        int radius = 96;
        ushort blockType = WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone;
        ushort wallType = WorldGen.crimson ? WallID.CrimsonUnsafe1 : WallID.CorruptionUnsafe1;

        WorldUtils.Gen(evilPoint, new Shapes.Circle(radius, radius), new Actions.SetTile(blockType));
        WorldUtils.Gen(evilPoint, new Shapes.Circle(radius - 20, radius - 20), new Actions.ClearTile());
        WorldUtils.Gen(evilPoint, new Shapes.Circle(radius - 40, radius - 40), new Actions.SetTile(blockType));

        ushort[] corruptWallTypes = [
            WallID.CorruptionUnsafe1,
            WallID.CorruptionUnsafe2,
            WallID.EbonstoneUnsafe
        ];

        ushort[] crimsonWallTypes = [
            WallID.CrimsonUnsafe1,
            WallID.CrimsonUnsafe2,
            WallID.CrimstoneUnsafe
        ];

        int decorativeBlock = WorldGen.crimson ? TileID.FleshBlock : TileID.LesionBlock;
        int lampType = WorldGen.crimson ? 14 : 33;
        int lanternType = WorldGen.crimson ? 23 : 39;
        for (int w = 0; w < 800; w++)
        {
            Point shadowOrbPoint = evilPoint + genRand.NextVector2Circular(80, 80).ToPoint();

            ushort wallType2 = WorldGen.crimson ?
                crimsonWallTypes[genRand.Next(0, crimsonWallTypes.Length)] :
                corruptWallTypes[genRand.Next(0, corruptWallTypes.Length)];
            WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(4, 4), Actions.Chain([
                new Actions.PlaceWall(wallType2),
                new Actions.Smooth(true)
            ]));
        }

        for (int w = 0; w < 150; w++)
        {
            int radius2 = genRand.Next(50, 100);
            Point shadowOrbPoint = evilPoint + genRand.NextVector2CircularEdge(radius2, radius2).ToPoint();
            ushort wallType2 = WorldGen.crimson ? WallID.Flesh : WallID.LesionBlock;
            WorldUtils.Gen(shadowOrbPoint, new Shapes.Circle(1, 1), Actions.Chain([
                new Actions.PlaceWall(wallType2),
                new Actions.Smooth(true)
            ]));
        }


        float pokey = 12;
        for (int n = 0; n < pokey; n++)
        {
            float p = n / pokey;
            float rot = p * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * 66;
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new(3, 4);

            Vector2 moveVelocity = -velocity.SafeNormalize(Vector2.Zero);
            VeilGen.GenerateSimpleCave(cavePoint.ToVector2(), moveVelocity,
                strength, moveVelocity, 2, caveSteps: 30);
        }

        for (int n = 0; n < 800; n++)
        {
            float p = n / 800f;
            float rot = p * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(50, 80);
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new(3, 4);

            WorldGen.TileRunner(cavePoint.X, cavePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), -1);
        }

        for (int n = 0; n < 800; n++)
        {
            float p = n / 800f;
            float rot = p * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(50, 80);
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new(3, 4);


            WorldGen.TileRunner(cavePoint.X, cavePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), decorativeBlock);
        }

        for (int n = 0; n < 800; n++)
        {
            float p = n / 800f;
            float rot = p * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * genRand.NextFloat(60, 100);
            Point cavePoint = evilPoint + velocity.ToPoint();
            Vector2 strength = new(3, 4);

            WorldGen.TileRunner(cavePoint.X, cavePoint.Y,
                genRand.NextFloat(strength.X, strength.Y),
                genRand.Next(4, 5), decorativeBlock);
        }

        for (int n = 0; n < 10; n++)
        {
            float p = n / 10f;
            float rot = p * MathHelper.TwoPi;
            rot += MathHelper.ToRadians(30);
            Vector2 velocity = rot.ToRotationVector2() * 10;
            Point shadowOrbPoint = evilPoint + velocity.ToPoint();
            WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
        }

        for (int n = 0; n < 10; n++)
        {
            float p = n / 10f;
            float rot = p * MathHelper.TwoPi;
            rot += MathHelper.ToRadians(60);
            Vector2 velocity = rot.ToRotationVector2() * 30;
            Point shadowOrbPoint = evilPoint + velocity.ToPoint();
            WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
        }

        for (int n = 0; n < 10; n++)
        {
            float p = n / 10f;
            float rot = p * MathHelper.TwoPi;
            Vector2 velocity = rot.ToRotationVector2() * 50;
            Point shadowOrbPoint = evilPoint + velocity.ToPoint();
            WorldGen.AddShadowOrb(shadowOrbPoint.X, shadowOrbPoint.Y);
        }

        for (int n = 0; n < 1600; n++)
        {
            float range = genRand.NextFloat(30, 100);
            Point fPoint = evilPoint + genRand.NextVector2CircularEdge(range, range).ToPoint();

            WorldGen.Place1xX(fPoint.X, fPoint.Y, TileID.Lamps, style: lampType);
        }
        for (int n = 0; n < 800; n++)
        {
            float range = genRand.NextFloat(30, 100);
            Point fPoint = evilPoint + genRand.NextVector2CircularEdge(range, range).ToPoint();
            WorldGen.Place1x2Top(fPoint.X, fPoint.Y, TileID.HangingLanterns, style: lanternType);
        }

        //Make Extra
        Vector2 caveStrength = new(10, 12);
        Vector2 pullDirection = -Vector2.UnitY;
        int caveWidth = 5;
        int steps = 150;

        VeilGen.GenerateStraightCaveWall((evilPoint + new Point(-16, -32)).ToVector2(), pullDirection, caveStrength * 2f, pullDirection, caveWidth, caveSteps: steps, tileToPlace: wallType);
        VeilGen.GenerateStraightCave((evilPoint + new Point(-16, -32)).ToVector2(), pullDirection, caveStrength * 2f, pullDirection, caveWidth, caveSteps: steps, tileToPlace: blockType);
        VeilGen.GenerateStraightCave((evilPoint + new Point(-16, -32)).ToVector2(), pullDirection, caveStrength, pullDirection, caveWidth, caveSteps: steps, tileToPlace: -1);

        int fallSteps = 40;
        VeilGen.GenerateSimpleCave((evilPoint + new Point(0, 48)).ToVector2(), Vector2.UnitY, caveStrength * 2f, Vector2.UnitY, caveWidth,
            caveSteps: fallSteps,
            tileToPlace: blockType);
        VeilGen.GenerateSimpleCave((evilPoint + new Point(0, 48)).ToVector2(), Vector2.UnitY, caveStrength, Vector2.UnitY, caveWidth,
            caveSteps: fallSteps,
            tileToPlace: -1);
        VeilGen.GenerateSimpleCave((evilPoint + new Point(-128, 100)).ToVector2(), Vector2.UnitX, caveStrength * 2f, Vector2.UnitX, caveWidth,
            caveSteps: fallSteps * 2,
            tileToPlace: blockType,
            addTile: true);
        VeilGen.GenerateSimpleCave((evilPoint + new Point(-128, 100)).ToVector2(), Vector2.UnitX, caveStrength, Vector2.UnitX, caveWidth,
            caveSteps: fallSteps * 2,
            tileToPlace: -1);

        for (int n = 0; n < 6400; n++)
        {
            int x = genRand.Next(evilPoint.X - 128, evilPoint.X + 128);
            int y = genRand.Next(evilPoint.Y + 90, evilPoint.Y + 150);
            int style = WorldGen.crimson ? 1 : 0;
            WorldGen.Place3x2(x, y, 26, style);
        }

        for (int x = evilPoint.X - 128; x < evilPoint.X + 128; x++)
        {
            int y = evilPoint.Y + 100;
            Point wallPoint = new(x, y);
            ushort wallType2 = WorldGen.crimson ? WallID.CrimstoneUnsafe : WallID.EbonstoneUnsafe;
            WorldUtils.Gen(wallPoint, new Shapes.Circle(8, 8), Actions.Chain(new GenAction[]
            {
                new Actions.PlaceWall(wallType2),
                new Actions.Smooth(true)
            }));
        }


        //Crimsonfy/Ebonfy surroundings
        for (int x = evilPoint.X - radius; x < evilPoint.X + radius; x++)
        {
            for (int y = evilPoint.Y - radius; y < evilPoint.Y + radius; y++)
            {
                if (!WorldGen.SolidTile(x, y))
                    continue;
                Tile tile = Main.tile[x, y];
                if (tile.TileType == TileID.Grass)
                {
                    ushort grassType = WorldGen.crimson ? TileID.CrimsonGrass : TileID.CorruptGrass;
                    WorldGen.PlaceTile(x, y, grassType);
                }
                if (tile.TileType == TileID.Stone)
                {
                    WorldGen.PlaceTile(x, y, blockType);
                }
            }
        }
    }
}