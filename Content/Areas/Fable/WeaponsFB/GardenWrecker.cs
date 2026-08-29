using Stellamod.Common.GunSystem;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{

    public class GardenWrecker : BaseGun
    {
        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.shootSpeed = 13f;
            Item.scale = 0.8f;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GardenWreckerBullet>();
            Item.shootSpeed = 5;
            Item.noMelee = true;
            Item.noUseGraphic = true;
        }


        public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (float f = 0; f < 4; f++)
            {
                Vector2 shootVelocty = velocity.RotateRandom(0.3f);
                shootVelocty *= Main.rand.NextFloat(0.5f, 1.5f);
                Projectile.NewProjectile(source, position, shootVelocty, type, damage, knockback, player.whoAmI);
            }
            return base.GunShot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcadizScrap, BlankGun>();
        }
    }

    public class GardenWreckerBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5; // The length of old position to be recorded
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0; // The recording mode
        }

        public override void SetDefaults()
        {
            Projectile.width = 8; // The width of projectile hitbox
            Projectile.height = 8; // The height of projectile hitbox
            Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
            Projectile.penetrate = 3; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = true; // Can the projectile collide with tiles?
                                           // Set to above 0 if you want the projectile to update multiple time in a frame
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet; // Act exactly like default Bullet
        }
        public override void AI()
        {
            base.AI();
            Projectile.velocity.Y += 0.1f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                return true;
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShakeScreenPosition.Shake = 4;
            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ProjectileID.DaybreakExplosion, Projectile.damage * 0, 0f, Projectile.owner, 0f, 0f);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<SkullShotKaboom>(), (int)(Projectile.damage * 1.1), 0f, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/flameup"), target.position);
        }

        public override void OnKill(int timeLeft)
        {
            // This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
        }
    }

    public class SkullShotKaboom : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Boom");
            Main.projFrames[Projectile.type] = 42;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 42;
            Projectile.scale = 1.3f;

        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 38, 22, 0) * (1f - Projectile.alpha / 50f);
        }
        public override void AI()
        {
            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            Main.dust[dust].noGravity = true;

            Vector3 RGB = new(2.55f, 2.55f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            overWiresUI.Add(index);
        }
        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 42)
                {
                    Projectile.frame = 0;
                }
            }
            return true;


        }

    }
}