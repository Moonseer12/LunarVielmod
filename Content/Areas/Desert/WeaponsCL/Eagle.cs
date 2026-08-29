using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.GunSwapping;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Desert.WeaponsCL
{
    public class Eagle : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();

            //Setting this to width and height of the texture cause idk
            Item.damage = 9;
            Item.width = 56;
            Item.height = 30;

            LeftHand = true;

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/MiniPistol3");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;


            //Higher is faster
            AttackSpeed = 12;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(15, -6);

            //Recoil
            RecoilDistance = 3;
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            if (player.PickAmmo(Item, out int projToShoot, out float speed, out int newDamage, out float knockBack, out int usedAmmoItemId))
            {
                //Treat this like a normal shoot function
                float spread = 0.4f;
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newDirection = velocity.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
                }
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
                if (Main.myPlayer == player.whoAmI)
                    Projectile.NewProjectile(player.GetSource_FromThis(), position, velocity * 8, projToShoot, damage, knockback, player.whoAmI);
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"), position);
                }
                else
                {
                    SoundStyle soundStyle = new("Stellamod/Assets/Sounds/MiniPistol3");
                    soundStyle.PitchVariance = 0.5f;
                    SoundEngine.PlaySound(soundStyle, position);
                }
            }
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<GintzlMetal, BlankGun>();
        }
    }
}