using Stellamod.Content.Areas.Tundra.Abyss.TilesAB;
using Stellamod.Content.Areas.Tundra.Snow.TilesSN;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Tundra.Abyss;

public class AbyssPass : GenPass
{
    public AbyssPass() : base("Abyss", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Shifting Shadows deep in the Ice";
        //Calculate center of the abyss
        ModContent.GetInstance<VeilGen>().AbyssCenter = new();
        ModContent.GetInstance<VeilGen>().AbyssCenter.X = GenVars.snowOriginLeft + GenVars.snowOriginRight;
        ModContent.GetInstance<VeilGen>().AbyssCenter.X /= 2;
        ModContent.GetInstance<VeilGen>().AbyssCenter.Y = (int)(GenVars.rockLayerHigh + Main.maxTilesY * 0.15);
        ModContent.GetInstance<VeilGen>().AbyssCenter.Y -= 20;
        //Place the center like a circle

        ushort abyssTile = (ushort)ModContent.TileType<AbyssalDirt>();

        int abyssHigh = ModContent.GetInstance<VeilGen>().AbyssCenter.Y - 500;
        int abyssLow = ModContent.GetInstance<VeilGen>().AbyssCenter.Y + 350;

        //Fill the entire area with abyss dirt tiles
        for (int x = GenVars.snowOriginLeft; x < GenVars.snowOriginRight; x++)
        {
            for (int y = abyssHigh; y < abyssLow; y++)
            {
                Tile tile = Main.tile[x, y];
                tile.TileFrameX = -1;
                tile.TileFrameY = -1;
                tile.HasTile = true;
                tile.TileType = abyssTile;
            }
        }

        var genRand = WorldGen.genRand;

        //Sprinkle Blotches of Ice, Snow, and Thick Snow tiles
        //This will add nice variation within the blocks
        Span<ushort> pool = new ushort[3].AsSpan();
        pool[0] = (ushort)ModContent.TileType<ThickSnowTile>();
        pool[1] = TileID.SnowBlock;
        pool[2] = TileID.IceBlock;

        int numAbyssBlotchSteps = 150;
        for (int i = 0; i < 3; i++)
        {
            ushort tileType = pool[i];
            for (int n = 0; n < numAbyssBlotchSteps; n++)
            {
                //Get a random center point to place the blotch
                Point p = new();
                p.X = genRand.Next(GenVars.snowOriginLeft, GenVars.snowOriginRight);
                p.Y = genRand.Next(abyssHigh, abyssLow);

                float strength = genRand.NextFloat(8, 16);
                int steps = genRand.Next(10, 20);
                WorldGen.OreRunner(p.X, p.Y, strength, steps, tileType);
            }
        }

        /*
        //Create long caves
        void CreateCave(Vector2 originPoint, in Vector2 initialVelocity)
        {
            //The way this cave style will work, is it will start form the origin point
            //and it will go until it hits the edge of the biome or if it['s traveled enoiugh steps
            //After each segment it generates, it randomizes the velocity again in 30 degree angles from the starting direction
            //Which should create nice little lines/caverns
            Vector2 cavernPoint = originPoint;
            int failsafe = 0;
            while(cavernPoint.X < GenVars.snowOriginRight && failsafe < 300)
            {
                int remainingSteps = 32;
                Vector2 velocity = initialVelocity.RotatedBy(genRand.NextFloat(-MathHelper.PiOver4 * 0.5f, MathHelper.PiOver4 * 0.5f));
                while (remainingSteps > 0)
                {
                    cavernPoint += velocity * 12f;
                    if(cavernPoint.X < GenVars.snowOriginRight)
                    {

                        //Cut away at the terrain
                        WorldGen.TileRunner((int)cavernPoint.X, (int)cavernPoint.Y,
                            strength: 24,
                            genRand.Next(7, 25), -1);
                    }

                    remainingSteps--;
                }
                failsafe++;
            }
        }

        //Sprinkle several long caves throughout the biome
        int numCaves = 32;
        for(int n = 0; n < numCaves; n++)
        {
            Vector2 p = new Vector2();
            p.X = genRand.Next(GenVars.snowOriginLeft - 25, GenVars.snowOriginLeft + 25);
            p.Y = genRand.Next(abyssHigh, abyssLow);

            //All caves should be moving to the right
            Vector2 initialDirection = Vector2.UnitX;
            initialDirection = initialDirection.RotatedBy(genRand.NextFloat(-0.2f, 0.2f));
            CreateCave(p, initialDirection);
        }*/


        //Let's try an implementation with fast noise lite
        FastNoiseLite fnl = new();
        fnl.SetSeed(genRand.Next(0, 20000));
        fnl.SetFrequency(0.005f);
        fnl.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
        fnl.SetDomainWarpAmp(65);

        for (int x = GenVars.snowOriginLeft; x < GenVars.snowOriginRight; x++)
        {
            for (int y = abyssHigh; y < abyssLow; y++)
            {
                float noise = fnl.GetNoise(x, y);
                if (noise < 0.5f)
                {
                    Tile tile = Main.tile[x, y];
                    tile.ClearTile();
                }
            }
        }
    }
}

public class AurelusTemplePass : GenPass
{
    public AurelusTemplePass() : base("Aurelus Temple", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        /*Rectangle rectangle = StructureLoader.ReadRectangle("Structures/Aurelus/AurelusTemple");
        progress.Message = "Singularities Singing!";
        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 1000000)
        {
            Point Loc = AbyssCenter;
            Loc.X -= rectangle.Width / 2;
            Loc.Y += rectangle.Height / 2;
            rectangle.Location = Loc;
            StructureLoader.ProtectStructure(Loc, "Structures/Aurelus/AurelusTemple");
            placed = true;
        }*/
    }
}