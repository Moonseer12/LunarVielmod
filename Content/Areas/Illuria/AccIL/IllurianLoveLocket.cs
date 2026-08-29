using Stellamod.Assets;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.AccIL
{
    public class IllurianLoveLocket : ModItem
    {
        private float _starTimer;
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            _starTimer--;
            if (_starTimer <= 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<IllurianLoveLocketStarProj>(), 150, 1, player.whoAmI);
                _starTimer = 10;
            }
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<IllurineScale, BlankAccessory>();
        }
    }

    public class IllurianLoveLocketStarProj : ModProjectile
    {
        float Timer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Projectile.hide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            OvalMovement();
            Projectile.rotation += 0.05f;
        }

        private void OvalMovement()
        {
            Player owner = Main.player[Projectile.owner];
            Timer++;
            if (Timer >= 120)
            {
                Timer = 0;
            }

            //Make an oval
            float xRadius = 96;
            float yRadius = 48;
            float angle = MathHelper.TwoPi;
            float ovalProgress = Timer / 120;
            float xOffset = xRadius * MathF.Cos(ovalProgress * angle);
            float yOffset = yRadius * MathF.Sin(ovalProgress * angle);
            float rotation = MathHelper.PiOver4 / 1.5f;
            Vector2 pointOnOval = owner.Center + new Vector2(xOffset, yOffset).RotatedBy(rotation);
            Projectile.Center = pointOnOval;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public static Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.White, ColorFunctions.Niivin, 0.5f);
            return Color.Lerp(color, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.StarTrail);
            return base.PreDraw(ref lightColor);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Timer >= 120)
            {
                behindNPCsAndTiles.Add(index);
            }
            else
            {
                overPlayers.Add(index);
            }
        }
    }
}