using Stellamod.Content.MoonlightMagic.Elements;
using Terraria.ModLoader;

namespace Stellamod.Content.MoonlightMagic.Enchantments.Guuts
{
    public class KillerEnchantment : BaseEnchantment
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 30;
        }

        public override void AI()
        {
            base.AI();
            Countertimer++;
            if (Countertimer == time)
                Projectile.Kill();
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<GuutElement>();
        }

    }
}
