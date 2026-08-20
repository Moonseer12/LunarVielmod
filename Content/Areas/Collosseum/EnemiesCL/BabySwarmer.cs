using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.Content.Areas.Collosseum.EnemiesCL
{
    public class BabySwarmer : ModNPC
    {
        private float Speed
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        private float WanderX
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        private float WanderY
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 34;
            NPC.damage = 54;
            NPC.defense = 8;
            NPC.lifeMax = 333;
            NPC.HitSound = SoundID.NPCHit31;
            NPC.DeathSound = SoundID.NPCDeath34;
            NPC.value = 563f;
            NPC.knockBackResist = .45f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.npcSlots = 0.2f;
        }

        private void AI_MoveToward(Vector2 targetCenter, float maxSpeed = 12)
        {
            //chase target
            float acceleration = 1;

            //Accelerate
            Speed += acceleration;
            Speed = MathHelper.Clamp(Speed, 0, maxSpeed);

            Vector2 directionToTarget = NPC.Center.DirectionTo(targetCenter);
            Vector2 targetVelocity = directionToTarget * Speed;

            if (NPC.velocity.X < targetVelocity.X)
            {
                NPC.velocity.X++;
                if (NPC.velocity.X >= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }
            else if (NPC.velocity.X > targetVelocity.X)
            {
                NPC.velocity.X--;
                if (NPC.velocity.X <= targetVelocity.X)
                {
                    NPC.velocity.X = targetVelocity.X;
                }
            }

            if (NPC.velocity.Y < targetVelocity.Y)
            {
                NPC.velocity.Y++;
                if (NPC.velocity.Y >= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
            else if (NPC.velocity.Y > targetVelocity.Y)
            {
                NPC.velocity.Y--;
                if (NPC.velocity.Y <= targetVelocity.Y)
                {
                    NPC.velocity.Y = targetVelocity.Y;
                }
            }
        }

        public override void AI()
        {
            NPC.TargetClosest();
            NPC.spriteDirection = -NPC.direction;
            if (MultiplayerHelper.IsHost && Main.rand.NextBool(20))
            {
                Speed /= 2;
                WanderX = Main.rand.NextFloat(-10f, 10f);
                WanderY = Main.rand.NextFloat(-10f, 10f);
                NPC.netUpdate = true;
            }

            Player target = Main.player[NPC.target];
            if (NPC.HasValidTarget &&
                Collision.CanHitLine(NPC.position, NPC.width, NPC.height, target.position, target.width, target.height))
            {
                Vector2 targetCenter = target.Center + new Vector2(WanderX * 20, WanderY * 20);
                AI_MoveToward(targetCenter, maxSpeed: 6);
            }
            else
            {


                Vector2 targetCenter = NPC.Center + new Vector2(WanderX, WanderY);
                AI_MoveToward(targetCenter, maxSpeed: 3);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.34f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override int SpawnNPC(int tileX, int tileY)
		{
            for (int i = 0; i < Main.rand.Next(2, 7); i++)
            {
                NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, NPC.type);
            }
			return NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, NPC.type);
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (!NPC.downedPlantBoss)
                return 0;
            return (SpawnCondition.DesertCave.Chance * 0.05f) + (SpawnCondition.OverworldDayDesert.Chance * 0.05f);
        }
    }
}