

using Stellamod.Content.MoonlightMagic.Elements;

using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.MoonlightMagic.Enchantments.Nature
{
    public class ReverserOfNatureEnchantment : BaseEnchantment
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            time = 120;
        }

        public override void AI()
        {
            base.AI();

            //Count up
            Countertimer++;
            if (Countertimer >= time)
            {

                //If greater than time then start homing, we'll just swap the movement type of the projectile

                foreach (var enchantment in MagicProj.Enchantments)
                {
                    //do a thing here
                    if (enchantment.Countertimer > enchantment.time)
                    {
                        enchantment.Countertimer = 0;
                    }

                }

            }
        }

        public override float GetStaffManaModifier()
        {
            return 1f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<NaturalElement>();
        }
    }
}
