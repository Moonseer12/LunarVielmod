using Stellamod.Content.MoonlightMagic.Elements;
using Stellamod.Content.MoonlightMagic.Forms;

using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.MoonlightMagic.Enchantments.Basic;

public class LanternFormEnchantment : BaseEnchantment
{
    public override float GetStaffManaModifier()
    {
        return 0.1f;
    }

    public override int GetElementType()
    {
        return ModContent.ItemType<BasicElement>();
    }


    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {

        return true;
    }

    public override void SpecialInventoryDraw(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        base.SpecialInventoryDraw(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        DrawHelper.DrawGlowInInventory(item, spriteBatch, position, Color.Gray);
    }

    public override void SetMagicDefaults()
    {
        Projectile.penetrate += 1;
        MagicProj.Form = FormRegistry.Lantern.Value;


    }




}
