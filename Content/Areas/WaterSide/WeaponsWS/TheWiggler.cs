using Stellamod.Assets;
using Stellamod.Content.Areas.Fable.WeaponsFB;
using Stellamod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.WeaponsWS
{
    public class TheWiggler : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<WigglerShot>();
            Item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {

                Projectile.NewProjectileDirect(source, position, velocity, ModContent.ProjectileType<WigglerDetonator>(), damage, knockback, player.whoAmI);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/clickk"));
                return false;
            }

            //Shooting Sound
            string soundPath;
            switch (Main.rand.Next(0, 3))
            {
                default:
                case 0:
                    soundPath = "Stellamod/Assets/Sounds/WigglerShot";
                    break;
                case 1:
                    soundPath = "Stellamod/Assets/Sounds/WigglerShot2";
                    break;
                case 2:
                    soundPath = "Stellamod/Assets/Sounds/WigglerShot3";
                    break;
            }

            SoundStyle soundStyle = new SoundStyle(soundPath) with { PitchVariance = 0.1f };
            SoundEngine.PlaySound(soundStyle, position);

            int numProjectiles = Main.rand.Next(0, 2);
            for (int p = 0; p < numProjectiles; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            return true;
        }
    }

    public class WigglerShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 22;
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
            if (Main.rand.NextBool(60))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemSapphire);
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Blue * 0.25f, Color.Transparent, completionRatio);
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
                ModContent.ProjectileType<WigglerStick>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: targetNpc);
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + oldVelocity, Projectile.velocity,
              ModContent.ProjectileType<WigglerStick2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft)
        {
            for (float f = 0; f < 6; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
        }
    }

    public class WigglerStick : ModProjectile
    {
        private float _lighting;
        private bool _setOffset;
        private Vector2 _offset;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
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
                    ModContent.ProjectileType<WigglerShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.Kill();
            }
            else
            {
                Vector2 targetPos = target.position - _offset + new Vector2(0.001f, 0.001f);
                Vector2 directionToTarget = Projectile.position.DirectionTo(targetPos);
                float dist = Vector2.Distance(Projectile.position, targetPos);
                Projectile.velocity = (directionToTarget * dist) + new Vector2(0.001f, 0.001f);
            }

            bool detonate = Projectile.ai[2] == 1;
            if (detonate)
            {
                ref float detonationTimer = ref Projectile.ai[1];
                detonationTimer--;
                if (detonationTimer == 10)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"), Projectile.position);
                    ExplodeEffects();
                }

                if (detonationTimer < 0)
                {
                    ShakeScreenPosition.Shake = 3;
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"), Projectile.position);
                    Boom();
                    Projectile.Kill();
                }

            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Boom()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                int projType;
                switch (Main.rand.Next(3))
                {
                    default:
                    case 0:
                        projType = ModContent.ProjectileType<SparklyBoom>();
                        break;
                    case 1:
                        projType = ModContent.ProjectileType<BongoBoom>();
                        break;
                    case 2:
                        projType = ModContent.ProjectileType<SparklyBoom>();
                        break;
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, projType, Projectile.damage * 3, Projectile.knockBack, Projectile.owner);
            }
        }
        private void ExplodeEffects()
        {
            for (float f = 0; f < 12; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Blue,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(12, 24f));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);
        }

        public override void PostDraw(Color lightColor)
        {
            bool detonate = Projectile.ai[2] == 1;
            if (detonate)
            {
                _lighting += 0.01f;
                Lighting.AddLight(Main.screenPosition - Projectile.position, Color.White.ToVector3() * _lighting * Main.essScale); ;

            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemSapphire, speed * 4);
                d.noGravity = true;
            }
        }
    }

    public class WigglerStick2 : ModProjectile
    {
        private float _lighting;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
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

            bool detonate = Projectile.ai[2] == 1;
            if (detonate)
            {
                ref float detonationTimer = ref Projectile.ai[1];
                detonationTimer--;

                if (detonationTimer == 10)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CombusterReady"), Projectile.position);
                    ExplodeEffects();
                }

                if (detonationTimer < 0)
                {
                    ShakeScreenPosition.Shake = 3;
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Kaboom"), Projectile.position);

                    Boom();

                    Projectile.Kill();
                }

            }

            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Boom()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                int projType;
                switch (Main.rand.Next(3))
                {
                    default:
                    case 0:
                        projType = ModContent.ProjectileType<SparklyBoom>();
                        break;
                    case 1:
                        projType = ModContent.ProjectileType<BongoBoom>();
                        break;
                    case 2:
                        projType = ModContent.ProjectileType<SparklyBoom>();
                        break;
                }
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, projType, Projectile.damage * 3, Projectile.knockBack, Projectile.owner);
            }
        }
        private void ExplodeEffects()
        {
            for (float f = 0; f < 12; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Blue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Blue,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(12, 24f));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }


        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);
        }

        public override void PostDraw(Color lightColor)
        {
            bool detonate = Projectile.ai[2] == 1;
            if (detonate)
            {
                _lighting += 0.01f;
                Lighting.AddLight(Main.screenPosition - Projectile.position, Color.White.ToVector3() * _lighting * Main.essScale); ;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemSapphire, speed * 4);
                d.noGravity = true;
            }
        }
    }

    public class WigglerDetonator : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            foreach (var p in Main.ActiveProjectiles)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    if (p.type == ModContent.ProjectileType<WigglerStick>() ||
                            p.type == ModContent.ProjectileType<WigglerStick2>())
                    {
                        if (p.ai[2] == 0)
                        {
                            p.ai[1] = Main.rand.Next(30, 90);
                            p.ai[2] = 1;
                            p.netUpdate = true;
                        }
                    }
                }
            }
            Projectile.Kill();
        }
    }
}