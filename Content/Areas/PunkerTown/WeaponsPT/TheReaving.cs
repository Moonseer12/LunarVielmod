using Stellamod.Common;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.GunSwapping;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.WeaponsPT
{
    public class TheReaving : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 136;
            LeftHand = true;

            //This number is in ticks
            AttackSpeed = 120;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
            {
                //Treat this like a normal shoot function
                float spread = 0.4f;
                for (int k = 0; k < 14; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.LightGoldenrodYellow, Main.rand.NextFloat(0.4f, 0.8f));
                }

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, projToShoot, damage, knockback, player.whoAmI);
                }
                player.GetModPlayer<ShakePlayer>().ShakeAtPosition(position, 1024f, 16f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);


                float rot = velocity.ToRotation();
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"), position);

                Vector2 offset = new Vector2(2, -0.1f * player.direction).RotatedBy(rot);
                for (int k = 0; k < 15; k++)
                {
                    Vector2 direction2 = offset.RotatedByRandom(spread);

                    Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction2 * Main.rand.NextFloat(8), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 0.5f));
                }


                int numProjectiles = Main.rand.Next(10, 30);
                for (int p = 0; p < numProjectiles; p++)
                {


                    Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
                    Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));



                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(25));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);


                    Projectile.NewProjectile(player.GetSource_FromThis(), position, newVelocity * 12, projToShoot, damage, knockback, player.whoAmI);
                }
            }
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MarshScrap, BlankGun>();
        }
    }
}