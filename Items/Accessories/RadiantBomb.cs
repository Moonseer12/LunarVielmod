using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    public class RadiantBomb : ModProjectile
    {
        int afterImgCancelDrawCount = 0;
        float ta = 0;
        float TimerR = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Charm Spragald");
            Main.projFrames[Projectile.type] = 60;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 106;
            Projectile.height = 106;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = int.MaxValue;


        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            bool hasSetBonus = player.GetModPlayer<MyPlayer>().RadiantBomb;
            if (!hasSetBonus)
            {
                Projectile.Kill();
                return;
            }


            Timer++;
            Projectile.rotation += 0.05f;
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                swordRotation = (Main.MouseWorld - player.Center).ToRotation();
            }

            Projectile.velocity = swordRotation.ToRotationVector2();
            Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position			
            Projectile.tileCollide = false;
            if (ta > 150)
            {
                afterImgCancelDrawCount++;
            }

            ta += 0.01f;
            TimerR++;
            if (TimerR == 100)
            {
                TimerR = 0;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        int counter = 1;
        float alphaCounter = 1;
        public override bool PreDraw(ref Color lightColor)
        {


            // just return false if you want only trail locked on player
            return true;
        }


        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 60)
                {
                    Projectile.frame = 0;
                }
            }
            return true;


        }
    }
}