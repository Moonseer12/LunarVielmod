using Stellamod.Common;
using Stellamod.Content.Gores;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.GunSwapping
{
    public class BasterParty : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 120;
            RightHand = true;
            LeftHand = true;
            TwoHands = true;

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/TentacleBubbleOut");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //This number is in ticks
            AttackSpeed = 5;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/GunShootNew11");
            soundStyle.PitchVariance = 0.3f;
            soundStyle.Volume = 0.8f;
            SoundEngine.PlaySound(soundStyle, position);

            float rot = velocity.ToRotation();
            float spread = 0.4f;
            Vector2 offset = new Vector2(2, -0.1f * player.direction).RotatedBy(rot);
            Vector2 newDirection = velocity.RotatedByRandom(spread);

            //Funny Screenshake
            Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(player.Center, 1024f, 5f);
            int numProjectiles = Main.rand.Next(1, 3);
            float distance = 12;
            for (int p = 0; p < numProjectiles; p++)
            {
                //Particles and stuff
                Dust.NewDustPerfect(position + offset * distance, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
                Dust.NewDustPerfect(player.Center + offset * distance, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));

                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                Projectile.NewProjectileDirect(player.GetSource_FromThis(), position, newDirection * 2 * Main.rand.NextFloat(12), ModContent.ProjectileType<BasterPartyProj>(), damage, knockback, player.whoAmI);
                for (int k = 0; k < Main.rand.Next(1, 3); k++)
                {
                    int[] goreTypes = [
                        ModContent.GoreType<RibbonBlue>(),
                        ModContent.GoreType<RibbonPink>(),
                        ModContent.GoreType<RibbonWhite>(),
                        ModContent.GoreType<RibbonYellow>()
                    ];

                    int goreType = goreTypes[Main.rand.Next(0, goreTypes.Length)];
                    Gore.NewGore(player.GetSource_FromThis(), position + offset.RotatedByRandom(MathHelper.PiOver4) * distance * Main.rand.NextFloat(0.5f, 1f),
                        newVelocity.RotatedByRandom(MathHelper.PiOver4),
                      goreType);
                }
            }
        }
    }

    public class BasterPartyProj : ModProjectile
    {
        private int _color;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                _color = Main.rand.Next(4);
            }

            Projectile.velocity.Y += 0.2f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            int goreType;
            switch (_color)
            {
                default:
                case 0:
                    goreType = ModContent.GoreType<RibbonBlue>();
                    break;
                case 1:
                    goreType = ModContent.GoreType<RibbonPink>();
                    break;
                case 2:
                    goreType = ModContent.GoreType<RibbonYellow>();
                    break;
                case 3:
                    goreType = ModContent.GoreType<RibbonWhite>();
                    break;
            }
            for (int i = 0; i < 1; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                  goreType);
            }
        }
    }
}