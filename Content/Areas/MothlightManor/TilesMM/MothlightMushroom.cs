using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MothlightManor.TilesMM
{
    public class MothlightMushroom : ModTile
    {

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(178, 163, 160), name);

            RegisterItemDrop(ModContent.ItemType<MothlightMushroomBlock>());
            // DustType = Main.rand.Next(110, 113);

            //MineResist = 1f;
            //MinPick = 25;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);

            if (!tileAbove.HasTile || !tileBelow.HasTile)
            {
                r = 0.05f;
                g = 0.15f;
                b = 0.25f;
            }
        }








        public class MothlightMushroomBlock : ModItem
        {
            public override void SetDefaults()
            {
                Item.DefaultToPlaceableTile(ModContent.TileType<MothlightMushroom>());
            }

            // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        }
    }
}