using System.Collections.Generic;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.GunSwapping;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.WeaponsRC
{
    public class STARBUST : MiniGun
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 42;
            LeftHand = true;

            //Higher is faster
            AttackSpeed = 1;

            //Offset it so it doesn't hold gun by weird spot
            HolsterOffset = new Vector2(0, -6);
        }

        public override void Fire(Player player, Vector2 position, Vector2 velocity, int damage, float knockback)
        {
            base.Fire(player, position, velocity, damage, knockback);
            //Treat this like a normal shoot function
            float spread = 0.4f;
            for (int k = 0; k < 7; k++)
            {
                Vector2 newDirection = velocity.RotatedByRandom(spread);
                Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), newDirection * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, Color.DarkRed, 1);
            Vector2 vel = velocity * 16;
            vel = vel.RotatedByRandom(MathHelper.PiOver4 / 15);
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), position, vel,
                ModContent.ProjectileType<STARBULLING>(), damage, knockback, player.whoAmI);
            }
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"), position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"), position);
            }
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcaricMush, BlankGun>();
        }
    }

    public class STARBULLING : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.damage = 100;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.light = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1000;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float Timer2;

        public override void AI()
        {
            Timer2++;
            Projectile.rotation += 0.2f;
            if (Timer2 < 30)
            {
                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Vector2.UnitY.RotatedBy(j * 3.141591734f / 6f, default) * new Vector2(8f, 16f);
                    vector2 = vector2.RotatedBy(Projectile.rotation - 1.57079637f, default);
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.BoneTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }
            }

            // Trying to find NPC closest to the projectile


            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero


        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {

            behindNPCs.Add(index);

        }
        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public Player FindClosestNPC(float maxDetectDistance)
        {
            Player closestplayer = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs(max always 200)
            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player target = Main.player[k];
                // Check if NPC able to be targeted. It means that NPC is
                // 1. active (alive)
                // 2. chaseable (e.g. not a cultist archer)
                // 3. max life bigger than 5 (e.g. not a critter)
                // 4. can take damage (e.g. moonlord core after all it's parts are downed)
                // 5. hostile (!friendly)
                // 6. not immortal (e.g. not a target dummy)

                // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                // Check if it is within the radius
                if (sqrDistanceToTarget < sqrMaxDetectDistance)
                {
                    sqrMaxDetectDistance = sqrDistanceToTarget;
                    closestplayer = target;
                }

            }


            return closestplayer;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.Kill();
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/STAREXPULSION"));
            for (int j = 0; j < 10; j++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1f);


            }

            for (int i = 0; i < 7; i++)
            {
            }

            for (int i = 0; i < 20; i++)
            {




            }
            return false;

        }


        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }



    }
}
