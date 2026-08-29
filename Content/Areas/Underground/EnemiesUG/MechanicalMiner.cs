using Stellamod.Common;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Underground.EnemiesUG
{
    public class MechanicalMiner : ModNPC
    {
        private int _lastDirection;
        private float _waitTimer;
        private float _attackTimer;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;
            this.AddToMineshaft();
        }

        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 48;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            NPC.damage = 40;
            NPC.defense = 12;
            NPC.lifeMax = 170;
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f };
            NPC.value = 563f;
            NPC.knockBackResist = .45f;
            AIType = NPCID.SnowFlinx;
        }

        public override void AI()
        {
            Player target = Main.player[NPC.target];
            if (NPC.HasValidTarget &&
                Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height))
            {
                _attackTimer++;
                if (_attackTimer == 60)
                {
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton, NPC.position);
                }
                else if (_attackTimer > 60 && _attackTimer < 120)
                {
                    if (_attackTimer % 2 == 0)
                    {
                        Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.Electric);
                        dust.velocity *= -1f;
                        dust.scale *= 0.5f;
                        dust.noGravity = true;
                        Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                        vector2_1.Normalize();
                        Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                        dust.velocity = vector2_2;
                        vector2_2.Normalize();
                        Vector2 vector2_3 = vector2_2 * 34f;
                        dust.position = NPC.Center - vector2_3;
                    }


                    //Telegraph
                    NPC.FaceTarget();

                }
                else if (_attackTimer >= 120)
                {
                    if (MultiplayerHelper.IsHost)
                    {

                        Vector2 velocity = NPC.DirectionTo(target.Center) * 10;
                        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));

                        //make them always throw a little up
                        velocity += new Vector2(0, -Main.rand.NextFloat(5, 10));
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velocity,
                            ModContent.ProjectileType<RustedPickaxe>(), (int)(NPC.damage * 0.2f), 4, Main.myPlayer);
                    }

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SkyrageShasher"));
                    _attackTimer = 0;
                }
            }

            _waitTimer++;
            if (_waitTimer < 120)
            {
                NPC.velocity.X *= 0;
                NPC.spriteDirection = _lastDirection;
            }
            else
            {
                NPC.spriteDirection = -NPC.direction;
                _lastDirection = NPC.spriteDirection;
            }

            if (_waitTimer >= 400)
            {
                _waitTimer = 0;
            }

        }

        public override void FindFrame(int frameHeight)
        {
            if (_waitTimer < 120)
            {
                //Idle
                NPC.frameCounter += 0.2f;
                NPC.frameCounter %= Main.npcFrameCount[NPC.type];
                if (NPC.frameCounter >= 6)
                    NPC.frameCounter = 0;
                int frame = (int)NPC.frameCounter;
                NPC.frame.Y = frame * frameHeight;
            }
            else
            {
                //Moving
                NPC.frameCounter += 0.2f;
                NPC.frameCounter %= Main.npcFrameCount[NPC.type];
                if (NPC.frameCounter < 6)
                    NPC.frameCounter = 6;
                int frame = (int)NPC.frameCounter;
                NPC.frame.Y = frame * frameHeight;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector3 huntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                Color.White.ToVector3(),
                Color.WhiteSmoke.ToVector3(),
                new Vector3(3, 3, 3), 0);

            DrawHelper.DrawDimLight(NPC, huntrianColorXyz.X, huntrianColorXyz.Y, huntrianColorXyz.Z, Color.White, Color.WhiteSmoke, 0);
            Lighting.AddLight(screenPos, Color.White.ToVector3() * 1.0f * Main.essScale);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.IronOre, 1, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ItemID.SpelunkerGlowstick, minimumDropped: 1, maximumDropped: 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.Coal, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.MiningPants, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 1));
            npcLoot.Add(ItemDropRule.Common(ItemID.MiningShirt, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 1));
            npcLoot.Add(ItemDropRule.Common(ItemID.MiningHelmet, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 1));
        }
    }

    public class RustedPickaxe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            //This makes it slow down
            Projectile.velocity.X *= 0.99f;

            //This makes it fall down
            Projectile.velocity.Y += 0.15f;

            //This makes the rotation effect scale with the velocity
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;
            Visuals();
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
            }

            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed);
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Cyan, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
}
