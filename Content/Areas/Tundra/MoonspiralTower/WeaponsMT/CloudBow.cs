using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Dusts;
using Stellamod.Core.Bases;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT;

public class CloudBow : BaseCrossbowItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 18;
        Item.knockBack = 6;
    }

    public override void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        base.ShootBow(player, source, shootParams);
        float numberProjectiles = 3;
        float rotation = MathHelper.ToRadians(10);

        Vector2 velocity = shootParams.velocity * shootParams.speed * 24;
        velocity *= 3;
        velocity *= shootParams.chargeStrength;
        Vector2 position = shootParams.position;
        float bowDamage = shootParams.damage * shootParams.chargeStrength;
        for (int i = 0; i < numberProjectiles; i++)
        {
            var EntitySource = player.GetSource_FromThis();
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
            Projectile crossShot = Projectile.NewProjectileDirect(EntitySource, position, perturbedSpeed,
                shootParams.projToShoot, (int)bowDamage, Item.knockBack, player.whoAmI);
            crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
        }
        for (int i = 0; i < 3; i++)
        {
            var EntitySource = player.GetSource_FromThis();
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(15)) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
            perturbedSpeed *= Main.rand.NextFloat(0.6f, 1f);
            Projectile.NewProjectile(EntitySource, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y,
                ModContent.ProjectileType<WobblingCloud>(), (int)(bowDamage * 0.3f), Item.knockBack, player.whoAmI);
        }
    }

    public override void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source, ShootParams shootParams)
    {
        base.StaminaShootBow(player, source, shootParams);

        Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
        fireVelocity *= 3;
        fireVelocity *= shootParams.chargeStrength;

        float bowDamage = shootParams.damage * shootParams.chargeStrength;
        Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity,
           ModContent.ProjectileType<CloudArrow>(), (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
        crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<PearlescentScrap, BlankBow>();
    }
}

public class WobblingCloud : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.friendly = true;
        Projectile.timeLeft = 120;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer == 1)
        {
            SoundStyle castSound = new SoundStyle("Stellamod/Assets/Sounds/MothlightStarCast1");
            castSound.PitchVariance = 0.3f;
            SoundEngine.PlaySound(castSound, Projectile.position);
        }

        if(Timer % 8 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.Cloud, Vector2.UnitY);
        }

        if(Timer < 30)
        {
            Projectile.velocity *= 0.85f;
        }
    
        Projectile.velocity.Y += MathF.Sin(Timer * 0.5f) * 0.1f;
        float outScale = (float)(Projectile.timeLeft / 30f);
        outScale = EasingFunction.InOutSine(outScale);
        Projectile.scale = outScale;
        DrawHelper.AnimateTopToBottom(Projectile, 4);
    }
}

public class CloudArrow : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 12;
        Projectile.height = 12;
        Projectile.friendly = true;
    }
    public override void AI()
    {
        base.AI();
        Timer++;
        if (Timer % 6 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, Projectile.velocity * 0.1f, 0, Color.White, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
        }
        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        if (Main.rand.NextBool(2))
            target.AddBuff(ModContent.BuffType<Clouded>(), 240);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        for (int i = 0; i < 4; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 0.1f, 0, Color.White, Main.rand.NextFloat(1f, 2f)).noGravity = true;
        }
    }
}

public class Clouded : ModBuff
{
    public override void SetStaticDefaults()
    {
        Main.debuff[Type] = true;
        Main.pvpBuff[Type] = true;
        Main.buffNoTimeDisplay[Type] = false;
    }

    public override void Update(NPC npc, ref int buffIndex)
    {
        base.Update(npc, ref buffIndex);
        if (Main.rand.NextBool(8))
        {
            Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<GlyphDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1.5f));
        }
        float risingSpeed = -2f;

        npc.defDefense -= 15;
        npc.lifeRegen -= 4;
        if (npc.velocity.Y > risingSpeed)
            npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, risingSpeed, 0.5f);
    }

    public override void Update(Player player, ref int buffIndex)
    {
        if (Main.rand.NextBool(8))
        {
            Dust.NewDust(player.position, player.width, player.height, ModContent.DustType<GlyphDust>(), newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1.5f));
        }

        float risingSpeed = -2f;
        if (player.velocity.Y > risingSpeed)
            player.velocity.Y = MathHelper.Lerp(player.velocity.Y, risingSpeed, 0.5f);
    }
}