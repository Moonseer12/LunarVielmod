using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.TilesMT;

public class CathediteTile : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = true;
        Main.tileMergeDirt[Type] = true;
        Main.tileBlockLight[Type] = true;
        DustType = Main.rand.Next(110, 113);
        AddMapEntry(new Color(2, 14, 26));
    }
    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }
    public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
    {
        base.PostDraw(i, j, spriteBatch);
        Vector2 pos = new Vector2(i, j) * 16;
        pos += new Vector2(Main.offScreenRange);
        Tile tile = Framing.GetTileSafely(i, j);
        Rectangle frame = new(tile.TileFrameX + 288, tile.TileFrameY, 16, 16);
        Color glowColor = Color.Lerp(Color.Pink, Color.Blue, ExtraMath.Osc(0f, 1f));
        glowColor.A = 0;
        spriteBatch.Draw(TextureAssets.Tile[Type].Value, pos - Main.screenPosition, frame, glowColor, 0, Vector2.Zero, 1, 0, 1);
    }
}

public class CathediteBlock : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<CathediteTile>();
    }
}