using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Ducanblitz
{
    public class DucanPlayer : ModPlayer
    {
        public bool DucanB;
        public int DucanBCooldown = 0;
        public override void ResetEffects()
        {
            DucanB = false;
        }
        public override void PostUpdate()
        {
            if (DucanB && DucanBCooldown == 520)
            {
                DucanBCooldown = 0;
            }
            if (DucanB && DucanBCooldown == 301)
            {
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Arcaneup"));
                for (int j = 0; j < 1; j++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(0.1f, 1f);
                    Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, speed * 3, ModContent.ProjectileType<Dulcans>(), 200, 1f, Player.whoAmI);
                }
            }
        }
        public override void PostUpdateEquips()
        {
            if (DucanB && DucanBCooldown > 350)
            {
                Player.GetDamage(DamageClass.Melee) *= 1.1f;
            }
        }
    }

    public class Dulcans : ModProjectile
    {
        int afterImgCancelDrawCount = 0;
        float ta = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spragald");
            // Sets the amount of frames this minion has on its spritesheet

            // This is necessary for right-click targeting

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;


            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public int Timer = 0;
        float jhe = 0;

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely
                                            // These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)// Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Melee; // Declares the damage type (needed for it to deal damage) // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.timeLeft = 1500;
            Projectile.scale = 0.7f;
            Projectile.CloneDefaults(ProjectileID.DeadlySphere);
            AIType = ProjectileID.DeadlySphere;
        }

        // Here you can decide if your minion breaks things like grass or pots	
        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Projectile.minionSlots = 0f;
            Projectile.minion = false;
            Projectile.tileCollide = false;
            if (ta > 1500)
            {
                afterImgCancelDrawCount++;
            }

            ta += 0.01f;
            jhe++;
            if (jhe == 1500)
            {
                Projectile.Kill();
                jhe = 0;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color afterImgColor = Main.hslToRgb(Projectile.ai[1], 1, 0.5f);
            //float opacityForSparkles = 1 - (float)afterImgCancelDrawCount / 30;
            afterImgColor.A = 40;
            afterImgColor.B = 50;
            afterImgColor.G = 50;
            afterImgColor.R = 50;

            Main.instance.LoadProjectile(ProjectileID.RainbowRodBullet);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = afterImgCancelDrawCount + 1; i < Projectile.oldPos.Length; i++)
            {
                //if(i % 2 == 0)
                float rotationToDraw;
                Vector2 interpolatedPos;
                for (float j = 0; j < 1; j += 0.25f)
                {
                    if (i == 0)
                    {
                        rotationToDraw = Utils.AngleLerp(Projectile.rotation, Projectile.oldRot[0], j);
                        interpolatedPos = Vector2.Lerp(Projectile.Center, Projectile.oldPos[0] + Projectile.Size / 2, j);
                    }
                    else
                    {
                        interpolatedPos = Vector2.Lerp(Projectile.oldPos[i - 1] + Projectile.Size / 2, Projectile.oldPos[i] + Projectile.Size / 2, j);
                        rotationToDraw = Utils.AngleLerp(Projectile.oldRot[i - 1], Projectile.oldRot[i], j);
                    }
                    Main.EntitySpriteDraw(texture, interpolatedPos - Main.screenPosition + Projectile.Size / 2, null, afterImgColor * (1 - i / (float)Projectile.oldPos.Length), rotationToDraw, texture.Size() / 2, 1, SpriteEffects.None, 0);
                }
            }

            return true;
        }
    }
    
    [AutoloadEquip(EquipType.Head)]
    public class DucanblitzCap : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 25; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {

            player.GetDamage(DamageClass.Melee) *= 1.24f;
            player.GetCritChance(DamageClass.Generic) += 10f;
            player.autoReuseGlove = true;

        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DucanblitzBreastplate>() && legs.type == ModContent.ItemType<DucanblitzThighs>();
        }

        // UpdateArmorSet allows you to give set bonuses to the armor.
        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);
            player.GetModPlayer<DucanPlayer>().DucanB = true;
            player.GetModPlayer<DucanPlayer>().DucanBCooldown++;
            player.aggro *= 2;
            player.hasPaladinShield = true;


        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }

    [AutoloadEquip(EquipType.Body)]
    public class DucanblitzBreastplate : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 25; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {

            // Increase how many minions the player can have by one
            player.statLifeMax2 += 10;
            player.GetDamage(DamageClass.Melee) *= 1.1f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }

    [AutoloadEquip(EquipType.Legs)]
    public class DucanblitzThighs : ModItem
    {
        public override void SetDefaults()
        {
            Item.defense = 20; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.4f;
            player.maxRunSpeed += 0.4f;
            player.runAcceleration += 0.3f;// Increase the movement speed of the player
            player.statLifeMax2 += 20;
            player.GetArmorPenetration(DamageClass.Melee) += 15f;
            player.GetDamage(DamageClass.Melee) *= 1.02f;
        }


        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }
}