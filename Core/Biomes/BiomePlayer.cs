using Stellamod.Assets.Biomes;
using Stellamod.Buffs;
using Stellamod.Common.Particles;
using Stellamod.Content.Areas.SpringHills;
using Stellamod.Content.Gores.Foreground;
using Stellamod.Core.Foreground;
using Stellamod.Visual.Particles;
using Stellamod.WorldG;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Biomes
{
    public static class BiomeExtensions
    {
        public static bool ZoneFable(this Player player) => player.InModBiome<FableBiome>();
        public static bool ZoneAbyss(this Player player) => player.InModBiome<AbyssBiome>();
        public static bool ZoneXixianVillage(this Player player) => player.InModBiome<XixVillageBiome>();
    }
    public class BiomePlayer : ModPlayer
    {
        private float _windCounter;
        public bool ZoneFable = false;
        public bool ZoneAbyss;
        public bool ZoneAurelus;
        public bool ZoneGovheil;
        public bool ZoneAlcadzia;
        public bool ZoneVillage;
        public bool ZoneCinder;
        public bool ZoneDrakonic;
        public bool ZoneIlluria;
        public bool ZoneBloodCathedral;
        public bool ZoneAshotiTemple;
        public bool ZoneMineshaft;
        public bool ZoneColloseum;
        public bool ZoneMothlight;
        public bool ZoneWonder;
        public bool ZoneIshtar;
        public bool ZoneSacredUnknowns;
        public bool ZoneEveroseVillage;
        public bool ZoneSpringHills;
        public bool ZoneMistyDungeon;
        public bool ZoneMistyDungeonAnywhere;
        public bool ZoneDesertTown;
        public bool ZoneMarsh;
        public bool ZonePunkerTown;
        public bool ZoneWorldsEnd;
        public bool ZoneMoonspiralTower;
        public bool ZoneForest;
        public bool ZoneJunkyard;
        public bool ZoneHarmonicCoralways;
        public bool ZoneAegislavSurface;
        public bool ZoneHeatedDepths;
        public bool ZoneEdgeoftheMoon;
        public bool ZoneDeepBelowCoralways
        {
            get
            {
                Player localPlayer = Player;
                StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();
                int heightOffset = 100;
                Rectangle biomeRect = new(stellaWorld.CoralwaysLocation.X, stellaWorld.CoralwaysLocation.Y + heightOffset, 1000, 1800 - heightOffset);
                return localPlayer.Center.ToTileCoordinates().Y > biomeRect.Bottom - 400 && localPlayer.Center.ToTileCoordinates().Y < biomeRect.Bottom;
            }
        }
        public bool ZoneCrimsonBridewell;
        public override void ResetEffects()
        {
            if (ZoneColloseum)
                Player.ZoneDesert = true;
        }
        public override void PostUpdate()
        {
            if (ZoneIshtar)// && !DownedBossTracker.IsDowned(DownedBossFlag.Zui))
            {
                Main.LocalPlayer.AddBuff(ModContent.BuffType<SigfriedsInsanity>(), 10);
            }
            if (ZoneIlluria)
            {
                if (Main.shimmerAlpha <= 1)
                {
                    Main.shimmerAlpha += 0.02f;
                }
                else
                {
                    Main.shimmerAlpha = 1.02f;
                }
                if (Main.shimmerBrightenDelay <= 0.2f)
                {
                    Main.shimmerBrightenDelay += 0.05f;
                }
                else
                {
                    Main.shimmerBrightenDelay = 0.811f;
                }
                if (Main.shimmerDarken <= 1.4f)
                {
                    Main.shimmerDarken += 0.06f;
                }
                else
                {
                    Main.shimmerDarken = 1.41f;
                }
            }
            else
            {
                if (Main.shimmerAlpha >= 0)
                {
                    Main.shimmerAlpha -= 0.01f;
                }
                else
                {
                    Main.shimmerAlpha = 0f;
                }
                if (Main.shimmerBrightenDelay >= 0f)
                {
                    Main.shimmerBrightenDelay -= 0.01f;
                }
                else
                {
                    Main.shimmerBrightenDelay = 0f;
                }
                if (Main.shimmerDarken >= 0f)
                {
                    Main.shimmerDarken -= 0.01f;
                }
                else
                {
                    Main.shimmerDarken = 0f;
                }
            }
        }
        public override void PostUpdateEquips()
        {
            Player.ZoneLihzhardTemple = BiomeTileCounts.InAshotiTemple;
        }
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            Player.ManageSpecialBiomeVisuals("Stellamod:Illuria", ZoneIlluria);
            if (Main.netMode == NetmodeID.Server)
                return;

            if (ZoneAlcadzia || ZoneWorldsEnd)
            {
                Main.GraveyardVisualIntensity = 0.4f;
            }

            if (Player.whoAmI == Main.myPlayer)
            {
   
                if (ZoneWorldsEnd)
                {
                    PetalStorm s = ScreenShader.GetInstance<PetalStorm>();
                    s.alpha = 1;
                }
                AddForegroundOrBackground();
                Player.ManageSpecialBiomeVisuals("Stellamod:Marsh", ZoneMarsh);
                Player.ManageSpecialBiomeVisuals("Stellamod:Aegislav", ZoneAegislavSurface);
                Player.ManageSpecialBiomeVisuals("Stellamod:HeatedDepths", ZoneHeatedDepths);


                if ((ZoneCinder || ZoneHeatedDepths || Player.ZoneUnderworldHeight) && !ZoneWonder)
                {
                    WorldDepthGradient depthGradient = ScreenShader.GetInstance<WorldDepthGradient>();
                    depthGradient.alpha = 1;


                    StellaWorld stellaWorld = ModContent.GetInstance<StellaWorld>();
                    float top = stellaWorld.HeatedDepthsStart;
                    float end = stellaWorld.HeatedDepthsEnd;
                    float steps = end - top;
                    float progress = (Player.position.ToTileCoordinates().Y - top) / steps;
                    Vector3 gradientStrength = new();
                    gradientStrength.X = MathHelper.Lerp(0f, 0.2f, progress);
                    gradientStrength.Y = MathHelper.Lerp(0.4f, 0.8f, progress);
                    gradientStrength.Z = 0.18f * 0.5f;
                    depthGradient.gradientStrength = gradientStrength;
                    depthGradient.gradientColor = Color.Red.ToVector3();
                }

                if (ZoneCinder)
                {
                    FlameParticles2();
                    return;
                }
                if (ZoneHeatedDepths)
                {
                    FlameParticles();
                }
            }
        }
        public static void FlameParticles2()
        {
            if (Main.rand.NextBool(2))
            {
                Vector2 pos = new();
                pos.X = Main.rand.Next(0, Main.screenWidth * 2);
                pos.Y = Main.rand.Next(0, Main.screenHeight);
                pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
                Particles.FaintSmokeDust.Spawn(FaintSmokeDustData.Default with { position = pos, velocity = -Vector2.UnitY * 0.1f, color = Color.White * 0.15F, timeleft = 180 });
            }
            if (Main.rand.NextBool(6))
            {
                Vector2 pos = new();
                pos.X = Main.rand.Next(0, Main.screenWidth * 2);
                pos.Y = Main.rand.Next(0, Main.screenHeight);
                pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
                Particles.FaintSmokeDust.Spawn(FaintSmokeDustData.Default with { position = pos, velocity = -Vector2.UnitY * 0.1f, color = Color.White * 0.15F, timeleft = 180 });
            }
            if (Main.rand.NextBool(2))
            {
                Vector2 pos = new();
                pos.X = Main.rand.Next(0, Main.screenWidth * 2);
                pos.Y = Main.rand.Next(0, Main.screenHeight);
                pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
                Particles.CinderEmberDust.Spawn(CinderEmberDustData.Default with { position = pos, velocity = -Vector2.UnitY * 0.1f, parallaxStrength = Main.rand.NextFloat(0.3f, 0.75f) });
            }
            if (Main.rand.NextBool(5))
            {
                Vector2 pos = new();
                pos.X = Main.rand.Next(0, Main.screenWidth * 2);
                pos.Y = Main.rand.Next(0, Main.screenHeight);
                pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
                Particles.CinderEmberDust.Spawn(CinderEmberDustData.Default with { position = pos, velocity = -Vector2.UnitY * 0.1f, parallaxStrength = Main.rand.NextFloat(0.3f, 0.75f) });
            }
            if (Main.rand.NextBool(2))
            {
                Vector2 pos = new();
                pos.X = Main.rand.Next(0, Main.screenWidth * 2);
                pos.Y = Main.rand.Next(0, Main.screenHeight);
                pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
                Particles.CinderEmberDustBackground.Spawn(CinderEmberDustData.Default with { position = pos, velocity = -Vector2.UnitY * 0.1f });
            }
        }
        public static void FlameParticles()
        {
            if (Main.rand.NextBool(12))
            {
                Vector2 pos = new Vector2();
                pos.X = Main.rand.Next(0, Main.screenWidth * 2);
                pos.Y = Main.rand.Next(0, Main.screenHeight);
                pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
                UnderworldFlameParticle.Spawn(pos, -Vector2.UnitY * 2 + -Vector2.UnitX, Scale: Main.rand.NextFloat(0.1f, 0.3f));
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = new Vector2();
                pos.X = Main.rand.Next(0, Main.screenWidth * 2);
                pos.Y = Main.rand.Next(0, Main.screenHeight);
                pos += Main.screenPosition - Main.screenWidth * Vector2.UnitX;
                UnderworldSmokeParticle.Spawn(pos, -Vector2.UnitY * 2 + -Vector2.UnitX, Scale: Main.rand.NextFloat(0.5f, 0.8f));
            }
        }
        private void AddForegroundOrBackground()
        {
            if (ZoneIlluria || ZoneIshtar || ZoneAbyss)
            {
                if (Main.rand.NextBool(5))
                {
                    ForegroundParticleRenderer.NewParticle<Starstrike>();
                }

                if (Main.rand.NextBool(5))
                {
                    ForegroundParticleRenderer.NewParticle<Snowstrike>();
                }
            }

            if (Main.raining && (Player.ZoneForest || ZoneVillage))
            {
                if (Main.rand.NextBool(5))
                {
                    ForegroundParticleRenderer.NewParticle<Cherryblossom>();
                }
            }

            if (Player.ZoneDesert)
            {
                if (Main.rand.NextBool(5))
                {
                    ForegroundParticleRenderer.NewParticle<Sandstrike>();
                }
            }

            if (ZoneMarsh)
            {
                if (Main.rand.NextBool(16))
                {
                    ForegroundParticleRenderer.NewParticle<MarshLeaf>();
                }
                if (Main.rand.NextBool(16))
                {
                    ForegroundParticleRenderer.NewParticle<MarshPetal>();
                }
            }
            if (ZoneWorldsEnd)
            {
                Main.windSpeedTarget = 50 * 0.01f;
                if (Main.rand.NextBool(32))
                {
                    float xPosition = Main.rand.Next(-(int)(Main.screenWidth * 0.25f), (int)(Main.screenWidth * 0.25f));
                    float yPosition = Main.rand.NextFloat(-Main.screenHeight * 0.25f, Main.screenHeight * 0.25f);
                    Vector2 pos = Main.LocalPlayer.Center + new Vector2(xPosition, yPosition);
                    SparkleParticle sp = SparkleParticle.Spawn(pos, Vector2.Zero, Scale: 0.7f);
                    sp.flickering = true;
                    sp.gravity = 0;
                    sp.fast = true;
                }
                if (Main.rand.NextBool(8))
                {
                    ForegroundParticleRenderer.NewParticle<GreyPetal>();
                }
            }

          
            if (ZoneAegislavSurface)
            {
                if (Main.rand.NextBool(8))
                {
                    ForegroundParticleRenderer.NewParticle<AegislavStrike>();
                }

     
            }
            SpringHillsForegroundBackground();
        }

        private void SpringHillsForegroundBackground()
        {
            //Only do this in spring hills
            if (!ZoneSpringHills && !Player.ZoneForest)
                return;
            _windCounter--;
            if (_windCounter <= 0)
            {
                if (Main.rand.NextBool(2))
                {
                    Main.windSpeedTarget = Main.rand.Next(-50, -25) * 0.01f;
                }
                else
                {
                    Main.windSpeedTarget = Main.rand.Next(25, 50) * 0.01f;
                }

                _windCounter = 1200;
            }
            //CHERRY BLOSSOM
            if (Main.rand.NextBool(20))
            {

                ForegroundParticleRenderer.NewParticle<Cherryblossom>();
            }

            if (Main.rand.NextBool(20))
            {
                ForegroundParticleRenderer.NewParticle<SpringFallingFlower>();
            }
        }
    }
}
