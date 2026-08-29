using Stellamod.Assets;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.GunSwapping;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC
{
    public class BubbleBussy : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 50;
            RightHand = true;

            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/TentacleBubbleOut");
            soundStyle.PitchVariance = 0.5f;
            Item.UseSound = soundStyle;

            //This number is in ticks
            AttackSpeed = 8;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            float spread = 0.4f;
            for (int k = 0; k < 14; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.DarkBlue, Main.rand.NextFloat(0.4f, 0.8f));
            }

            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkBlue, 1);
            Vector2 vel = velocity * 12;
            vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                ModContent.ProjectileType<BubbleBussyProj>(), damage, knockback, player.whoAmI);
            }
            SoundStyle soundStyle = new("Stellamod/Assets/Sounds/TentacleBubbleOut");
            soundStyle.PitchVariance = 0.5f;
            SoundEngine.PlaySound(soundStyle, position);
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcaricMush, BlankGun>();
        }
    }

    public class BubbleBussyProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 20;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.4f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool(12))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BoneTorch);
                }

                Main.rand.NextVector2Circular(0.5f, 0.5f);
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black * 0.25f, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            return base.PreDraw(ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int targetNpc = target.whoAmI;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                ModContent.ProjectileType<BubbleBussyStickyProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: targetNpc);
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position + oldVelocity, Projectile.velocity,
              ModContent.ProjectileType<BubbleBussyStickyProj2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed * 4);
                d.noGravity = true;
            }
        }
    }

    public class BubbleBussyStickyProj : ModProjectile
    {
        private bool _setOffset;
        private Vector2 _offset;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60 * 10;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;
        }

        public override void AI()
        {
            int targetNpc = (int)Projectile.ai[0];
            NPC target = Main.npc[targetNpc];

            if (target.active && !_setOffset)
            {
                _offset = target.position - Projectile.position + new Vector2(0.001f, 0.001f);
                _setOffset = true;
            }
            else if (!target.active)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                    ModContent.ProjectileType<BubbleBussyProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.Kill();
            }
            else
            {
                Vector2 targetPos = target.position - _offset + new Vector2(0.001f, 0.001f);
                Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
                float dist = Vector2.Distance(Projectile.position, targetPos);
                Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);

        }


        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed * 4);
                d.noGravity = true;
            }
        }
    }

    public class BubbleBussyStickyProj2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60 * 10;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            if (Main.rand.NextBool(100))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BoneTorch);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed * 4);
                d.noGravity = true;
            }
        }
    }
}