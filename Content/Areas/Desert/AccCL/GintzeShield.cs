using Stellamod.Common.MagicCauldron;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Desert.AccCL
{
    public class GintzeShield : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToShield(ModContent.ProjectileType<GintzeShieldHeld>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<GintzlMetal, BlankCard>();
        }
    }

    public class GintzeShieldHeld : AbstractShieldProjectile
    {
        public override void OnBlockMovement(NPC npc)
        {
            base.OnBlockMovement(npc);
            if (npc.boss)
                return;
            if (!npc.HasBuff<GintzeStanceBreak>())
            {
                for (float f = 0; f < 3; f++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(8, 8);
                    DustParticle.Spawn(npc.Center, vel);
                }

                var strike = GlowDonutParticle.Spawn(npc.Center, Vector2.UnitY.RotatedByRandom(0.5f));
                strike.xMult = 6;
                strike.Scale *= 0.2f;
                strike.rotOffset += MathHelper.PiOver2;

                var hit = SoundID.NPCHit53;
                hit.PitchVariance = 0.3f;
                hit.Volume = 0.5f;
                SoundEngine.PlaySound(hit, npc.position);
                npc.AddBuff(ModContent.BuffType<GintzeStanceBreak>(), 60000);
            }
        }
    }

    public class GintzeStanceBreak : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.life > npc.lifeMax * 0.75f)
                npc.life = (int)(npc.lifeMax * 0.75f);
        }
    }
}
