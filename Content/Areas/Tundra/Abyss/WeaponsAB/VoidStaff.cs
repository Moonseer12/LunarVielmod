using Stellamod.Common;
using Stellamod.Content.Buffs;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB
{
    public class VoidMinionBuff : MinionBuff<VoidMinionProj> { }

    public class VoidStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller.
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 1, 33, 0);
            Item.rare = ItemRarityID.Orange;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<VoidMinionBuff>();
            // No buffTime because otherwise the item tooltip would say something like "1 minute duration"
            Item.shoot = ModContent.ProjectileType<VoidMinionProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GSummon"), player.position);
            // Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position.
            position = Main.MouseWorld;
            return true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<PearlescentScrap>());
        }
    }


    public class VoidMinionProj : ModProjectile
    {
        Player Owner => Main.player[Projectile.owner];
        ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Jelly Minion");
            // Sets the amount of frames this minion has on its spritesheet
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 32;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
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
            Projectile.width = 18;
            Projectile.height = 28;
            // Makes the minion go through tiles freely
            Projectile.tileCollide = false;

            // These below are needed for a minion weapon
            // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.friendly = true;
            // Only determines the damage type
            Projectile.minion = true;
            // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.minionSlots = 2f;
            // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
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
        
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }


        public override void AI()
        {
            if (!SummonHelper.CheckMinionActive<VoidMinionBuff>(Owner, Projectile))
                return;

            SummonHelper.SearchForTargets(Owner, Projectile,
                out bool foundTarget,
                out float distanceFromTarget,
                out Vector2 targetCenter);
            if (foundTarget)
            {
                Timer++;
                Vector2 directionToTarget = Projectile.Center.DirectionTo(targetCenter);
                Vector2 offset = -directionToTarget * 200;
                Projectile.velocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, targetCenter + offset, 8);
                if (Timer > 20 && Timer % 7 == 0 && Timer < 100)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.position);
                    Vector2 velocity = directionToTarget * 16;
                    velocity = velocity.RotatedByRandom(MathHelper.PiOver4 / 2);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<VoidMinionSparkProj>(), Projectile.damage, Projectile.knockBack, Owner: Projectile.owner);
                }

                if (Timer > 100 && Timer < 150)
                {
                    //Idle
                    SummonHelper.CalculateIdleValues(Owner, Projectile,
                        out Vector2 vectorToIdlePosition,
                        out float distanceToIdlePosition);
                    SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
                }

                if (Timer >= 150)
                {
                    Timer = 0;
                }
            }
            else
            {
                //Idle
                SummonHelper.CalculateIdleValues(Owner, Projectile,
                    out Vector2 vectorToIdlePosition,
                    out float distanceToIdlePosition);
                SummonHelper.Idle(Projectile, distanceToIdlePosition, vectorToIdlePosition);
            }
            Visuals();
        }

        private void Visuals()
        {
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = Projectile.velocity.X * 0.05f;

            // This is a simple "loop through all frames from top to bottom" animation
            int frameSpeed = 5;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }

    public class VoidMinionSparkProj : ModProjectile
    {
        private bool Moved;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 250;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<AbyssalFlame>(), 200);
        }

        private float alphaCounter = 1;
        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.alpha = 255;
                Moved = true;
            }

            if (Projectile.ai[1] <= 1)
            {
                Projectile.scale = 1.5f;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
            }
            if (Main.rand.NextBool(3))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].velocity.Y += Main.rand.Next(-2, 2);
                Main.dust[dustnumber].velocity.X += Main.rand.Next(-2, 2);
                Main.dust[dustnumber].noGravity = true;
                Main.dust[dustnumber].noLight = false;
            }
            if (Projectile.ai[1] >= 90)
            {
                if (Projectile.scale >= 0)
                {
                    Projectile.scale -= 0.22f;
                }
                if (alphaCounter >= 0)
                {
                    alphaCounter -= 0.08f;
                }
            }

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation += 0.08f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 1.0f * Main.essScale);
        }
    }
}