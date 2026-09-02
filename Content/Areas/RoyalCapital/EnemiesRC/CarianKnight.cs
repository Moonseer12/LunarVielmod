using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.EnemiesRC
{
    public class CarianKnight : ModNPC
    {
        private float ai_Counter;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 15;
        }

        public override void SetDefaults()
        {
            NPC.width = 56;
            NPC.height = 62;
            NPC.damage = 40;
            NPC.defense = 10;
            NPC.lifeMax = 975;
            NPC.value = 90f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = NPCAIStyleID.CursedSkull;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<AlcadziaBiome>())
            {
                return 0.6f;
            }

            //Else, the example bone merchant will not spawn if the above conditions are not met.
            return 0f;
        }

        public override void AI()
        {
            base.AI();
            ai_Counter++;

            Player player = Main.player[NPC.target];
            NPC.rotation = NPC.velocity.X * 0.03f;
            if (ai_Counter == 400)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 direction = NPC.DirectionTo(player.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, direction * 9,
                        ModContent.ProjectileType<CarianKnightProj>(), 40, 1, Main.myPlayer);
                }

                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GhostExcalibur1"));
                for (int i = 0; i < 16; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                    var d = Dust.NewDustPerfect(NPC.Center, DustID.GemAmethyst, speed, Scale: 1.5f);
                    d.noGravity = true;
                }
                ai_Counter = 0;
            }
            else if (ai_Counter > 300)
            {
                NPC.velocity *= 0.2f;
                float distance = 128;
                float particleSpeed = 8;

                Vector2 position = NPC.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                Vector2 speed = (NPC.Center - position).SafeNormalize(Vector2.Zero) * particleSpeed;
                var d = Dust.NewDustPerfect(position, DustID.GemAmethyst, speed, Scale: 2f);
                d.noGravity = true;
            }
            else if (ai_Counter == 300)
            {
                SoundEngine.PlaySound(SoundID.Zombie82, NPC.position);
                SoundEngine.PlaySound(SoundID.Zombie99, NPC.position);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcaricMush>(), 2, 1, 2));
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.3f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
    }

    public class CarianKnightProj : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.timeLeft = 240;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        private ref float ai_Counter => ref Projectile.ai[0];

        public override void AI()
        {
            base.AI();
            ai_Counter++;
            Player playerToHomeTo = Main.player[Main.myPlayer];
            float closestDistance = Vector2.Distance(Projectile.position, playerToHomeTo.position);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                float distanceToPlayer = Vector2.Distance(Projectile.position, player.position);
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    playerToHomeTo = player;
                }
            }

            if (ai_Counter < 70)
            {
                float speed = 8;
                Vector2 velocity;
                Vector2 direction = Projectile.DirectionTo(playerToHomeTo.Center);
                Vector2 maxVelocity = direction * (speed * ai_Counter / 60);
                float distanceToTarget = Vector2.Distance(playerToHomeTo.Center, Projectile.Center);
                if (distanceToTarget < speed)
                {
                    velocity = direction * distanceToTarget;
                }
                else
                {
                    velocity = maxVelocity;
                }


                Projectile.velocity = velocity;
            }

            Projectile.rotation++;
            Projectile.spriteDirection = Projectile.direction;
            if (Main.rand.NextBool(6))
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, Scale: 1.5f)];
                d.noGravity = true;
                d.fadeIn = 1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            FXUtil.ShakeCamera(Projectile.Center, 512f, 50f);
            for (int i = 0; i < 32; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, speed, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        Vector2 DrawOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.spriteDirection != 1)
            {
                DrawOffset.X = Projectile.Center.X - 18;
                DrawOffset.Y = Projectile.Center.Y;
            }
            else
            {
                DrawOffset.X = Projectile.Center.X - 25;
                DrawOffset.Y = Projectile.Center.Y;
            }


            Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Spiin").Value;
            float r = 234;
            float g = 118;
            float b = 135;

            for (int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(texture, DrawOffset - Main.screenPosition, null, new Color((int)r, (int)g, (int)b, 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            }

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3() * 1.75f * Main.essScale);
        }
    }
}