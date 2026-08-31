using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class Antaciz : ModItem
    {
        public override void SetDefaults()
        {

            Item.damage = 11; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Melee;
            Item.useTime = 100; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 11;
            Item.UseSound = SoundID.Item1; // The sound that this Item plays when used.
            Item.shoot = ModContent.ProjectileType<AntacizProj>();
            Item.shootSpeed = 0f; // the speed of the projectile (measured in pixels per frame)
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcadizScrap, BlankSword>();
        }
    }

    public class AntacizProj : ModProjectile
    {
        public override string Texture => "Stellamod/Content/Areas/Fable/WeaponsFB/Antaciz";
        
        public override void SetDefaults()
        {
            Projectile.damage = 15;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 80;
            Projectile.width = 80;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotation = Projectile.rotation;
            player.RotatedRelativePoint(Projectile.Center);
            Projectile.rotation -= 0.5f;

            if (Main.mouseLeft && Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.Distance(Main.MouseWorld) / 12;
                Projectile.netUpdate = true;
            }
            else
            {
                Projectile.velocity = Projectile.DirectionTo(player.Center) * 20;
                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    Projectile.Kill();
                }
            }

            Vector3 RGB = new(2.55f, 2.55f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.Center, RGB.X, RGB.Y, RGB.Z);

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = rotation * player.direction;
            //Projectile.netUpdate = true;
        }
    }
}