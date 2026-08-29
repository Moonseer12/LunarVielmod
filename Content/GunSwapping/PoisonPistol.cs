using Stellamod.Common;
using Stellamod.Projectiles.Steins;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.GunSwapping
{
    public class PoisonPistol : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 70;
            RightHand = true;

            //This number is in ticks
            AttackSpeed = 60;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 14; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Purple, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkViolet, 1);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 16,
                ModContent.ProjectileType<SSShot>(), damage, knockback, player.whoAmI);
            }

            player.GetModPlayer<ShakePlayer>().ShakeAtPosition(position, 1024f, 5f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/gun1"), position);
        }
    }
}