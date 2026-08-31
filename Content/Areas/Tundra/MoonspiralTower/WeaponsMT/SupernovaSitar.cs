using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.MoonspiralTower.WeaponsMT
{
    public class SupernovaSitar : ModItem
    {
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "SupernovaSitar", "(S) Explosion damage is out of this world!")
            {
                OverrideColor = new Color(108, 201, 255)

            };
            tooltips.Add(line);





            base.ModifyTooltips(tooltips);




        }
        public override void SetDefaults()
        {
            Item.damage = 34;
            Item.mana = 20;
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Guitar;
            Item.noMelee = true;


            Item.knockBack = 0f;
            Item.DamageType = DamageClass.Magic;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/bongo");
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Supernova>();
            Item.autoReuse = true;
            Item.crit = 28;
            Item.scale = 0.8f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;

        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            // Draw the periodic glow effect behind the item when dropped in the world (hence PreDrawInWorld)
            Texture2D texture = TextureAssets.Item[Item.type].Value;

            Rectangle frame;

            if (Main.itemAnimations[Item.type] != null)
            {
                // In case this item is animated, this picks the correct frame
                frame = Main.itemAnimations[Item.type].GetFrame(texture, Main.itemFrameCounter[whoAmI]);
            }
            else
            {
                frame = texture.Frame();
            }

            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;

            float time = Main.GlobalTimeWrappedHourly;
            float timer = Item.timeSinceItemSpawned / 240f + time * 0.04f;

            time %= 4f;
            time /= 2f;

            if (time >= 1f)
            {
                time = 2f - time;
            }

            time = time * 0.5f + 0.5f;

            for (float i = 0f; i < 1f; i += 0.25f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 8f).RotatedBy(radians) * time, frame, new Color(90, 70, 255, 50), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            for (float i = 0f; i < 1f; i += 0.34f)
            {
                float radians = (i + timer) * MathHelper.TwoPi;

                spriteBatch.Draw(texture, drawPos + new Vector2(0f, 4f).RotatedBy(radians) * time, frame, new Color(140, 120, 255, 77), rotation, frameOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<PearlescentScrap, BlankStaff>();
        }

    }
    
    public class Supernova : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("FrostShotIN");
            Main.projFrames[Projectile.type] = 25;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 280;
            Projectile.height = 280;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 50;
            Projectile.scale = 0.9f;

        }

        public float Timer2 = 0;
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public override void AI()
        {
            Projectile.rotation += 0.03f;
            Vector3 RGB = new(0.89f, 2.53f, 2.55f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            Timer2++;

            if (Timer2 <= 2)
            {
                ShakeScreenPosition.Shake = 5;
            }

        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 25)
                {
                    Projectile.frame = 0;
                }
            }
            return true;


        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {


            SoundEngine.PlaySound(SoundID.Item110, Projectile.position);
        }
    }
}
