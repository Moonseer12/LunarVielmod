using Stellamod.Content.Areas.Terror.WeaponsTR;
using Stellamod.Content.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Junkyard.WeaponsJY
{
    public class GlocketRouncher : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 350;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 72;
            Item.useAnimation = 72;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GlocketRouncher");
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<GlocketRouncherProj>();
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            FXUtil.ShakeCamera(player.Center, 1024f, 32f);

            //Dust Burst Towards Mouse

            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(1, -0.1f * player.direction).RotatedBy(rot);
            for (int k = 0; k < 15; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);

                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
            Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
            return base.Shoot(player, source, position, velocity, type, damage, knockback);

        }
    }

    
    public class GlocketRouncherProj : ModProjectile
    {
        public bool HasEnemy;
        public NPC ShotAt;
        public float Endrotation;
        public Vector2 StartVel;
        public Vector2 FakeVel;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(241, 85, 64), new Color(133, 42, 42), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void SetDefaults()
        {

            AIType = ProjectileID.Bullet;
            Projectile.penetrate = 30;
            Projectile.width = 13;
            Projectile.height = 25;
        }
        public override void AI()
        {
            Projectile.ai[0]++;


            if (Projectile.ai[0] == 4)
            {
                Projectile.velocity = StartVel;
            }
            if (Projectile.ai[0] >= 4 && Projectile.ai[0] <= 50)
            {
                Projectile.velocity.Y += 0.01f;
                Projectile.velocity *= 0.95f;
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Endrotation, 0.14f);

            }
            if (HasEnemy)
            {
                if (Projectile.ai[0] == 20)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GlocketRouncher2"), Projectile.position);
                }
                if (Projectile.ai[0] == 50)
                {
                    Vector2 direction = Vector2.Normalize(ShotAt.Center - Projectile.Center) * 8.5f;


                    float rot = direction.ToRotation();
                    float spread = 0.4f;

                    Vector2 offset = new Vector2(1, -0.1f * Projectile.direction).RotatedBy(rot);
                    for (int k = 0; k < 15; k++)
                    {
                        Vector2 direction2 = offset.RotatedByRandom(spread);

                        Dust.NewDustPerfect(Projectile.Center + offset * 43, ModContent.DustType<GlowDust>(), direction2 * Main.rand.NextFloat(8), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 0.5f));
                    }
                    Dust.NewDustPerfect(Projectile.Center + offset * 43, ModContent.DustType<GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
                    Dust.NewDustPerfect(Projectile.Center + offset * 43, ModContent.DustType<TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));

                    FXUtil.ShakeCamera(Projectile.Center, 1024f, 32f);

                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.Red, 1f).noGravity = true;
                    }

                    float offsetX = Main.rand.Next(-50, 50) * 0.01f;
                    float offsetY = Main.rand.Next(-50, 50) * 0.01f;
                    int damage = Main.expertMode ? 10 : 14;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, direction.X + offsetX, direction.Y + offsetY, ModContent.ProjectileType<DeathSnipe>(), Projectile.damage, 1,
                            Main.myPlayer, 0, 0);
                }
            }
            else
            {
                if (Projectile.ai[0] == 40)
                {
                    Projectile.timeLeft = 1;
                }
            }

            if (Projectile.ai[0] >= 50 && Projectile.ai[0] <= 150)
            {
                Vector2 Bdirection = Vector2.Normalize(Projectile.Center - ShotAt.Center) * 8.5f;
                Projectile.velocity = Bdirection;
                Projectile.alpha += 100;

            }
            bool flag25 = false;
            int jim = 1;
            for (int index1 = 0; index1 < 200; index1++)
            {
                if (Main.npc[index1].CanBeChasedBy(Projectile, false)
                    && Projectile.Distance(Main.npc[index1].Center) < 500
                    && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1))
                {
                    HasEnemy = true;
                    ShotAt = Main.npc[index1];
                    flag25 = true;
                    jim = index1;
                }
            }

            if (flag25)
            {
                float num1 = 10f;
                Vector2 vector2 = new(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                float num2 = Main.npc[jim].Center.X - vector2.X;
                float num3 = Main.npc[jim].Center.Y - vector2.Y;
                float num4 = (float)Math.Sqrt((double)num2 * num2 + num3 * num3);
                float num5 = num1 / num4;
                float num6 = num2 * num5;
                float num7 = num3 * num5;
                int num8 = 10;
                FakeVel.X = (FakeVel.X * (num8 - 1) + num6) / num8;
                FakeVel.Y = (FakeVel.Y * (num8 - 1) + num7) / num8;
                Endrotation = FakeVel.ToRotation() + MathHelper.PiOver2;
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = 20;
            StartVel = Projectile.velocity;
            Projectile.alpha = Math.Max(0, Projectile.alpha - 25);


        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 1;
            return false;
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.Red, 1f).noGravity = true;
            }
        }
    }
}