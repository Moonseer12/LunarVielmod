using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.GunSwapping
{
    public class ShottyPitol : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 9;
            RightHand = true;

            //This number is in ticks
            AttackSpeed = 60;

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
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.White, Main.rand.NextFloat(0.4f, 0.8f));
                }

                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.White, 1);
                for (int i = 0; i < Main.rand.Next(3, 7); i++)
                {
                    Vector2 vel = velocity * 16;
                    vel = vel.RotatedByRandom(MathHelper.PiOver4 / 2);
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                        projToShoot, damage, knockback, player.whoAmI);
                    }
                }

                FXUtil.ShakeCamera(position, 1024f, 16f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);
            }
        }
    }
}