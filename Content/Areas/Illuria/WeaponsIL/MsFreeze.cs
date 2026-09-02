using Stellamod.Common.MagicCauldron;
using Stellamod.Content.Areas.Tundra.Snow.WeaponsSN;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.GunSwapping;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.WeaponsIL
{
    public class MsFreeze : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 14;
            LeftHand = true;

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunLaser");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //This number is in ticks
            AttackSpeed = 2;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 4; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.LightCyan, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.White, 1);
            for (int i = 0; i < Main.rand.Next(2, 5); i++)
            {
                Vector2 vel = velocity * 16;
                vel = vel.RotatedByRandom(MathHelper.PiOver4 / 3);
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity,
                    ModContent.ProjectileType<MintyBlastProj>(), damage, knockback, player.whoAmI);
                }
            }

            FXUtil.ShakeCamera(position, 1024f, 2f);

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunLaser");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<IllurineScale, BlankGun>();
        }
    }
}