using Stellamod.Common.DungeonGeneration;
using Stellamod.Content.Areas.MothlightManor.TilesMM;
using Stellamod.WorldG;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Content.Areas.Dungeon;

public class MistyDungeonHillPass : GenPass
{
    public MistyDungeonHillPass() : base("Misty Dungeon Hill Terrain", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "A Mysterious Hill...";

        //Calculate the starting location
        Point startHillTile = ModContent.GetInstance<VeilGen>().FableFarEdgeLocation;
        startHillTile.X += 150;
        startHillTile.Y -= 200;
        startHillTile = TileUtilities.FallToSolidTile(startHillTile.X, startHillTile.Y);
        startHillTile.Y += 36;
        ModContent.GetInstance<VeilGen>().MistyHillStartLocation = startHillTile;

        //Calculate the ending location
        Point endHillTile = startHillTile;
        endHillTile.X += 2200;
        endHillTile.Y -= 200;
        endHillTile = TileUtilities.FallToSolidTile(endHillTile.X, endHillTile.Y);
        endHillTile.Y += 10;
        ModContent.GetInstance<VeilGen>().MistyHillEndLocation = endHillTile;

        float hillHeight = 350;
        float width = endHillTile.X - startHillTile.X;
        for (int x = startHillTile.X; x < endHillTile.X; x++)
        {
            float ratio = (x - startHillTile.X) / width;
            float height = (int)(VeilGen.GetFableHillHeight(ratio) * hillHeight);
            for (int y = 0; y < height; y++)
            {
                WorldGen.PlaceTile(x, startHillTile.Y - y, TileID.Dirt);
            }
        }

        //Place the fable
        Point placementTile = new();
        placementTile.X = (int)MathHelper.Lerp(startHillTile.X, endHillTile.X, 0.65f);
        placementTile.Y = (int)(Main.worldSurface - 400);
        placementTile = TileUtilities.FallToSolidTile(placementTile.X, placementTile.Y);
        ModContent.GetInstance<VeilGen>().MistyDungeonLocation = placementTile;
    }
}

public class MistyDungeonPass : GenPass
{
    public MistyDungeonPass() : base("Misty Dungeon", 449.3721923828125)
    {
    }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Mistying the Dungeon";
        Room[] prefabs = DungeonSaveUtility.GetDungeonPrefabs("Dungeon");


        int dungeonLayoutCount = 1;
        string path = $"MistyDungeon_{WorldGen.genRand.Next(dungeonLayoutCount) + 1}";
        GenerationPrefab prefab = ModContent.GetInstance<GenerationTextureManager>().GetPrefab(path);
        DungeonChart chart = DungeonChart.FromPrefab(prefab);
        Room[] map = Dungeonizer.GenerateFromChart(prefabs, chart, WorldGen.genRand);
        int[] tileBlend = [
            TileID.RubyGemspark
        ];
        Point topLeft = Point.Zero;
        Point bottomRight = Point.Zero;
        for (int r = 0; r < map.Length; r++)
        {
            Room room = map[r];
            if (topLeft.X > room.bounds.Left)
                topLeft.X = room.bounds.Left;
            if (topLeft.Y > room.bounds.Top)
                topLeft.Y = room.bounds.Top;

            if (bottomRight.X < room.bounds.Right)
                bottomRight.X = room.bounds.Right;
            if (bottomRight.Y < room.bounds.Bottom)
                bottomRight.Y = room.bounds.Bottom;
        }
        Rectangle rectangle = new(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);

        //Look for a spot to place it
        //We're placing it on the right side of the world ig
        bool placed = false;
        int attempts = 0;
        while (!placed && attempts++ < 10000000)
        {
            Point point = ModContent.GetInstance<VeilGen>().MistyDungeonLocation;
            Point vectorToOrigin = point - rectangle.Top().ToPoint();
            rectangle.Location += vectorToOrigin;

            //Just a failsafe
            while (rectangle.Right().X >= Main.maxTilesX)
                rectangle.Location -= new Point(32, 0);

            //Override dungeon variables
            GenVars.dungeonLocation = point.X;
            GenVars.dungeonX = point.X;
            GenVars.dungeonY = point.Y;

            //The first room is the starting room, we don't want to outline that one
            //So we're just gonna start from index 1 to skip it
            for (int r = 1; r < map.Length; r++)
            {
                Room room = map[r];
                int padding = 80;
                Rectangle roomRectangle = Structurizer.ReadRectangle(room.prefab);
                int outlineWidth = roomRectangle.Width + padding;
                int outlineHeight = roomRectangle.Height + padding;

                //This hsould give us an outline of bricks, I think
                Point topLeftRoom = room.bounds.TopLeft().ToPoint() + new Point(-padding / 2, -padding / 2);
                Point offset = rectangle.Top().ToPoint();
                topLeftRoom.Y -= map[0].bounds.Height;
            
                topLeftRoom += offset;
                WorldUtils.Gen(topLeftRoom, new Shapes.Rectangle(outlineWidth, outlineHeight),
                   Actions.Chain(
                        new Actions.ClearWall(),
                        new Actions.SetTile((ushort)ModContent.TileType<MothlightBrick>()))
                   );
            }

            for (int r = 0; r < map.Length; r++)
            {
                Room room = map[r];
                Point bottomLeft = room.bounds.BottomLeft().ToPoint();
                Point offset = rectangle.Top().ToPoint();

                int tileX = offset.X;
                int tileY = offset.Y;

                bottomLeft.X += tileX;
                bottomLeft.Y += tileY;
                bottomLeft.Y -= map[0].bounds.Height;
                Structurizer.ReadStruct(bottomLeft, room.prefab, tileBlend);
                if (r == 0)
                {
                    Rectangle rect = Structurizer.ReadRectangle(room.prefab);
                    rect.Location = bottomLeft;
                    Point start = bottomLeft;
                    for (int x = start.X; x < start.X + rect.Width; x++)
                    {
                        Point downPoint = new(x, start.Y + 1);
                        for (int y = 0; y < 50; y++)
                        {
                            Tile tile = Main.tile[downPoint];
                            //Checking for walls cause we don't wanna break the inside of the dungeon
                            if (tile.WallType == WallID.None)
                            {
                                tile.ClearEverything();
                                tile.TileType = TileID.Dirt;
                                tile.HasTile = true;
                                tile.TileFrameX = -1;
                                tile.TileFrameY = -1;

                            }
                            downPoint.Y++;
                        }
                    }
                }
                Structurizer.ProtectStructure(bottomLeft, room.prefab);
            }
            placed = true;
        }
    }
}