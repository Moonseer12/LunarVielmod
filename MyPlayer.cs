using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod
{
    public class MyPlayer : ModPlayer
    {
        public int SwordCombo;
        public int SwordComboR;
        public int OnionDamage = 0;
        public bool Onion1 = false;
        public bool Onion2 = false;
        public bool Onion3 = false;
        public bool Onion4 = false;
        private float shakeDrama;
        public bool RadiantBomb = false;
        public int RadiantBombCooldown = 0;

        public void ShakeAtPosition(Vector2 position, float distance, float strength)
        {
            LunarVeilClientConfig config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.ShakeToggle)
                return;
            shakeDrama = strength * (1f - Player.Center.Distance(position) / distance) * 0.5f;
        }

        public override void ModifyScreenPosition()
        {
            if (shakeDrama > 0.5f)
            {
                shakeDrama *= 0.92f;
                Vector2 shake = new(Main.rand.NextFloat(shakeDrama), Main.rand.NextFloat(shakeDrama));
                Main.screenPosition += shake;
            }
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            if (RadiantBomb && RadiantBombCooldown <= 0)
            {
                for (int d = 0; d < 4; d++)
                {
                    float speedXa = Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-1f, 1f);
                    float speedYa = Main.rand.Next(10, 15) * 0.01f + Main.rand.Next(-1, 1);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0, speedYa * 0, ModContent.ProjectileType<GoldsSpawnEffect>(), 490, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0.7f, speedYa * 0.6f, ModContent.ProjectileType<GoldsSlashProj>(), 400, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 0.5f, speedYa * 0.3f, ModContent.ProjectileType<GoldsSlashProj>(), 405, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 1.3f, speedYa * 0.3f, ModContent.ProjectileType<GoldsSlashProj>(), 405, 1f, Player.whoAmI);
                    Projectile.NewProjectile(Player.GetSource_OnHit(victim), (int)victim.Center.X, (int)victim.Center.Y, speedXa * 1f, speedYa * 1.5f, ModContent.ProjectileType<GoldsSlashProj>(), 401, 1f, Player.whoAmI);
                }
                RadiantBombCooldown = 220;
            }
        }

        public override void ResetEffects()
        {
            RadiantBomb = false;
            if (SwordComboR <= 0)
            {
                SwordCombo = 0;
                SwordComboR = 0;
            }
            else
            {
                SwordComboR--;
            }
            Onion1 = false;
            Onion2 = false;
            Onion3 = false;
            Onion4 = false;
            OnionDamage = 0;
        }

        public override void OnEnterWorld()
        {
            Main.NewText(LangText.Misc("EnterWorld"));
        }

        public override bool PreItemCheck()
        {
            if (SwordComboR > 0)
            {
                SwordComboR--;
                if (SwordComboR == 0)
                {
                    SwordCombo = 0;
                }
            }
            return true;
        }
    }
}