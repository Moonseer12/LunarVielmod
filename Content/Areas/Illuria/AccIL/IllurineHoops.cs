using Stellamod.Common;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.AccIL
{
    public class LunarPlayer : ModPlayer
    {
        public bool hasMoonflareBand;
        private int Timer;
        public override void ResetEffects()
        {
            hasMoonflareBand = false;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            Timer--;
            if (hasMoonflareBand && Timer <= 0)
            {
                Timer = 30;
                float maxDetectRange = 1024;
                NPC[] npcs = NPCHelper.FindNPCsInRange(Player.position, maxDetectRange, -1);
                for (int n = 0; n < npcs.Length; n++)
                {
                    NPC npc = npcs[n];
                    if (!npc.HasBuff(ModContent.BuffType<MoonFlame>()))
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), npc.Center, Main.rand.NextVector2Circular(1, 1),
                            ModContent.ProjectileType<MoonFlameSlashProj>(), 1, 1, Player.whoAmI);
                    }
                }
            }
        }
    }

    public class MoonFlameSlashProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 110;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.aiStyle = -1;
        }

        public override bool ShouldUpdatePosition()
        {
            //Returning false here makes the position not change
            return false;
        }

        public override bool PreAI()
        {
            Projectile.ai[0]++;
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.ai[0] <= 1)
            {
                SoundStyle soundStyle = new("Stellamod/Assets/Sounds/RipperSlash1");
                soundStyle.PitchVariance = 0.5f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
                Main.LocalPlayer.GetModPlayer<ShakePlayer>().ShakeAtPosition(Projectile.Center, 512f, 2f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 7)
                {
                    Projectile.active = false;
                }
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            int buffType = ModContent.BuffType<MoonFlame>();
            if (!target.HasBuff(buffType))
            {
                target.AddBuff(buffType, 36000);
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }

    public class MoonFlame : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.life > npc.lifeMax / 2)
            {
                npc.life = npc.lifeMax / 2;
            }

            npc.lifeRegen -= 8;
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 1; i++)
                {
                    int d = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<Dusts.GlowDust>(), newColor: ColorFunctions.Niivin, Scale: 0.33f);
                    Main.dust[d].rotation = (Main.dust[d].position - npc.position).ToRotation() - MathHelper.PiOver4;
                    Main.dust[d].velocity *= 0.5f;
                }
            }
        }
    }

    public class IllurineHoops : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            player.GetModPlayer<LunarPlayer>().hasMoonflareBand = true;
            player.GetDamage(DamageClass.Magic) *= 1.1f;
            player.manaCost -= 0.1f;
            player.manaRegen += 1;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<IllurineScale, BlankAccessory>();
        }
    }
}