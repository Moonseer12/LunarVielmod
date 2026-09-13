using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Content.TODO.Zui.Projectiles
{
    public class ZuiLASERWARN : ModNPC
    {
        public bool Down;
        public bool Lightning;
        public override void SetDefaults()
        {
            NPC.alpha = 255;
            NPC.width = 0;
            NPC.height = 0;
            NPC.damage = 0;
            NPC.defense = 8;
            NPC.lifeMax = 156;
            NPC.value = 30f;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.buffImmune[BuffID.Venom] = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.dontTakeDamage = true;
            NPC.friendly = true;
            NPC.dontCountMe = true;
            NPC.aiStyle = -1;
        }
        float alphaCounter = 0;
        float counter = 8;


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_47").Value;
            Main.spriteBatch.Draw(texture2D4, NPC.Center - Main.screenPosition, null, new Color((int)(45f * alphaCounter), (int)(35f * alphaCounter), (int)(15f * alphaCounter), 0), -NPC.rotation - MathHelper.PiOver2, new Vector2(30 / 2, 1028 / 2), 0.2f * (counter + 0.3f), SpriteEffects.None, 0f);
            return true;
        }

        Vector2 velocity;
        int gren = 0;
        int y = 0;
        public override void AI()
        {
            NPC.TargetClosest();
            gren++;
            Player target = Main.player[NPC.target];


            if (gren < 50)
            {

                velocity = NPC.Center.DirectionTo(target.Center) * 10;



                NPC.rotation = -velocity.ToRotation();


            }


            float ai1 = NPC.whoAmI;
            if (!Down)
            {
                alphaCounter += 0.04f;
                if (alphaCounter >= 5)
                {
                    Down = true;

                }
            }
            else
            {
                y++;
                if (y == 1)
                {
                    Vector2 LightPos;

                    int Sound = Main.rand.Next(1, 4);
                    if (Sound == 1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain1"));
                    }
                    if (Sound == 2)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain2"));
                    }
                    if (Sound == 3)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire__LightingRain3"));

                    }
                    FXUtil.ShakeCamera(NPC.Center, 2048f, 32f);
                    LightPos.X = NPC.Center.X;
                    LightPos.Y = NPC.Center.Y;
                    var EntitySource = NPC.GetSource_FromThis();
                    if (MultiplayerHelper.IsHost)
                    {



                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, velocity.X, velocity.Y,
                        ModContent.ProjectileType<ZuiRay>(), 250, 10, Main.myPlayer, ai0: NPC.whoAmI);

                        Projectile.NewProjectile(EntitySource, LightPos.X + 150, LightPos.Y + 150, 0, 0,
                        ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 1, Owner: Main.myPlayer, 0, 0);
                    }
                }

                if (y >= 10)
                {
                    if (alphaCounter <= 0)
                    {
                        NPC.active = false;

                    }
                    alphaCounter -= 0.29f;
                }


            }

            if (!Lightning)
            {
                Lightning = true;

            }
        }
    }
}

