using Stellamod.Content.MoonlightMagic.Elements;

using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.MoonlightMagic.Enchantments.Nature
{
    public class BoomerangEnchantment : BaseEnchantment
    {

        public override int GetElementType()
        {
            return ModContent.ItemType<NaturalElement>();
        }

        public override void AI()
        {
            base.AI();
            Countertimer++;
            if(Countertimer > 60)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, Owner.Center, degreesToRotate: 7);
            }
  
        }
    }
}
