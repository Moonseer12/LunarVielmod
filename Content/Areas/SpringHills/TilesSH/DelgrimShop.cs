using Stellamod.Core.DecorativeTileSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    //Wall Version
    public class DelgrimShopItem : DecorativeWallItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<DelgrimShop>();
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D iconTexture = ModContent.Request<Texture2D>("Stellamod/Tiles/ExampleDecorativeWallItem").Value;
            Vector2 drawOrigin = Vector2.Zero;
            Vector2 drawPosition = position + drawOrigin;
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
        }

    }

    public class DelgrimShop : DecorativeWall
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            StructureColor = Color.Gray;
            Origin = DrawOrigin.BottomUp;

            //If you need other static defaults it go here
        }
    }



}
