using Microsoft.Xna.Framework;
using Stellamod.Content.MoonlightMagic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Stellamod.Content.Chests
{
    public class MythicalChest : BaseChest
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            ChestColor = new Color(210, 33, 221);
        }
        public override void AI()
        {
            base.AI();
            if (Timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ShadowExplosion"), NPC.position);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

            base.ModifyNPCLoot(npcLoot);
            int[] enchantmentTypes = BaseEnchantment.GetNonSpecialTypes();
            npcLoot.Add(ItemDropRule.FewFromOptions(amount: 3, chanceDenominator: 1, enchantmentTypes));

        }
    }
}
