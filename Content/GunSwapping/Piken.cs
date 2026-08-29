using Stellamod.Common;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.GunSwapping
{
    public class Piken : MiniGun
    {
        private int _comboCounter;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 50;
            LeftHand = true;

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunShootNew1");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //This number is in ticks
            AttackSpeed = 30;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (!player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
                return;
            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);

            _comboCounter++;
            if (_comboCounter > 100)
            {
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.IndianRed, Main.rand.NextFloat(0.2f, 0.8f));
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol2"));
                AttackSpeed = 30;
                _comboCounter = 0;
            }
            if (_comboCounter > 75)
            {
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.TSmokeDust>(), new Vector2(0, 0) + offset.RotatedByRandom(spread), 150, Color.IndianRed * 0.5f, Main.rand.NextFloat(0.5f, 1));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.White, 1);
            if (AttackSpeed > 2)
            {
                AttackSpeed--;

            }

            for (int p = 0; p < 1; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 14, projToShoot, damage, knockback, player.whoAmI);
                }
            }

            Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(player.Center, 1024f, 8f);
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GunShootNew6"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GunShootNew6"));
            }

            //Dust Burst Towards Mouse


            for (int k = 0; k < 7; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);


                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(180, 50, 40), Main.rand.NextFloat(0.2f, 0.5f));
            }

            Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
            Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
        }
    }
}