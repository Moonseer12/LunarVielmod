using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.GunSwapping
{
    public class MeredaX : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 86;
            LeftHand = true;
            RightHand = true;
            TwoHands = true;

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunBlasting");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //Higher is faster
            AttackSpeed = 5;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (player.HeldItem.ModItem is not GunHolster gunHolster)
                return;

            if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
            {
                if (gunHolster.HeldLeftHandGun == this)
                {
                    //Treat this like a normal shoot function
                    float spread = 0.4f;
                    for (int k = 0; k < 7; k++)
                    {
                        Vector2 newDirection = velocity.RotatedByRandom(spread);
                        Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Black, Main.rand.NextFloat(0.2f, 0.5f));
                    }
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Black, 1);
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, ModContent.ProjectileType<PINKX>(), damage, knockback, player.whoAmI);
                    }
                    SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunBlasting");
                    soundStyle.PitchVariance = 0.5f;
                    SoundEngine.PlaySound(soundStyle);
                }
                else
                {
                    float spread = 0.4f;
                    for (int k = 0; k < 7; k++)
                    {
                        Vector2 newDirection = velocity.RotatedByRandom(spread);
                        Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Black, Main.rand.NextFloat(0.2f, 0.5f));
                    }
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.Black, 1);
                    if (Main.myPlayer == player.whoAmI)
                    {
                        Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, ModContent.ProjectileType<BLACKX>(), damage, knockBack, player.whoAmI);
                    }

                    SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunBlasting");
                    soundStyle.PitchVariance = 0.5f;
                    SoundEngine.PlaySound(soundStyle);
                }
            }
        }
    }
}