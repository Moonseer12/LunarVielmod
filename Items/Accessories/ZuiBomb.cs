
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    // Load the spritesheet you create as a shield for the player when it is equipped.
    public class ZuiBomb : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }




        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetModPlayer<MyPlayer>().RadiantBombCooldown--;
            player.GetModPlayer<MyPlayer>().RadiantBomb = true;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<RadiantBomb>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<RadiantBomb>(), 10, 4, player.whoAmI);
            }
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }

}