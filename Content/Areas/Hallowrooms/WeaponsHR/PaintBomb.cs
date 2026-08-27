using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Hallowrooms.WeaponsHR
{
    public class PaintBomb1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 47;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 82;
            Projectile.height = 73;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 47;
            Projectile.scale = 1.4f;
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 47)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }

    public class PaintBomb2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 27;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 68;
            Projectile.height = 80;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 27;
            Projectile.scale = 1.3f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 27)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }

    public class PaintBomb3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 27;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 69;
            Projectile.height = 58;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 27;
            Projectile.scale = 1f;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }


        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 27)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }

    public class PaintBomb4 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 26;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 68;
            Projectile.height = 80;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 52;
            Projectile.scale = 1.3f;
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 26)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }

    public class PaintBomb5 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 27;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 68;
            Projectile.height = 80;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 27;
            Projectile.scale = 1.3f;
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 27)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }

    public class PaintBomb6 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 28;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 68;
            Projectile.height = 80;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 56;
            Projectile.scale = 1.5f;
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 28)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }

    public class PaintBomb7 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 28;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 68;
            Projectile.height = 80;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 56;
            Projectile.scale = 1.5f;
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 28)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }
    
    public class PaintBomb8 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 27;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 68;
            Projectile.height = 80;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 54;
            Projectile.scale = 1.5f;
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 27)
                {
                    Projectile.frame = 0;
                }
            }
            return true;
        }
    }
}