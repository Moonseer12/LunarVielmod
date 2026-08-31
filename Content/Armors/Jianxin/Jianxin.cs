using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Jianxin
{
    public class DragonsSurround : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 15;
        }

        private int _frameCounter;
        private int _frameTick;
        public override void SetDefaults()
        {
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.width = 350;
            Projectile.height = 350;
            Projectile.penetrate = -1;
            Projectile.timeLeft = int.MaxValue;
            Projectile.scale = 1f;
            Projectile.tileCollide = false;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center;

            bool hasSetBonus = owner.GetModPlayer<JianxinPlayer>().Waterwhisps;
            if (!hasSetBonus)
            {
                Projectile.Kill();
                return;
            }



            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

        }



        public override bool PreAI()
        {
            if (++_frameTick >= 3)
            {
                _frameTick = 0;
                if (++_frameCounter >= 15)
                {
                    _frameCounter = 0;
                }
            }
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float width = 350;
            float height = 350;
            Vector2 origin = new(width / 2, height / 2);
            int frameSpeed = 3;
            int frameCount = 15;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture, drawPosition,
                texture.AnimationFrame(ref _frameCounter, ref _frameTick, frameSpeed, frameCount, false),
                (Color)GetAlpha(lightColor), 0f, origin, 0.5f, SpriteEffects.None, 0f);
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }


    }

    public class WateryWhisp : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 16;
        }

        public override void SetDefaults()
        {
            Projectile.width = 220;
            Projectile.height = 220;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 6;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = int.MaxValue;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        float trueFrame = 0;
        public void UpdateFrame(float speed, int minFrame, int maxFrame)
        {
            trueFrame += speed;
            if (trueFrame < minFrame)
            {
                trueFrame = minFrame;
            }
            if (trueFrame > maxFrame)
            {
                trueFrame = minFrame;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center;

            bool hasSetBonus = owner.GetModPlayer<JianxinPlayer>().Waterwhisps;
            if (!hasSetBonus)
            {
                Projectile.Kill();
                return;
            }




            //Lighting
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);

            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
            UpdateFrame(1f, 1, 96);
        }


        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Rectangle rectangle = new Rectangle(0, 0, 220, 220);
            rectangle.X = ((int)trueFrame % 6) * rectangle.Width;
            rectangle.Y = (((int)trueFrame - ((int)trueFrame % 6)) / 6) * rectangle.Height;

            Vector2 origin = new Vector2(rectangle.Width / 2, rectangle.Height / 2);
            SpriteBatch spriteBatch = Main.spriteBatch;
            float drawRotation = Projectile.rotation;
            float drawScale = 0.6f;

            spriteBatch.Draw(texture, drawPosition,
               rectangle,
                (Color)GetAlpha(lightColor), drawRotation, origin, drawScale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class JianxinPlayer : ModPlayer
    {
        public bool Waterwhisps;
        public override void ResetEffects()
        {
            Waterwhisps = false;
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class JianxinMask : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 23; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetArmorPenetration(DamageClass.Generic) += 10;
            player.GetDamage(DamageClass.Generic) *= 1.1f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<JianxinCoat>() && legs.type == ModContent.ItemType<JianxinPants>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"Increases life regen by a great amount!" + "\nMove faster and go along with the watery winds." + "\nSummons in watery dragons to come and swirl around your character." + "\nThese dragons emit a great becoming amongst the lost dynasty and give 50 Health." + "\nThis aura also lessens enemy damage by 10% and damages enemies." + "\nEnemies are less likely to target you!"); // This is the setbonus tooltip

            player.statLifeMax2 += 50;
            player.moveSpeed += 0.3f;
            player.maxRunSpeed += 0.3f;
            player.lifeRegen += 2;  // This is the setbonus tooltip
            player.aggro *= 2;
            player.endurance += 0.10f;

            player.GetModPlayer<JianxinPlayer>().Waterwhisps = true;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<WateryWhisp>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<WateryWhisp>(), 120, 4, player.whoAmI);
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<DragonsSurround>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<DragonsSurround>(), 120, 4, player.whoAmI);
            }

        }




        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }

    [AutoloadEquip(EquipType.Body)]
    public class JianxinCoat : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 26; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetArmorPenetration(DamageClass.Generic) += 10;
            player.GetDamage(DamageClass.Generic) *= 1.1f;
            player.GetCritChance(DamageClass.Generic) += 20f;
        }



        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }

    [AutoloadEquip(EquipType.Legs)]
    public class JianxinPants : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 20; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetArmorPenetration(DamageClass.Generic) += 10;
            player.GetDamage(DamageClass.Generic) *= 1.1f;
            player.maxMinions += 3;
        }



        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }
}