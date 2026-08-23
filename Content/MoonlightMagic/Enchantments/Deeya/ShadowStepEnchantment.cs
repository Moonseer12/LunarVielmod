using Microsoft.Xna.Framework;
using Stellamod.Content.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.MoonlightMagic.Enchantments.Deeya
{
    public class ShadowStepEnchantment : BaseEnchantment
    {
        public override float GetStaffManaModifier()
        {
            return 0.12f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<DeeyaElement>();
        }

        public override void SetMagicDefaults()
        {
            float range = MagicProj.Size * 8;
            Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 / 2);
            Projectile.position.X += Main.rand.NextFloat(-range, range);
            Projectile.position.Y += Main.rand.NextFloat(-range, range);
        }
    }
}
