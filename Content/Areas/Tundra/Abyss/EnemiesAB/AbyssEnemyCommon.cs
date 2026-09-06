using Stellamod.Common.Particles;
using Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB.Gores;
using Stellamod.Content.Particles;
using Stellamod.Core;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.EnemiesAB;

public static class AbyssEnemyCommon
{
    public static void HitAndDeathEffects(NPC NPC)
    {
        float numDust = 3;
        for (float n = 0; n < numDust; n++)
        {
            Vector2 inverseVelocity = -NPC.oldVelocity;
            inverseVelocity = inverseVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f);
            var dp = DustParticle.Spawn(NPC.Center, inverseVelocity);
            dp.dampening = 0.1f;
            dp.Scale *= 0.5f;
            dp.innerColor = Color.White;
            dp.outerColor = Color.SkyBlue;
        }
        if (NPC.life <= 0)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + velocity, velocity * 2, ModContent.GoreType<AbyssFeatherGore>());
                }

                var sound = AssetReferences.Assets.Sounds.NiiviWingFlap.Asset with { Pitch = 0.5f, PitchVariance = 0.3f };
                SoundEngine.PlaySound(sound, NPC.Center);
                for(int i = 0; i < Main.rand.Next(5, 8); i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(12, 12);
                    velocity.Y -= Main.rand.Next(4, 8);
                    ModContent.GetInstance<FeatherDust>().Spawn(FeatherDustData.Default with { 
                        position = NPC.Center + Main.rand.NextVector2Circular(16, 16), 
                        velocity = velocity,
                        scale = Main.rand.NextFloat(0.5f, 0.9f) * 0.8f,
                        timeLeft = Main.rand.Next(120, 240)});
                }
            }
        }
    }
}
