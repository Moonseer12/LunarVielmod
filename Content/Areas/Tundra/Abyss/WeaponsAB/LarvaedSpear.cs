using Stellamod.Common.MagicCauldron;
using Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.WeaponsAB
{
    public class LarvaedSpear : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Ranged;
            Item.UseSound = SoundID.DD2_DarkMageAttack;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LarvaeSpearP>();
            Item.shootSpeed = 20f;
            Item.autoReuse = true;
            Item.crit = 12;
            Item.noUseGraphic = true;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.consumable = false;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<ConvulgingMater, BlankJuggler>();
        }
    }

    public class LarvaeSpearP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactius2");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.JavelinFriendly);
            AIType = ProjectileID.JavelinFriendly;
            Projectile.penetrate = 1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Confused, 180);
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
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch);
            }
            return false;
        }
        public override bool PreAI()
        {
            if (Main.rand.NextBool(3))
            {

                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch);
            }
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch);
            }

            var EntitySource = Projectile.GetSource_Death();
            for (int i = 0; i < 5; i++)
            {
                Projectile.timeLeft = 2;
                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-4, 5), Main.rand.Next(-4, 5), ModContent.ProjectileType<LarveinScriputeProg2>(), Projectile.damage / 2, 1, Projectile.owner, 0, 0);
            }


            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterStorm"), Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(25.0), 0, default, 1f).noGravity = false;
            }
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.VenomStaff, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(25.0), 0, default, 1f).noGravity = false;
            }


        }

    }
}