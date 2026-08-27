using Stellamod.Common;
using Stellamod.Content.Areas.Hallowrooms.ArmorHR;
using Stellamod.Content.CommonMaterials;
using Stellamod.Dusts;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Hallowrooms.WeaponsHR
{
    public class JacksonPollockMinionBuff : MinionBuff<JacksonPollockMinionProj> { }

    public class JacksonPollock : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.knockBack = 3f;
            Item.mana = 20;
            Item.width = 76;
            Item.height = 80;
            Item.useTime = 18;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 0, 33, 0);
            Item.rare = ItemRarityID.LightPurple;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;

            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.buffType = ModContent.BuffType<JacksonPollockMinionBuff>();
            Item.shoot = ModContent.ProjectileType<JacksonPollockMinionProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Spawn at the mouse cursor position
            position = Main.MouseWorld;
            player.AddBuff(Item.buffType, 2);
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;

            player.UpdateMaxTurrets();
            return false;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<KaleidoscopicInk>());
        }
    }

    public class JacksonPollockMinionProj : ModProjectile
    {
        private int _counter;
        private const int Time_Between_Spills = 15;
        private const int Spill_Count = 3;
        public override void SetStaticDefaults()
        {
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            // These below are needed for a minion
            // Denotes that this projectile is a pet or minion
            Main.projPet[Projectile.type] = true;

            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            // Don't mistake this with "if this is true, then it will automatically home". It is just for damage reduction for certain NPCs
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 46;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;

            // Only determines the damage type
            //Projectile.minion = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Terraria.Projectile.SentryLifeTime;

            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 0f;

            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player p = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<JacksonPollockMinionBuff>(p, Projectile))
                return;

            _counter++;
            if (_counter > Time_Between_Spills)
            {
                Player owner = Main.player[Projectile.owner];
                for (int i = 0; i < Spill_Count; i++)
                {
                    float x = Main.rand.NextFloat(-32f, 32f);
                    float y = 16;

                    Vector2 randOffset = new Vector2(x, y);
                    Vector2 velocity = VectorHelper.VelocityDirectTo(
                        Projectile.Center,
                        Projectile.Center + randOffset, 4);

                    Projectile projectile = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<JacksonPollockProj>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
                    projectile.DamageType = DamageClass.Summon;
                }

                _counter = 0;
            }

            Visuals();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Main.DiscoColor, Color.Black, ref lightColor);
            return true;
        }

        private void Visuals()
        {
            float hoverSpeed = 5;
            float rotationSpeed = 2.5f;
            float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
            float rotation = VectorHelper.Osc(MathHelper.ToRadians(-5), MathHelper.ToRadians(5), rotationSpeed);
            Projectile.velocity = new Vector2(0, yVelocity);
            Projectile.rotation = rotation + MathHelper.ToRadians(180);

            //It needs to make two of those particles
            //Then have a delay before actually enabling the AI and void rift particle
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.28f);
        }
    }

    public class JacksonPollockProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactius2");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.FrostDaggerfish);
            AIType = ProjectileID.FrostDaggerfish;
            Projectile.penetrate = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Poisoned, 180);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 8)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), speed * 2, 0, default(Color), 4f).noGravity = false;
            }

            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob1>());
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 8)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }
            return false;
        }

        public override bool PreAI()
        {
            if (Main.rand.NextBool(3))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), speed * 2, 0, default(Color), 4f).noGravity = false;

            }

            if (Main.rand.NextBool(3))
            {

                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

            }

            if (Main.rand.NextBool(3))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob4>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;

            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
           
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob3>(), 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(2))
            {
                float speedXa = Main.rand.NextFloat(-80f, 80f);
                float speedYa = Main.rand.Next(-80, 80);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb1>(), (Projectile.damage * 2) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob3>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (Main.rand.NextBool(1))
            {
                float speedXa = Main.rand.NextFloat(-80f, 80f);
                float speedYa = Main.rand.Next(-80, 80);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb2>(), Projectile.damage + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
            }

            if (Main.rand.NextBool(4))
            {
                float speedXa = Main.rand.NextFloat(-80f, 80f);
                float speedYa = Main.rand.Next(-80, 80);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb3>(), (Projectile.damage * 3) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob2>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
            }

            if (player.GetModPlayer<ArtisanPlayer>().PPPaintI)
            {
                if (Main.rand.NextBool(4))
                {
                    float speedXa = Main.rand.NextFloat(-80f, 80f);
                    float speedYa = Main.rand.Next(-80, 80);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb7>(), (Projectile.damage * 4) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob5>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }

            if (player.GetModPlayer<ArtisanPlayer>().PPPaintII)
            {
                if (Main.rand.NextBool(7))
                {
                    float speedXa = Main.rand.NextFloat(-35f, 35f);
                    float speedYa = Main.rand.Next(-35, 35);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + speedXa, Projectile.Center.Y + speedYa, 0, 0, ModContent.ProjectileType<PaintBomb8>(), (Projectile.damage * 3) + player.GetModPlayer<ArtisanPlayer>().PPPaintDMG2, 1, Projectile.owner, 0, 0);
                    Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<PaintBlob1>(), (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                }
            }
        }
    }
}