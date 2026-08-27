
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Venatici : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 41;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 43;
            Item.height = 10;
            Item.useTime = 35;
            Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.value = 100000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/TON618");
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<Venbullet>();
            Item.shootSpeed = 20f;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20, 4);
        }
    }
}
