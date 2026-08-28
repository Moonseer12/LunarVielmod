using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class PunkedUpChops : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 60;
            Item.width = 40;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Ranged;
            Item.value = 200;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.DD2_DarkMageAttack;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PunkedUpChopsP>();
            Item.shootSpeed = 20f;
            Item.autoReuse = true;
            Item.crit = 12;
            Item.noUseGraphic = true;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.consumable = true;
            Item.maxStack = 9999;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<MarshScrap, BlankJuggler>();
        }
    }
}