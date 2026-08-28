using Stellamod.Common.IgnitersNPowders;
using Stellamod.Common.MagicCauldron;
using Stellamod.Content.CommonMaterials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.WeaponsPT;

public class GhetsisCard : BaseIgniterCard
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 30;
        Item.shoot = ModContent.ProjectileType<GhetsisCardProj>();
    }
    public override int GetPowderSlotCount()
    {
        return 4;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MarshScrap, BlankCard>();
    }
}

public class GhetsisCardProj : IgniterCardProjectile
{
    private int _hitCount;
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    protected override void OnExplode()
    {
        _hitCount++;
        Projectile.velocity.X *= -1;
        Projectile.velocity.Y -= 7;
        if(_hitCount >= 3)
        {
            Projectile.Kill();
        }
    }
}