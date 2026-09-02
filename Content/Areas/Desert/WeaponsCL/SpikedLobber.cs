using Stellamod.Common.MagicCauldron;
using Stellamod.Content.Areas.Junkyard.WeaponsJY;
using Stellamod.Content.Dusts;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Desert.WeaponsCL
{
    public class SpikedLobber : BaseJugglerItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.noUseGraphic = true;
            Item.useTime = 120;
            Item.useAnimation = 120;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.crit = 16;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SpikedLobberProj>();
            Item.shootSpeed = 28;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<GintzlMetal, BlankJuggler>();
        }
    }

    public class SpikedLobberProj : BaseJugglerProjectile
    {
        public override void AI()
        {
            base.AI();
            if (State == AIState.Catch)
            {
                if (Juggler.combo >= 5 && Timer % 5 == 0 && Timer < 30 && Main.myPlayer == Projectile.owner)
                {
                    //Spikes
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    velocity += new Vector2(0, -16);
                    SoundEngine.PlaySound(SoundID.Item108, Projectile.position);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SpikedLobberSpikeProj>(),
                        Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Juggler.combo >= 5)
            {
                FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
                SoundStyle fireBomb = new("Stellamod/Assets/Sounds/StormDragon_Bomb");
                SoundEngine.PlaySound(fireBomb, target.Center);

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.White, 1f).noGravity = true;
                }
                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }

                FXUtil.GlowCircleBoom(target.Center,
                         innerColor: Color.White,
                         glowColor: Color.Gray,
                         outerGlowColor: Color.Black, duration: 25, baseSize: 0.24f);
            }
        }
    }

    public class SpikedLobberSpikeProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.4f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<NailKaboom>(), 0, 0, Projectile.owner);
            Main.projectile[p].scale = 0.5f;
        }
    }
}