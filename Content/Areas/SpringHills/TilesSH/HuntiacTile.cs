using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.TilesSH
{
    public class HuntiacTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine2[Type] = true;
            Main.tileShine[Type] = 900;
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(36, 31, 27));
            DustType = DustID.Web;
            HitSound = SoundID.Grass;
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }

    public class HuntiacBlock : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HuntiacTile>());
        }
    }
}