using Stellamod.Common;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.WeaponsTR
{
    public class VampirePlayer : ModPlayer
    {
        public bool lifesteal;
        public float cooldown;
        public override void ResetEffects()
        {
            base.ResetEffects();
            lifesteal = false;
        }

        public override void UpdateEquips()
        {
            base.UpdateEquips();
            cooldown--;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPCWithProj(proj, target, hit, damageDone);
            if (lifesteal)
            {
                float distanceToTarget = Vector2.Distance(Player.position, target.position);
                //10 tile radius
                if (distanceToTarget <= 320 && Main.rand.NextBool(6) && cooldown <= 0)
                {
                    cooldown = 30;
                    //Life steal for 5% of the damage
                    float healFactor = damageDone * 0.08f;
                    int healthToHeal = (int)healFactor;
                    healthToHeal = Math.Clamp(healthToHeal, 1, 20);
                    Player.Heal(healthToHeal);

                    int count = 8;
                    float degreesPer = 360 / (float)count;
                    for (int k = 0; k < count; k++)
                    {
                        float degrees = k * degreesPer;
                        Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                        Vector2 vel = direction * 2;
                        Dust.NewDust(target.Center, 0, 0, DustID.BloodWater, vel.X, vel.Y);
                    }
                    Dust.QuickDustLine(Player.Center, target.Center, 100f, Color.Red);
                    SoundEngine.PlaySound(SoundID.NPCHit18, target.Center);
                }
            }
        }
    }

    public class VampireTorchMinionProj : ModProjectile
    {
        public Vector2[] CirclePos = new Vector2[16];
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            Main.projFrames[Projectile.type] = 4;
            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

            Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
        }

        public sealed override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 42;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely

            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
            Projectile.minion = true; // Declares this as a minion (has many effects)// Declares the damage type (needed for it to deal damage)
            Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
        }

        // Here you can decide if your minion breaks things like grass or pots
        public override bool? CanCutTiles()
        {
            return false;
        }

        // This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!SummonHelper.CheckMinionActive<VampireTorchMinionBuff>(owner, Projectile))
                return;

            //This minion doesn't attack
            Projectile.Center = owner.Center - new Vector2(0, 96);
            Visuals();
        }

        private void Visuals()
        {
            Player owner = Main.player[Projectile.owner];
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            if (Main.rand.NextBool(12))
            {
                int count = 3;
                for (int k = 0; k < count; k++)
                {
                    Dust.NewDust(Projectile.position, 8, 8, DustID.Blood);
                }
            }

            DrawHelper.DrawCircle(owner.Center, VectorHelper.Osc(280, 320), CirclePos);
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }

    public class VampireTorchMinionBuff : MinionBuff<VampireTorchMinionProj>
    {
        private int _vampiricTimer;
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VampireTorchMinionProj>()] > 0)
            {
                player.buffTime[buffIndex] = 18000;
                player.statLifeMax2 /= 2;
                player.lifeRegenCount = 0;
                _vampiricTimer++;
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (!npc.CanBeChasedBy())
                        continue;

                    float distanceToNpc = Vector2.Distance(player.Center, npc.Center);
                    if (distanceToNpc < 320)
                    {
                        if (_vampiricTimer % 24 == 0)
                        {
                            if (player.whoAmI == Main.myPlayer)
                            {
                                player.Heal(Main.rand.Next(2, 4));
                            }

                        }
                        npc.AddBuff(ModContent.BuffType<VampiricFlames>(), 10);
                    }
                }
                player.GetDamage(DamageClass.Summon) += 0.3f;
                player.GetDamage(DamageClass.Magic) += 0.3f;
            }
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }

    public class VampiricFlames : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen -= 120;
            if (Main.rand.NextBool(4))
            {
                Vector2 offset = new(Main.rand.Next(0, npc.width), Main.rand.Next(0, npc.height));
                Dust.NewDustPerfect(npc.position + offset, ModContent.DustType<GlyphDust>(),
                    Velocity: -Vector2.UnitY * Main.rand.NextFloat(1f, 5f),
                    newColor: Color.Red,
                    Scale: Main.rand.NextFloat(1f, 2f));
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen -= 32;
            player.manaRegen -= 8;
            if (Main.rand.NextBool(4))
            {
                Vector2 offset = new(Main.rand.Next(0, player.width), Main.rand.Next(0, player.height));
                Dust.NewDustPerfect(player.position + offset, ModContent.DustType<GlyphDust>(),
                    Velocity: -Vector2.UnitY * Main.rand.NextFloat(1f, 5f),
                    newColor: Color.Red,
                    Scale: Main.rand.NextFloat(1f, 2f));
            }
        }
    }

    public class VampireScepter : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 40;
            Item.height = 48;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 1, 33, 0);
            Item.rare = ItemRarityID.LightRed;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.UseSound = SoundID.Item46;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<VampireTorchMinionBuff>();
            Item.shoot = ModContent.ProjectileType<VampireTorchMinionProj>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Only allow one
            if (player.ownedProjectileCounts[Item.shoot] > 0)
                return false;
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;
            return false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<TerrorFragments, BlankStaff>();
        }
    }
}