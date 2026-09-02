using Stellamod.Common;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT
{
    public class RevengenceMinionBuff : MinionBuff<RevengenceMinion> { }

    public class SwordsOfRevengence : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item11;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<RevengenceMinionBuff>();
            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<RevengenceMinion>();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;

            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;
            return false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<PearlescentScrap, BlankStaff>();
        }
    }

    public class RevengenceMinion : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private ref float Timer => ref Projectile.ai[0];
        private ref float SpeedTimer => ref Projectile.ai[1];
        private ref float HitCount => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Irradiated Creeper");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
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
            Projectile.height = 30;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
                                            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)// Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage) // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.timeLeft = 1500;
            Projectile.minion = true;
            Projectile.minionSlots = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.extraUpdates = 2;
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        public override bool MinionContactDamage()
        {
            return true;
        }
        public override void AI()
        {
            Timer++;
            if (Timer % 36 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.LightBlue, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            Player player = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<RevengenceMinionBuff>(player, Projectile))
                return;

            NPC target = ProjectileHelper.FindNearestEnemyThroughWalls(Projectile.Center, 1024);
            if (target != null)
            {
                float progress = MathHelper.Clamp(Timer / 35f, 0f, 1f);
                float d = MathHelper.Lerp(3f, 45, progress);
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, target.Center, d);
                if (Projectile.velocity.Length() < 15)
                {
                    Projectile.velocity *= 1.5f;
                }

                if (Projectile.velocity == Vector2.Zero)
                {
                    Projectile.velocity.Y -= 1;
                }
            }
            else
            {
                SummonHelper.CalculateIdleValues(Owner, Projectile, Owner.Center, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var EntitySource = Projectile.GetSource_Death();
            for (float f = 0; f < 1; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MusicDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.LightBlue, Main.rand.NextFloat(0.2f, 1f)).noGravity = true;
            }
            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Black,
                    duration: Main.rand.NextFloat(12, 25),
                    baseSize: Main.rand.NextFloat(0.01f, 0.04f));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            Projectile.velocity = -Projectile.velocity;
            int Sound = Main.rand.Next(1, 6);
            SoundStyle shootSound = new("Stellamod/Assets/Sounds/MiniPistol");
            shootSound = new("Stellamod/Assets/Sounds/Starblast");
            shootSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(shootSound, Projectile.position);
            Timer = 1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

   

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}