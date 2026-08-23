using Stellamod.Content.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.MoonlightMagic.Enchantments.Deeya
{
    public class GravityDownEnchantment : BaseEnchantment
    {
        public override float GetStaffManaModifier()
        {
            return 0.2f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<DeeyaElement>();
        }

        public override void AI()
        {
            base.AI();
            float maxSpeed = 15;
            if (Projectile.velocity.Y < maxSpeed)
            {
                Projectile.velocity.Y += 0.4f;
            }
        }
    }
}
