using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Projectiles.Slashers.GrailAxe;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class GrailAxe : ModItem
    {
        public int AttackCounter = 1;
        public int combowombo = 0;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");

            line = new TooltipLine(Mod, "GrailAxe", "(A+) Strikes enemies with some straight ass star power!")
            {
                OverrideColor = new Color(255, 223, 82)

            };
            tooltips.Add(line);



        }
        public override void SetDefaults()
        {
            Item.damage = 45;
            Item.DamageType = DamageClass.Generic;
            Item.width = 0;
            Item.height = 0;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 12;
            Item.value = 10000;
            Item.noMelee = true;

            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/StarSheith");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GrailAxeProj>();
            Item.shootSpeed = 20f;
            Item.noUseGraphic = true;
            Item.value = Item.sellPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<GrailAxeProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int dir = AttackCounter;
            AttackCounter = -AttackCounter;
            Projectile.NewProjectile(source, position, velocity, type, damage * 3, knockback, player.whoAmI, 1, dir);



            float numberProjectiles2 = 4;
            float rotation = MathHelper.ToRadians(20);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles2; i++)
            {

                Vector2 perturbedSpeed2 = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles2 - 1))) * 1f;// This defines the projectile roatation and speed. .4f == projectile speed

                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed2.X, perturbedSpeed2.Y, ModContent.ProjectileType<GrailShot>(), damage, Item.knockBack, player.whoAmI);
            }
            Projectile.NewProjectile(source, position, velocity * 1f, ModContent.ProjectileType<GrailAxeProj>(), damage * 1, knockback, player.whoAmI, 1, dir);

            return false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MinersGold, BlankSword>();
        }
    }
}