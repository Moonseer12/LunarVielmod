using Stellamod.Common.ArmorRework;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.AccWS
{
    public class OceanScroll : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetStats().artifactManaReduction += 0.2f;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MusicalHarmonise, BlankAccessory>();
        }
    }
}

