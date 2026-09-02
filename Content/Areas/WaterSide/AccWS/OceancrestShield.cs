using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.AccWS
{
    public class OceanShieldPlayer : ModPlayer
    {
        private Projectile _waterShieldProj;
        private int _cooldown;
        public bool hasOceanShield;

        public override void ResetEffects()
        {
            hasOceanShield = false;
        }

        public override void UpdateEquips()
        {
            if (Main.myPlayer != Player.whoAmI)
                return;
            if (hasOceanShield)
            {
                if (_cooldown != 0)
                {
                    _cooldown--;
                }
                else if (_waterShieldProj == null || !_waterShieldProj.active)
                {
                    _waterShieldProj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                        ModContent.ProjectileType<WaterShield>(), 0, 0, Player.whoAmI);
                }
                else
                {
                    _waterShieldProj.timeLeft = 60;
                }
            }
            else if (_waterShieldProj != null && _waterShieldProj.active)
            {
                _waterShieldProj.Kill();
                _waterShieldProj = null;
            }
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (hasOceanShield && modifiers.Dodgeable && _cooldown <= 0)
            {
                int cooldownInSeconds = 30;
                int cooldownInTicks = cooldownInSeconds * 60;

                _cooldown = cooldownInTicks;
                modifiers.FinalDamage *= 0f;

                int count = 48;
                float degreesPer = 360 / (float)count;
                for (int k = 0; k < count; k++)
                {
                    float degrees = k * degreesPer;
                    Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                    Vector2 vel = direction * 4;
                    Dust.NewDust(Player.Center, 1, 1, DustID.Water, vel.X, vel.Y);
                }

                _waterShieldProj.Kill();
                _waterShieldProj = null;
            }
        }
    }

    public class WaterShield : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.timeLeft = 60;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                float A = Main.rand.Next(0, 2);

                FXUtil.ShakeCamera(Projectile.Center, 512f, 10f);
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, -2f, 0, default, .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Water, 0f, -2f, 0, default, .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightBlue.ToVector3() * 1.75f * Main.essScale);
        }
    }
    
    public class OceancrestShield : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
            Item.defense = 4;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<OceanShieldPlayer>().hasOceanShield = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MusicalHarmonise, BlankAccessory>();
        }
    }
}
