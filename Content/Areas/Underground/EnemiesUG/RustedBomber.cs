using Stellamod.Common;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.EnemiesUG
{
    public class RustedBomber : ModNPC
    {
        private bool _attack;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 13;
            this.AddToMineshaft();
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 58;
            NPC.damage = 51;
            NPC.defense = 12;
            NPC.lifeMax = 70;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 63f;
            NPC.knockBackResist = 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            //NPC.velocity.X *= 0.98f;
            //Syncing the attack to the animation
            int frame = (int)NPC.frameCounter;
            if (frame == 0)
            {
                _attack = true;
            }

            if (frame == 7 && _attack)
            {
                _attack = false;
                Vector2 fireCenter = NPC.Center + new Vector2(0, -NPC.height / 2);
                if (MultiplayerHelper.IsHost)
                {

                    for (int i = 0; i < Main.rand.Next(2, 4); i++)
                    {
                        Vector2 velocity = new(0, -10);
                        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), fireCenter, velocity,
                            ModContent.ProjectileType<RustedBomb>(), 10, 4, Main.myPlayer);
                    }

                }

                for (int i = 0; i < 16; i++)
                {
                    Vector2 velocity = new(0, -10);
                    velocity = velocity.RotatedByRandom(MathHelper.ToRadians(45));
                    Dust.NewDustPerfect(fireCenter, DustID.Smoke, velocity);
                }

                SoundEngine.PlaySound(SoundID.Item14, NPC.position);
            }

            Visuals();
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(80))
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.IronOre, 1, 1, 5));
        }
    }

    public class RustedBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 190;
        }

        public override void AI()
        {
            //This makes it slow down
            Projectile.velocity.X *= 0.99f;

            //This makes it fall down
            Projectile.velocity.Y += 0.15f;

            //This makes the rotation effect scale with the velocity
            Projectile.rotation += Projectile.velocity.Length() * 0.11f;
            Visuals();
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
            }

            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, speed);
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.OrangeRed * 0.6f, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
}
