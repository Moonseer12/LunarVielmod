using Stellamod.Common.MagicCauldron;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.CommonMaterials;

public class MinersGold : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 7;
    }
    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<MinersGoldRarity>();
        Item.maxStack = Item.CommonMaxStack;
    }
}
public class IllurineScale : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 20;
    }
    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<IllurineScaleRarity>();
        Item.maxStack = Item.CommonMaxStack;
    }
    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);
    }
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
        return true;
    }
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        return true;
    }
}
public class RadiantNectar : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 23;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<RadiantNectarRarity>();
    }
}
public class ConvulgingMater : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 12;
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 5));
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        ItemID.Sets.ItemNoGravity[Item.type] = false;
    }
    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.35f * Main.essScale);
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<ConvulgingMatterRarity>();
    }
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
        return true;
    }
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        return true;
    }
}
public class MarshScrap : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 16;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<MarshScrapRarity>();
    }
}
public class MechanizedSoul : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 17;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<MechanizedSoulRarity>();
    }
}
public class FallenEyes : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 26;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<FallenEyesRarity>();
    }
}
public class MusicalHarmonise : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 13;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<MusicalHarmoniseRarity>();
    }
}
public class EreshkinCandle : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 22;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<SpidersSilkRarity>();
    }
}
public class MothlightWing : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 28;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<MothlightWingRarity>();
    }
}
public class GhastlySpirit : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 27;
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        ItemID.Sets.ItemNoGravity[Item.type] = true;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<GhastlySpiritRarity>();
    }
}
public class Mushroom : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 2;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<SpringMushroomRarity>();
    }
}
public class Ivythorn : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 3;
    }
    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<IvythornRarity>();
        Item.maxStack = Item.CommonMaxStack;
    }
}
public class AlcadizScrap : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 4;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<FableScrapRarity>();
    }
}
public class WinterbornShard : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 5;
    }
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Lighting.AddLight(Item.Center, Color.LightSkyBlue.ToVector3() * 1.25f * Main.essScale);
        return true;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<WinterbornShardRarity>();
    }
}
public class TerrorFragments : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 8;
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
        ItemID.Sets.ItemNoGravity[Item.type] = true;
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<TerrorFragmentRarity>();
    }
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Lighting.AddLight(new Vector2(Item.Center.X, Item.Center.Y), 81 * 0.001f, 194 * 0.001f, 58 * 0.001f);
        return true;
    }
}
public class GintzlMetal : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 9;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<GintzlMetalRarity>();
    }
}
public class Cinderscrap : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 10;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<CinderscrapRarity>();
    }
}
public class HypnotizedSoul : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 11;
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
        ItemID.Sets.ItemNoGravity[Item.type] = true;
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<HypnotizedSoulRarity>();
    }
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Lighting.AddLight(new Vector2(Item.Center.X, Item.Center.Y), 81 * 0.001f, 194 * 0.001f, 58 * 0.001f);
        return true;
    }
}
public class PearlescentScrap : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 14;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<PearlescentScrapRarity>();
    }
}
public class KaleidoscopicInk : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 18;
    }
    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<KaleidoscopicInkRarity>();
    }
}
public class MiracleThread : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 21;
    }
    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);
    }
    public override void SetDefaults()
    {
        Item.rare = ModContent.RarityType<MiracleThreadRarity>();
        Item.maxStack = Item.CommonMaxStack;
    }
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Blue);
        return true;
    }
    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        return true;
    }
    public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        float hoverSpeed = 5;
        float hoverRange = 0.2f;
        float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
        Vector2 position = new Vector2(Item.position.X, Item.position.Y + y);
        Item.position = position;
    }
}
public class AlcaricMush : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 25;
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
        ItemID.Sets.ItemNoGravity[Item.type] = true;
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
    }

    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<AlcaricMushRarity>();
    }
}