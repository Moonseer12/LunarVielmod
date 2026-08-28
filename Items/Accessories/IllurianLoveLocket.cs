using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class IllurianLoveLocket : ModItem
    {
        private float _starTimer;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            _starTimer--;
            if (_starTimer <= 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<IllurianLoveLocketStarProj>(), 150, 1, player.whoAmI);
                _starTimer = 10;
            }
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<IllurineScale, BlankAccessory>();
        }
    }
}
