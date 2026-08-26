using Stellamod.Content.Areas.Desert.WeaponsCL;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Core.Pixelation;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.WeaponsPT;

public class StaffoftheIrradiaflare : ModItem
{
    private int _dir;

    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.staff[Item.type] = true;
        Item.damage = 90;
        Item.width = 50;
        Item.height = 50;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 4;
        Item.value = Item.sellPrice(0, 1, 1, 29);
        Item.autoReuse = true;
        Item.DamageType = DamageClass.Magic;
        Item.shoot = ModContent.ProjectileType<ITProj>();
        Item.shootSpeed = 15f;
        Item.mana = 25;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.consumeAmmoOnLastShotOnly = true;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-5f, 0f);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (_dir == 0)
        {
            _dir = 1;
        }
        else
        {
            _dir *= -1;
        }

        Projectile.NewProjectile(source, position, velocity * Main.rand.NextFloat(0.6f, 1f), type, damage, knockback, player.whoAmI);
        var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
            ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: _dir);
        //(p.ModProjectile as StaffWaveHold).MagicCircleStyle = 1;
        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<MarshScrap>());
    }
}

public class ITExplosionProj : ModProjectile, IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void AI()
    {
        base.AI();
        Timer++;
        if(Timer == 1)
        {
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGreen, Color.DarkGreen, 6, baseSize: 0.24f);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.LightGreen, 24, 128);
            PixelPrimitiveCircleFactory.CreateGenericBoom(Projectile.Center, Color.White, Color.LightGreen, 24, 100);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 256;
        Projectile.height = 256;
        Projectile.friendly = true;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 30;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }

    public void DrawToRenderTargets()
    {

    }
}

public class ITProj : ModProjectile
{
    bool Moved;
    float WhiteTimer;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
    }
    public override void SetDefaults()
    {
        Projectile.penetrate = 5;
        Projectile.width = 17;
        Projectile.height = 16;
        Projectile.timeLeft = 860;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }
    public override void AI()
    {
        Projectile.velocity *= .96f;
        Projectile.ai[1]++;
        if (!Moved && Projectile.ai[1] >= 0)
        {
            SoundStyle useSound = new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/IrradiatedNest_Fall");
            useSound = useSound with { PitchVariance = 0.6f, Volume = 0.4f };
            SoundEngine.PlaySound(useSound, Projectile.position);
            Projectile.spriteDirection = Projectile.direction;
            Moved = true;
        }
        if (Projectile.ai[1] == 30)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ITBeep");
            //Between -1 and 1f
            soundStyle.Volume = 0.35f;
            soundStyle.Pitch = 0.8f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            WhiteTimer = 1;
        }
        if (Projectile.ai[1] == 60)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ITBeep");
            //Between -1 and 1f
            soundStyle.Volume = 0.35f;
            soundStyle.Pitch = 0.9f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            WhiteTimer = 1;
        }
        if (Projectile.ai[1] == 90)
        {
            SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/ITBeep");
            //Between -1 and 1f
            soundStyle.Volume = 0.35f;
            soundStyle.Pitch = 1f;
            SoundEngine.PlaySound(soundStyle, Projectile.position);
            WhiteTimer = 1;
        }
        if (Projectile.ai[1] >= 120)
        {
            Projectile.Kill();
            WhiteTimer = 1;
        }

        if (Projectile.ai[1] >= 90)
        {
            if (Main.rand.NextBool(2))
            {
                var sp = SmokeParticle.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY);
                sp.fadeToColor = Color.Black;
                sp.initialColor = Color.DarkGray;
            }
            
        }
        WhiteTimer = MathHelper.Lerp(WhiteTimer, 0, 0.1f);
        Rectangle myRect = Projectile.getRect();
        foreach (var p in Main.ActiveProjectiles)
        {
            if (p.type != ModContent.ProjectileType<ITExplosionProj>())
                continue;
            if (p == Projectile)
                continue;
            Rectangle otherRect = p.getRect();
            if (Projectile.Colliding(myRect, otherRect))
            {
                if (Projectile.ai[1] <= 100)
                {
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ITPrimer"), Projectile.position);
                    Projectile.ai[1] = 111;
                }
            }
        }

        Projectile.spriteDirection = Projectile.direction;
    }
    public override void OnKill(int timeLeft)
    {
        var entitySource = Projectile.GetSource_Death();
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(entitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<ITExplosionProj>(), Projectile.damage, 1, Projectile.owner, 0, 0);
            Projectile.NewProjectile(entitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<IrradiatedBoom>(), Projectile.damage, 1, Projectile.owner, 0, 0);
        }

        SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);

        int S1 = Main.rand.Next(0, 3);
        if (S1 == 0)
        {
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb1"), Projectile.position);
        }
        if (S1 == 1)
        {
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb2"), Projectile.position);
        }
        if (S1 == 2)
        {
            SoundEngine.PlaySound(new SoundStyle($"{nameof(Stellamod)}/Assets/Sounds/ITBomb3"), Projectile.position);
        }
        FXUtil.ShakeCamera(Projectile.Center, 2048, 8);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for (int i = 0; i < Projectile.oldPos.Length; i++)
        {
            Vector2 drawCenter = Projectile.oldPos[i] + Projectile.Size * 0.5f;
            SpritebatchDrawer sbDrawer = SpritebatchDrawer.FromProjectile(Projectile);
            sbDrawer.worldPosition = drawCenter;
            sbDrawer.color = Color.Lerp(Color.Green, Color.Transparent, i / (float)Projectile.oldPos.Length) * 0.3f;
            Main.spriteBatch.Draw(sbDrawer);
        }
        SpritebatchDrawer sbDrawer2 = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(sbDrawer2);
        return false;
    }
    public override void PostDraw(Color lightColor)
    {
        Lighting.AddLight(Projectile.Center, Color.DarkSeaGreen.ToVector3() * 1.75f * Main.essScale);
        string glowTexture = Texture + "_White";
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(ModContent.Request<Texture2D>(glowTexture), Projectile.Center);
        //Lerping
        float progress = WhiteTimer;
        Color drawColor = Color.Lerp(Color.Transparent, Color.White, progress);
        drawer.color = drawColor;
        Main.spriteBatch.Draw(drawer);
    }
}


public class IrradiatedBoom : ModProjectile
{
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 60;
    }

    private int _frameCounter;
    private int _frameTick;
    public override void SetDefaults()
    {
        Projectile.localNPCHitCooldown = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.friendly = true;
        Projectile.width = 129;
        Projectile.height = 129;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
        Projectile.scale = 1f;
    }

    public override void AI()
    {
        Vector3 RGB = new(0.89f, 2.53f, 2.55f);
        Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
    }

    public override bool PreAI()
    {
        if (++_frameTick >= 1)
        {
            _frameTick = 0;
            if (++_frameCounter >= 60)
            {
                _frameCounter = 0;
            }
        }
        return true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Vector2 drawPosition = Projectile.Center - Main.screenPosition;
        float width = 129;
        float height = 129;
        Vector2 origin = new(width / 2, height / 2);
        int frameSpeed = 1;
        int frameCount = 60;
        Color w = Color.White;
        w.A = 0;
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Draw(texture, drawPosition, texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false), w, 0f, origin, 3.5f, SpriteEffects.None, 0f);
        return false;
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
    }
}