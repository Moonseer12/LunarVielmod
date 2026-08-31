using Stellamod.Assets;
using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Common.Shaders;
using Stellamod.Content.Areas.Tundra.MoonspiralTower.AccMT;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT;

public class MooneskCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 15;
        Item.shoot = ModContent.ProjectileType<MooneskCardProj>();
    }
    public override int GetPowderSlotCount()
    {
        return 4;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<PearlescentScrap, BlankCard>();
    }
}

public class MooneskCardPlayer : ModPlayer
{
    public int hitCount;
}

public class MooneskCardProj : IgniterCardProjectile
{
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
        MooneskCardPlayer cardPlayer = Owner.GetModPlayer<MooneskCardPlayer>();
        cardPlayer.hitCount++;
        if (cardPlayer.hitCount >= 10)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center + new Vector2(0, -96), Vector2.Zero,
                ModContent.ProjectileType<MoonramMoon>(), damageDone * 10, Projectile.knockBack, Projectile.owner);
            cardPlayer.hitCount = 0;
        }
    }
    public override void DrawToRenderTargets()
    {
        base.DrawToRenderTargets();
        void DrawMoonyTrail(GraphicsDevice gDevice)
        {
            float GetSpiralDashTrailWidth(float completionRatio)
            {
                return MathHelper.SmoothStep(128, 96, completionRatio) * EasingFunction.QuadraticBump(completionRatio) * 0.35f;
            }
            float GetSpiralDashTrailWidth2(float completionRatio)
            {
                return GetSpiralDashTrailWidth(completionRatio) * 1.3f;
            }
            Color GetSpiralDashTrailColor(float completionRatio)
            {
                return Color.Lerp(Color.White, Color.Transparent, completionRatio);
            }


            BasicLaserShader bloomShader = ShaderContent.GetInstance<BasicLaserShader>();
            bloomShader.LaserTexture = AssetManager.LaserTextures.CometTrail;
            bloomShader.InnerColor = Color.SkyBlue;
            bloomShader.OuterColor = Color.DarkBlue;
            TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, bloomShader, Projectile.Size * 0.5f);

            BasicLaserShader basicLaserShader = ShaderContent.GetInstance<BasicLaserShader>();
            basicLaserShader.LaserTexture = AssetManager.LaserTextures.Aura;
            basicLaserShader.InnerColor = Color.SkyBlue;
            basicLaserShader.OuterColor = Color.DarkBlue;
            TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth2, basicLaserShader, Projectile.Size * 0.5f);


            basicLaserShader.InnerColor = Color.White;
            basicLaserShader.OuterColor = Color.DarkGray;
            TrailDrawer.Draw(Projectile.oldPos, GetSpiralDashTrailColor, GetSpiralDashTrailWidth, basicLaserShader, Projectile.Size * 0.5f);
        }
        PixelationManager.QueuePrimitivesDrawAction(DrawMoonyTrail);
    }
}