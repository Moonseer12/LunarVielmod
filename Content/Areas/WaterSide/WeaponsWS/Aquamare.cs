using Stellamod.Common.Shaders;
using Stellamod.Content.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.WeaponsWS;

    public class Aquamare : ModItem
    {
        public int Star;
        public override void SetDefaults()
        {
            Item.damage = 98;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.shootSpeed = 15;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<AquamareProj>();
            Item.shootSpeed = 10f;
            Item.mana = 10;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.consumeAmmoOnLastShotOnly = true;
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Star += 1;
            if (Star >= 1)
            {
                Star = 0;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Astalaiya3"), player.position);
                type = ModContent.ProjectileType<AquamareProj>();
            }
            if (Star == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/iceshake"), player.position);
                type = ModContent.ProjectileType<AquamareProj>();
            }
            if (Star == 3)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MoonBlow"), player.position);
                type = ModContent.ProjectileType<AquamareProj>();
            }
        }



    }

public class AquamareProj : ModProjectile
{
    float distance = 8;
    int rotationalSpeed = 4;
    bool initialized = false;
    Vector2 initialSpeed = Vector2.Zero;
    private ref float Timer => ref Projectile.ai[1];
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 300;
        Projectile.width = Projectile.height = 50;
        Projectile.hostile = false;
        Projectile.friendly = true;
    }

    public override void AI()
    {
        Timer++;
        if (Timer % 8 == 0)
        {
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Aquamarine, Main.rand.NextFloat(1f, 2f)).noGravity = true;
        }
        Projectile.velocity *= 0.991f;
        int rightValue = (int)Projectile.ai[1] - 1;
        if (rightValue < (double)Main.projectile.Length && rightValue != -1)
        {
            Projectile other = Main.projectile[rightValue];
            Vector2 direction9 = other.Center - Projectile.Center;
            direction9.Normalize();
        }
        if (!initialized)
        {
            initialSpeed = Projectile.velocity;
            initialized = true;
        }
        if (initialSpeed.Length() < 15)
            initialSpeed *= 1.01f;
        Projectile.spriteDirection = 1;
        if (Projectile.ai[0] > 0)
        {
            Projectile.spriteDirection = 0;
        }

        distance += 0.4f;
        Projectile.ai[0] += rotationalSpeed;

        Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
        offset.Normalize();
        offset *= (float)(Math.Cos(Projectile.ai[0] * (Math.PI / 180)) * (distance / 3));
        Projectile.velocity = initialSpeed + offset;
        Projectile.rotation -= 0.5f;
        Projectile.ai[0]++;
    }


    private void DrawEnergyBall()
    {
        //Draw Code for the orb
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 centerPos = Projectile.Center - Main.screenPosition;
        GlowCircleShader shader = GlowCircleShader.Instance;

        //How quickly it lerps between the colors
        shader.Speed = 10f;

        //This effects the distribution of colors
        shader.BasePower = 2.5f;

        //Radius of the circle
        shader.Size = 0.12f;


        //Colors
        Color startInner = Color.White;
        Color startGlow = Color.Lerp(Color.LightBlue, Color.CadetBlue, VectorHelper.Osc(0f, 1f, speed: 3f));
        Color startOuterGlow = Color.Lerp(Color.Blue, Color.Aquamarine, VectorHelper.Osc(0f, 1f, speed: 3f));

        shader.InnerColor = startInner;
        shader.GlowColor = startGlow;
        shader.OuterGlowColor = startOuterGlow;

        //Idk i just included this to see how it would look
        //Don't go above 0.5;
        shader.Pixelation = 0.005f;

        //This affects the outer fade
        shader.OuterPower = 13.5f;
        shader.Apply();


        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
        for (int i = 0; i < 2; i++)
        {
            spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
        }

        spriteBatch.RestartDefaults();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        DrawEnergyBall();

        return false;
    }
}